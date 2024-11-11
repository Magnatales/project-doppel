using Godot;

namespace Code.Utils;

public class SpriteAnimator(Node2D user, NavigationAgent2D navAgent, AnimationPlayer animationPlayer, Sprite2D sprite2D)
{
    private bool _playingOneShot;
    
    public void Process()
    {
        if (_playingOneShot) return;
        if (navAgent.IsNavigationFinished())
        {
            animationPlayer.Play("Idle");
        }
        else
        {
            var direction = navAgent.GetNextPathPosition() - user.GlobalPosition;
            if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    sprite2D.FlipH = false;
                    animationPlayer.Play("Walk");
                }
                else
                {
                    sprite2D.FlipH = true;
                    animationPlayer.Play("Walk");
                }
            }
            else
            {
                animationPlayer.Play(direction.Y > 0 ? "Walk" : "Walk");
            }
        }
    }

    public async void PlayOneShot(string animation, bool canOverride = false)
    {
        if (_playingOneShot && !canOverride) return;
        _playingOneShot = true;
        animationPlayer.Play(animation);
        await animationPlayer.ToSignal(animationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _playingOneShot = false;
    }
}