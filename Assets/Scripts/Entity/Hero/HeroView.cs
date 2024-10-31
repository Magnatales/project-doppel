using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleSpriteAnimator;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class HeroView : MonoBehaviour, IHeroView
    {
        [field: SerializeField] public SpriteAnimator SpriteAnim { get; private set; }
        [field: SerializeField] public NavMeshAgent NavAgent { get; private set; }
        public List<Hero> Hero;
        public Transform Transform => transform;
        
        [SerializeField] private SpriteRenderer destination;
        [SerializeField] private LineRenderer pathRenderer;
        public BehaviorGraphAgent Agent;
        public BehaviorGraph Graph;
        private Coroutine _fadeCoroutine;
        private Gradient _originalGradient;
        
        private void Start()
        {
            destination.enabled = false;
            destination.transform.SetParent(null);
            _originalGradient = pathRenderer.colorGradient;
            Hero.Add(new Hero(this, null));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Find a random direction within a circle
                float searchRadius = 55f; 
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * searchRadius;
                Vector3 targetPosition = new Vector3(randomDirection.x, randomDirection.y, 0) + NavAgent.transform.position;
                targetPosition.z = 0;
                // Sample the NavMesh to find a valid point in 2D space
                
                if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
                {
                    // Set the agent's destination to the random point
                    NavAgent.SetDestination(hit.position);
                }
                else
                {
                    Debug.Log("No valid point found within search radius.");
                }
                
                
            }
        }

        public void ShowLine()
        {
            destination.enabled = true;
            Vector3 agentPosition = NavAgent.transform.position;
            
            var pointsNotWalked = NavAgent.path.corners
                .SkipWhile(corner => Vector3.Distance(agentPosition, corner) < NavAgent.stoppingDistance)
                .ToArray();
            
            pathRenderer.positionCount = pointsNotWalked.Length;
            pathRenderer.SetPositions(pointsNotWalked);
            destination.transform.position = NavAgent.path.corners.Last();
            if (_fadeCoroutine != null)
            {
                destination.color = new Color(destination.color.r, destination.color.g, destination.color.b, 1);
                pathRenderer.colorGradient = _originalGradient;
                StopCoroutine(_fadeCoroutine);
            }
            //Calculate the duration based on the navmesh agent path
            var duration = NavAgent.path.corners
                .Select((corner, index) => index == 0 ? 0 : Vector3.Distance(NavAgent.path.corners[index - 1], corner) / NavAgent.speed)
                .Sum();
            _fadeCoroutine = StartCoroutine(FadeOut(duration));
        }
        
        private IEnumerator FadeOut(float duration)
        {
            var currentFadeTime = 0f;
            while (currentFadeTime < duration)
            {
                currentFadeTime += Time.deltaTime;
                var alpha = Mathf.Lerp(1, 0, currentFadeTime / duration);

                destination.color = new Color(destination.color.r, destination.color.g, destination.color.b, alpha);
                var newGradient = new Gradient();
                newGradient.SetKeys(
                    _originalGradient.colorKeys,
                    new GradientAlphaKey[]
                    {
                        new GradientAlphaKey(alpha, 0f),
                        new GradientAlphaKey(alpha, 1f)
                    }
                );

                pathRenderer.colorGradient = newGradient;
                yield return null;
            }
        }
        
        public void HideLine()
        {
            destination.enabled = false;
            pathRenderer.positionCount = 0;
        }
        
    }
}
