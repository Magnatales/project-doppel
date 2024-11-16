using System.Threading.Tasks;
using Code.Entity;
using Code.Utils;
using Godot;
using projectdoppel.scripts.csharp;
using Steam;
using Steamworks;

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
    
    [Export] private MultiplayerSynchronizer _multiplayerSynchronizer;
    
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

    public override void _EnterTree()
    {
        _multiplayerSynchronizer.SetMultiplayerAuthority((int)long.Parse(Name));
    }

    public override void _Ready()
    {
        animSprite.Play("IdleDown");
        _entityAnimator = new EntityAnimator(this, navAgent, animSprite);
        currentHealth = maxHealth;
        healthBar.Value = currentHealth;
        mouseTargetArea.AreaEntered += OnTargetAreaEntered;
        mouseTargetArea.AreaExited += OnTargetAreaExited;
        
        if(_multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {
            _label.Text = $"Player {Name}";
            _camera.MakeCurrent();
        }
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
         _entityAnimator.Update();
        if (_multiplayerSynchronizer.GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
        {
            return;
        }
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
            for (int i = 0; i < 1; i++)
            {

                var enemy2 = new Enemy();
                GetParent().AddChild(enemy2);
                var enemy = packedScene.Instantiate<Enemy>();
                enemy.GlobalPosition = new Vector2(GetGlobalMousePosition().X + i, GetGlobalMousePosition().Y + i);
                GetParent().AddChild(enemy);
            }
        }
        
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
    
    private void DoPhysicsPointQuery()
    {
        var mousePos = GetGlobalMousePosition();
        var query = new PhysicsPointQueryParameters2D();
        query.Position = mousePos;
        query.CollideWithAreas = true;
        query.CollisionMask = 2;
        var spaceState = GetWorld2D().DirectSpaceState;
        var result = spaceState.IntersectPoint(query, 10);

        if (result.Count <= 0) return;
        
        foreach (var value in result)
        {
            if (!value.TryGetValue("collider", out var collider)) continue;
            if (collider.Obj is not Area2D area2D) continue;
            if (area2D.GetParent() is not Enemy damageable) continue;
            
            //GD.Print("Hit enemy " + damageable.Name);
            damageable.TakeDamage(40, this);
        }
    }

    public void Log(string text)
    {
        GD.Print(text);
    }
    
    public void TakeDamage(int amount, ITarget from)
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