using System.Collections.Generic;
using Character;

namespace Entity.Provider
{
    public class TargetProvider : ITargetProvider
    {
        public Dictionary<string, IHero> Heroes { get; }
        public void RegisterHero(IHero hero)
        {
            Heroes.Add(hero.Id, hero);
        }

        public void UnregisterHero(IHero hero)
        {
            Heroes.Remove(hero.Id);
        }

        public IHero GetHero(string heroId)
        {
            return Heroes[heroId];
        }

        public List<IHero> GetHeroes()
        {
            return new List<IHero>(Heroes.Values);
        }
    }
}