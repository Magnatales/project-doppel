using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private Sprite pressed;
    [SerializeField] private Sprite released;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        UnityEngine.Cursor.visible = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        transform.position = pos;
        
        if (Input.GetMouseButtonDown(0))
        {
            _spriteRenderer.sprite = pressed;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _spriteRenderer.sprite = released;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
        }
    }
}
