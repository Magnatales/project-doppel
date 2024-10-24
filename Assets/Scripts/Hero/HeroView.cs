using SimpleSpriteAnimator;
using UnityEngine;

namespace Character
{
    public class HeroView : MonoBehaviour
    {
        [field:SerializeField] public float speed;
        [field: SerializeField] public SpriteAnimator spriteAnimator;
    }
}
