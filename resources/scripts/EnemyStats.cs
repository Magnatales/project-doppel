using Godot;

[GlobalClass]
public partial class EnemyStats : Resource
{
	[Export] public int Health {get ; set;}
	[Export] public int Damage {get ; set;}
	[Export] public int Speed {get ; set;}

	public EnemyStats()
	{
		
	}
}
