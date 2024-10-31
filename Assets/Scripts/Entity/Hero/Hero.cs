using System;
using Cysharp.Threading.Tasks;
using Entity;
using UnityEngine;

namespace Character
{
    [Serializable]
    public class Hero : ITarget
    {
        public void InstantKill()
        {
            throw new NotImplementedException();
        }

        public void TakeDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public void Heal(int heal)
        {
            throw new NotImplementedException();
        }

        public void AddDebuff(string debuffId)
        {
            throw new NotImplementedException();
        }

        private readonly IHeroView _heroView;
        private readonly Camera _camera;
        private Vector3 _lastPosition;
        private Vector3 _targetPosition;
        private bool _mouseWasPressed;
        private const float MOVEMENT_THRESHOLD = 0.01f;

        public Hero(IHeroView heroView, Camera camera)
        {
            _heroView = heroView;
            _camera = camera;
            _heroView.NavAgent.updateRotation = false;
            _heroView.NavAgent.updateUpAxis = false;
            //Id = $"Hero {Guid.NewGuid().ToString()}";
        }

        public void Update()
        {
            var movementDelta = (_heroView.Transform.position - _lastPosition).sqrMagnitude;
            var walking = movementDelta > MOVEMENT_THRESHOLD * MOVEMENT_THRESHOLD;

            if (walking)
            {
                _heroView.SpriteAnim.Play(_heroView.Transform.position.y > _lastPosition.y ? "walknorth" : "walk");
            }
            else
            {
                _heroView.SpriteAnim.Play("idle");
            }
            _heroView.SpriteAnim.FlipX(_heroView.Transform.position.x > _lastPosition.x);

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            if (horizontal != 0 || vertical != 0)
            {
                var direction = new Vector3(horizontal, vertical, 0).normalized;
                _targetPosition = _heroView.Transform.position + direction;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _mouseWasPressed = true;
                var mousePosition = Input.mousePosition;
                var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0;
                _targetPosition = worldPosition;
            }
            else if (Input.GetMouseButton(0))
            {
                _mouseWasPressed = true;
                var mousePosition = Input.mousePosition;
                var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0;
                _targetPosition = worldPosition;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dash().Forget();
            }

            _heroView.NavAgent.SetDestination(_targetPosition);
            if(_mouseWasPressed)
                _heroView.ShowLine();
            
            _lastPosition = _heroView.Transform.position;
            Position = _heroView.Transform.position;
            _mouseWasPressed = false;
        }

        private async UniTaskVoid Dash()
        {
            _heroView.NavAgent.speed = 400;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            _heroView.NavAgent.speed = 50;
        }

        public Vector3 Position { get; private set; }
    }
}