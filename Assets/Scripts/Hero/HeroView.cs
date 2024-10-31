using System.Collections;
using System.Linq;
using SimpleSpriteAnimator;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class HeroView : MonoBehaviour, IHeroView
    {
        [field: SerializeField] public SpriteAnimator SpriteAnim { get; private set; }
        [field: SerializeField] public NavMeshAgent NavAgent { get; private set; }
        public Transform Transform => transform;
        
        [SerializeField] private SpriteRenderer destination;
        [SerializeField] private LineRenderer pathRenderer;
        
        private Coroutine _fadeCoroutine;
        private Gradient _originalGradient;
        
        private void Start()
        {
            destination.enabled = false;
            destination.transform.SetParent(null);
            _originalGradient = pathRenderer.colorGradient;
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
