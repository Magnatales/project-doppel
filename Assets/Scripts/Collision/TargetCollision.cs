using System;
using Entity;
using UnityEngine;
using UnityEngine.AI;

namespace Collision
{
    public class TargetCollision : MonoBehaviour, ITargetCollision
    {
        public ITarget Self { get; private set; }
        public Action<ITargetCollision> OnTargetCollision { get; set; }

        private void Awake()
        {
            var agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        public void BindTo(ITarget self)
        {
            Self = self;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ITargetCollision targetCollision))
            {
                OnTargetCollision?.Invoke(targetCollision);
            }
        }
    }
}