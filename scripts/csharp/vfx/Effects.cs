using Godot;

public partial class Effects : Node
{
	[Export] private PackedScene _effectScene;
	public static Effects Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public void Play(string animation, Vector2 position)
	{
		var effect = _effectScene.Instantiate<Effect>();
		GetParent().AddChild(effect);
		effect.Play(animation, position);
	}
}
