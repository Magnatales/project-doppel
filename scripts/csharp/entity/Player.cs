using System.Threading.Tasks;
using Code.Entity;
using Code.Networking.Packets;
using Code.Service;
using Code.Utils;
using Godot;
using projectdoppel.scripts.csharp;
using Steamworks;
using Steamworks.Data;

public partial class Player : Node2D, ITarget
{
    [Export] public float Speed;
    [Export] public NavigationAgent2D navAgent;

    [Export] private PackedScene packedScene;
    [Export] private AnimatedSprite2D animSprite;
    [Export] private ProgressBar healthBar;
    [Export] private Area2D area;
    [Export] private Area2D mouseTargetArea;
    [Export] private Label _label;
    [Export] private Camera2d _camera;
    
    //[Export] private MultiplayerSynchronizer _multiplayerSynchronizer;
    [Export] private MultiplayerSpawner _multiplayerSpawner;
    
    public bool IsDead => currentHealth <= 0;
    public Vector2 Pos => GlobalPosition;
    
    private int maxHealth = 100;
    private int currentHealth;
    
    private Vector2 _targetPosition;
    private float oldX;
    private float oldY;
    private Vector2 _position;
    private EntityAnimator _entityAnimator;
    private Vector2 _velocity = Vector2.Zero;
    private ITarget _target;
    
    public uint NetworkId { get; private set; }
    public ulong NetworkOwner { get; private set; }

    public string NickName;

    public override void _EnterTree()
    {
        //_multiplayerSynchronizer.SetMultiplayerAuthority((int)long.Parse(Name));
    }

    public override void _Ready()
    {
        animSprite.Play("IdleDown");
        _entityAnimator = new EntityAnimator(this, navAgent, animSprite);
        currentHealth = maxHealth;
        healthBar.Value = currentHealth;
        mouseTargetArea.AreaEntered += OnTargetAreaEntered;
        mouseTargetArea.AreaExited += OnTargetAreaExited;
        
        if (HasOwnership())
        {
            _camera.MakeCurrent();
        } 
        _label.Text = $"{NickName}";
        animSprite.
    }
    

    public void SetPawn(uint networkId, ulong networkOwner, string nickName)
    {
        NetworkId = networkId;
        NetworkOwner = networkOwner;
        NickName = nickName;
    }

    public override void _ExitTree()
    {
        mouseTargetArea.AreaEntered -= OnTargetAreaEntered;
        mouseTargetArea.AreaExited -= OnTargetAreaExited;
    }
    
    public void DisplayHp(bool value)
    {
        
    }
    
    public override void _Process(double delta)
    {
        if (currentHealth <= 0) return;
        if (!HasOwnership()) return;
        // if (_multiplayerSynchronizer.GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
        // {
        //     return;
        // }
        _entityAnimator.Update();
        mouseTargetArea.GlobalPosition = GetGlobalMousePosition();
       
        if (navAgent.IsNavigationFinished())
        {
            GlobalPosition = GlobalPosition.Floor();
        }
        
        if (!navAgent.IsNavigationFinished() && !navAgent.IsTargetReached())
        {
            var direction = Vector2.Zero;
            direction = navAgent.GetNextPathPosition() - GlobalPosition;
            direction = direction.Normalized();
            _velocity = _velocity.Lerp(direction * Speed, (float)(20 * delta));
            GlobalPosition += _velocity * (float) delta;
        }
        
        if (Input.IsActionJustPressed("LeftClick"))
        {
            if(_target != null)
            {
                navAgent.SetTargetPosition(GlobalPosition.Floor());
                GlobalPosition = GlobalPosition.Floor();
               _target.TakeDamage(70, this);
                return;
            }
            _targetPosition = GetGlobalMousePosition();
            navAgent.SetTargetPosition(_targetPosition);
        }
        
        if (Input.IsActionJustPressed("RightClick"))
        {
            //Rpc(nameof(ShowUniqueID));
            if(!Multiplayer.IsServer()) return;
            // for (int i = 0; i < 1; i++)
            // {
            //     EnemySpawner.Instance.SpawnEnemy(new Vector2(600, 100));
            //      // var enemy2 = new Enemy();
            //      // GetParent().AddChild(enemy2);
            //      // var enemy = packedScene.Instantiate<Enemy>();
            //      // enemy.GlobalPosition = new Vector2(GetGlobalMousePosition().X + i, GetGlobalMousePosition().Y + i);
            //      // GetTree().Root.AddChild(enemy);
            // }
        }
        
    }
    
    public bool HasOwnership()
    {
        return SteamClient.SteamId == NetworkOwner;
    }
    
    private void OnTargetAreaEntered(Area2D area2D)
    {
        if(area2D.GetParent() is not ITarget target) return;
        if (_target != null)
        {
            return;
        }
        target.DisplayHp(true);
        _target = target;
    }


    private void OnTargetAreaExited(Area2D area2D)
    {
        if(area2D.GetParent() is not ITarget target) return;
        target.DisplayHp(false);
        _target = null;

    }

    public void Log(string text)
    {
        GD.Print(text);
    }
    
    public void TakeDamage(int amount, ITarget target)
    {
        Rpc(nameof(TakeDamageRpc), amount);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void TakeDamageRpc(int amount)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(GetNode("EntityAnimatedSprite2D"), "scale", Vector2.One * 1.2f, 0.15f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);
		
        tween.TweenProperty(GetNode("EntityAnimatedSprite2D"), "scale", Vector2.One, 0.15f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Linear);
     
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        var tween2 = GetTree().CreateTween();
        tween2.TweenProperty(healthBar, "value", currentHealth, 0.1f).SetTrans(Tween.TransitionType.Sine);
        if(currentHealth <= 0)
        {
            RotationDegrees = 90;
            area.ProcessMode = ProcessModeEnum.Disabled;
            animSprite.Stop();
            healthBar.Visible = false;
        }
    }
}