using Godot;

namespace Code.Entity;

public interface INpc
{
    public NavigationAgent2D NavAgent { get; }
    public AnimationPlayer AnimPlayer { get; }
    public Sprite2D Sprite { get; }
    public IMovement Movement { get; }
    void PlayOneShot(string anim);
    void _FromAnim(string trackKey);
}