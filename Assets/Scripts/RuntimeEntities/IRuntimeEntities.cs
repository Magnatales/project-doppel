using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Code.RuntimeEntities
{
    public interface IRuntimeEntities
    {
        public List<IHero> heroes { get; }
        public List<IEnemy> enemies { get;}
        
        void RegisterHero(IHero hero);
        void RegisterEnemy(IEnemy enemy);
        void UnregisterHero(IHero hero);
        void UnregisterEnemy(IEnemy enemy);
        
        List<IHero> GetHeroesInRange(Vector2 position, float range);
        List<IEnemy> GetEnemiesInRange(Vector2 position, float range);
    }
}