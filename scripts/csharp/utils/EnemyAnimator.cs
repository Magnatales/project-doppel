using System;
using System.Threading.Tasks;
using Godot;

namespace Code.Utils;

public class EnemyAnimator(Node2D parent, NavigationAgent2D navAgent, AnimatedSprite2D animSprite)
{
    private bool _playingOneShot;
    public void Update()
    {
        if (_playingOneShot) return;
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
                    animSprite.Play("WalkDown");
                }
                else
                {
                    animSprite.FlipH = true;
                    animSprite.Play("WalkDown");
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
                    animSprite.Play("WalkDown");
                }
            }
        }
    }

    public async Task PlayAnimation(string animation)
    {
        _playingOneShot = true;
        animSprite.Play(animation);
        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        _playingOneShot = false;
    }
}