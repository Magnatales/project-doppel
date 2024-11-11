using Godot;
using System;

public partial class Effect : Node
{
	[Export] private AnimationPlayer _animPlayer;
	[Export] private Sprite2D _sprite;
	
	private static PackedScene my_Scene = (PackedScene)ResourceLoader.Load("res://scripts/csharp/vfx/Effect.tscn");

	public Effect()
	{
		
	}

	public void Play(string animation, Vector2 position)
	{
		if (!_animPlayer.HasAnimation(animation))
		{
			GD.PushError($"Animation [{animation}] not found in [{_animPlayer.Name}]");
			return;
		}
		_sprite.GlobalPosition = position;
		_animPlayer.Play(animation);
		_animPlayer.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished(StringName animname)
	{
		_animPlayer.AnimationFinished -= OnAnimationFinished;
		QueueFree();
	}
}
