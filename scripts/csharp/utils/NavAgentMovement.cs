using Code.Entity;
using Godot;

namespace Code.Utils;

public class NavAgentMovement(Node2D parent, NavigationAgent2D navAgent, Sprite2D sprite2D, INpc npc, float speed) : IMovement
{
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
        npc.Velocity = npc.Velocity.Lerp(direction * speed, speed * delta);
        var newPosition = parent.GlobalPosition + (npc.Velocity * delta);
        
        parent.GlobalPosition = parent.GlobalPosition.Lerp(newPosition, 1f);
    }
}