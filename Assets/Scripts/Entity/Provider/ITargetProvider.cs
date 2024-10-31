using System.Collections.Generic;
using Character;

namespace Entity.Provider
{
    public interface ITargetProvider
    {
        public Dictionary<string, IHero> Heroes { get; }
        
        void RegisterHero(IHero hero);
        
        void UnregisterHero(IHero hero);
        
        IHero GetHero(string heroId);
        
        List<IHero> GetHeroes();
    }
}