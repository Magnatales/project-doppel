using Code.Entity;
using Code.Utils;
using Godot;

public partial class Player : Node2D, ITarget
{
    [Export] public float Speed;
    [Export] public NavigationAgent2D navAgent;
    [Export] public Vector2 Target;
    [Export] private AnimatedSprite2D animSprite;
    
    public Vector2 Pos => GlobalPosition;
    private float oldX;
    private float oldY;
    private Vector2 _position;
    private EntityAnimator _entityAnimator;
    private Vector2 _velocity = Vector2.Zero;

    public override void _Ready()
    {
        animSprite.Play("IdleDown");
        _entityAnimator = new EntityAnimator(this, navAgent, animSprite);
    }
    
    public void Log(string text)
    {
        GD.Print(text);
    }
    
    public void TakeDamage(int amount)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(GetNode("EntityAnimatedSprite2D"), "scale", Vector2.One * 0.75f, 0.2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);
		
        tween.TweenProperty(GetNode("EntityAnimatedSprite2D"), "scale", Vector2.One, 0.2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Linear);
     
        GD.Print("PLAYER ATTACKED FOR: " + amount);
    }


    public override void _Process(double delta)
    {
        _entityAnimator.Update();
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
            Target = GetGlobalMousePosition();
            navAgent.SetTargetPosition(Target);
        }

        if (Input.IsActionJustPressed("RightClick"))
        {
            Target = GetGlobalMousePosition();
            navAgent.SetTargetPosition(Target);
            GlobalPosition = Target;
            GlobalPosition = GlobalPosition.Floor();
        }
    }
}