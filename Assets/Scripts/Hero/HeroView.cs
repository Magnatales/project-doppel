using SimpleSpriteAnimator;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class HeroView : MonoBehaviour
    {
        [field: SerializeField] public SpriteAnimator spriteAnimator;
        [field: SerializeField] public NavMeshAgent navMeshAgent;
    }
}
