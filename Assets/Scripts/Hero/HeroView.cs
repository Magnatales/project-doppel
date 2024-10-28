using System;
using System.Linq;
using SimpleSpriteAnimator;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class HeroView : MonoBehaviour
    {
        [field: SerializeField] public SpriteAnimator spriteAnimator;
        [field: SerializeField] public NavMeshAgent navMeshAgent;
        [SerializeField] private SpriteRenderer destination;
        [SerializeField] private LineRenderer pathRenderer;
        
        private void Start()
        {
            destination.enabled = false;
            destination.transform.SetParent(null);
        }

        public void ShowLine()
        {
            destination.enabled = true;
            Vector3 agentPosition = navMeshAgent.transform.position;
            
            var pointsNotWalked = navMeshAgent.path.corners
                .SkipWhile(corner => Vector3.Distance(agentPosition, corner) < navMeshAgent.stoppingDistance)
                .ToArray();
            
            pathRenderer.positionCount = pointsNotWalked.Length;
            pathRenderer.SetPositions(pointsNotWalked);
            destination.transform.position = navMeshAgent.path.corners.Last();
        }
        
        public void HideLine()
        {
            destination.enabled = false;
            pathRenderer.positionCount = 0;
        }
    }
}
