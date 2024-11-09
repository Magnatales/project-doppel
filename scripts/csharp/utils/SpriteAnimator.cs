using Godot;

namespace Code.Utils;

public class SpriteAnimator(Node2D parent, NavigationAgent2D navAgent, AnimationPlayer animationPlayer, Sprite2D sprite2D)
{
    private bool _playingOneShot;
    public void Update()
    {
        if (_playingOneShot) return;
        if (navAgent.IsNavigationFinished())
        {
            animationPlayer.Play("Idle");
        }
        else
        {
            var direction = navAgent.GetNextPathPosition() - parent.GlobalPosition;
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
                if (direction.Y > 0)
                {
                    animationPlayer.Play("Walk");
                }
                else
                {
                    animationPlayer.Play("Walk");
                }
            }
        }
    }

    public async void PlayOneShot(string animation)
    {
        _playingOneShot = true;
        animationPlayer.Play(animation);
        await animationPlayer.ToSignal(animationPlayer, AnimationMixer.SignalName.AnimationFinished);
        _playingOneShot = false;
    }
}