using Godot;

namespace Code.Utils
{
    public partial class HitDamage : Label
    {
        private float _lifetime = 1.0f;
        private float _elapsedTime = 0.0f;
        private Vector2 _initialPosition;
        
        private static Theme _cachedTheme;
        
        public HitDamage(int number, Vector2 position)
        {
            if (_cachedTheme == null)
            {
                _cachedTheme = ResourceLoader.Load<Theme>("uid://dp7shd8hg32r");
            }

            Theme = _cachedTheme;
            Scale = Vector2.One * 0.5f;
            GlobalPosition = position;
            _initialPosition = position;
            ZIndex = 10;
            Text = number.ToString();
            Modulate = new Color(1, 0.25f, 0.25f);
            Scale = new Vector2(1.5f, 1.5f);
        }

        public override void _Process(double delta)
        {
            _elapsedTime += (float)delta;
            
            var moveY = -30 * Mathf.Pow(0.5f, _elapsedTime) + -20 * (float)delta;
            GlobalPosition += new Vector2(0, moveY) * (float)delta;
            
            Modulate = new Color(1, Mathf.Lerp(0.5f, 0, _elapsedTime / _lifetime), Mathf.Lerp(0.5f, 0, _elapsedTime / _lifetime), 1 - (_elapsedTime / _lifetime));
            
            Scale = Vector2.One * Mathf.Lerp(1.5f, 1.0f, _elapsedTime / _lifetime);
            
            if (_elapsedTime >= _lifetime)
            {
                QueueFree();
            }
        }
    }
}