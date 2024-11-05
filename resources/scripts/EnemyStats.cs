using Godot;
using System;

public partial class EnemyStats : Resource
{
	[Export] public int Health {get ; set;}
	[Export] public int Damage {get ; set;}
	[Export] public int Speed {get ; set;}
	
	[Export] public int X {get ; set;}
	[Export] public int Y {get ; set;}

	public EnemyStats()
	{
		
	}
}
