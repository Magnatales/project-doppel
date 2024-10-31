using Character;
using UnityEngine;

public class GameReferences : MonoBehaviour, IGameReferences
{
    [field: SerializeField]
    public Camera GameCamera { get; private set; }
    
    [field: SerializeField]
    public HeroView HeroView { get; private set; }

    [field: SerializeField] 
    public Cursor Cursor { get; private set; }
}
