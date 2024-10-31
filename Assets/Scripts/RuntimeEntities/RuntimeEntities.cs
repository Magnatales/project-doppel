using System.Collections.Generic;
using System.ComponentModel;
using Character;
using Code.RuntimeEntities;
using UnityEngine;

[CreateAssetMenu(fileName = "RuntimeEntities", menuName = "RuntimeEntities/RuntimeEntities")]
public class RuntimeEntities : ScriptableObject, IRuntimeEntities
{
    [ReadOnly(true)]
    [field:SerializeReference]
    public List<IHero> heroes { get; private set; }
    
    [ReadOnly(true)]
    [field:SerializeReference]
    public List<IEnemy> enemies { get; private set; }

    public void RegisterHero(IHero hero)
    {
        heroes.Add(hero);
    }

    public void RegisterEnemy(IEnemy enemy)
    {
        enemies.Add(enemy);
    }

    public void UnregisterHero(IHero hero)
    {
        heroes.Remove(hero);
    }

    public void UnregisterEnemy(IEnemy enemy)
    {
        enemies.Remove(enemy);
    }
    
    public List<IHero> GetHeroesInRange(Vector2 position, float range)
    {
        var result = heroes.FindAll(h => Vector2.Distance(h.Position, position) <= range);
        return result;
    }

    public List<IEnemy> GetEnemiesInRange(Vector2 position, float range)
    {
        var result = enemies.FindAll(e => Vector2.Distance(e.Position, position) <= range);
        return result;
    }

    public void Clear()
    {
        heroes.Clear();
        enemies.Clear();
    }
}
