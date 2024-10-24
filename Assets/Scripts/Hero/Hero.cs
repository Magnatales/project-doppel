using PrimeTween;
using UnityEngine;

namespace Character
{
    public class Hero
    {
        private readonly HeroView _heroView;
        private readonly Camera _camera;
        private Tween _movementTween;
        private Vector3 _lastPosition;

        public Hero(HeroView heroView, Camera camera)
        {
            _heroView = heroView;
            _camera = camera;
        }

        public void Update()
        {
            _heroView.spriteAnimator.Play(_heroView.transform.position != _lastPosition ? "walk" : "idle");
            _heroView.spriteAnimator.FlipX(_heroView.transform.position.x > _lastPosition.x);

            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = Input.mousePosition;
                var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
                worldPosition.z = _heroView.transform.position.z;
                _movementTween.Stop();
                _movementTween = Tween.LocalPositionAtSpeed(_heroView.transform, worldPosition, _heroView.speed, Ease.Linear);
            }
            
            _lastPosition = _heroView.transform.position;
        }
    }
}