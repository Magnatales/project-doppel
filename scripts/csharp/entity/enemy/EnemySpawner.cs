using Godot;
using System;

public partial class EnemySpawner : MultiplayerSpawner
{
	public static EnemySpawner Instance { get; private set; }
	[Export] private PackedScene _enemyScene;
	[Export] private Node2D _enemyParent;

	public override void _Ready()
	{
		Instance = this;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SpawnEnemy(Vector2 position)
	{
		var enemy = _enemyScene.Instantiate<Enemy>();
		GetTree().Root.AddChild(enemy);
		enemy.GlobalPosition = position;
	}

}
