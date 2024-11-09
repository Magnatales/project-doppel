using Code.Entity;

public interface IEnemy
{
    public EnemyType Type { get; }
    public EnemyAreas EnemyAreas { get; }
    public EnemyStats Stats { get;  }
}