using Godot;

namespace Code.Utils;

public class NavAgentMovement(Node2D parent, NavigationAgent2D navAgent, Sprite2D sprite2D, float speed) : IMovement
{
    private Vector2 _velocity = Vector2.Zero;
    public void Move(float delta)
    {
        if (navAgent.IsNavigationFinished())
        {
            parent.GlobalPosition = parent.GlobalPosition.Floor();
            return;
        }

        var targetPos = navAgent.GetTargetPosition();
        sprite2D.FlipH = targetPos.X < parent.GlobalPosition.X;
        var direction = navAgent.GetNextPathPosition() - parent.GlobalPosition;
        direction = direction.Normalized();
        _velocity = _velocity.Lerp(direction * speed, speed * delta);
        parent.GlobalPosition += _velocity * delta;
    }
}