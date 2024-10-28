using SimpleSpriteAnimator;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public interface IHeroView
    {
        public SpriteAnimator SpriteAnim { get; }
        public NavMeshAgent NavAgent { get; }
        public Transform Transform { get; }
        public void ShowLine();
    }
}