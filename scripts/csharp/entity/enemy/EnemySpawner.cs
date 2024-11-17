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
	
	public void SpawnEnemy(Vector2 position)
	{
		var enemy = _enemyScene.Instantiate<Enemy>();
		_enemyParent.AddChild(enemy, true);
		enemy.GlobalPosition = position;
	}
}
