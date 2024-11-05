using Godot;

namespace Code.Utils;

public class EntityAnimator (Node2D parent, NavigationAgent2D navAgent, AnimatedSprite2D animSprite)
{
    public void Update()
    {
        if (navAgent.IsNavigationFinished())
        {
            animSprite.Play("IdleDown");
        }
        else
        {
            var direction = navAgent.GetNextPathPosition() - parent.GlobalPosition;
            if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    animSprite.FlipH = false;
                    animSprite.Play("WalkHorizontal");
                }
                else
                {
                    animSprite.FlipH = true;
                    animSprite.Play("WalkHorizontal");
                }
            }
            else
            {
                if (direction.Y > 0)
                {
                    animSprite.Play("WalkDown");
                }
                else
                {
                    animSprite.Play("WalkUp");
                }
            }
        }
    }
}