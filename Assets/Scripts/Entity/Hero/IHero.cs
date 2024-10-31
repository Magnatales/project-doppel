using UnityEngine;

namespace Character
{
    public interface IHero
    {
        public string Id { get; }
        public Vector2 Position { get; }
    }
}