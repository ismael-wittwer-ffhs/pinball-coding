// TweenSystem.cs : Description : Simple tween system for animations

using UnityEngine;
using System.Collections;

namespace TriggerSystem
{
    [System.Flags]
    public enum AnimationType
    {
        Rotation = 1 << 0,
        Scale = 1 << 1,
        Translation = 1 << 2
    }

    public class TweenSystem : MonoBehaviour
    {
        public static void TweenRotation(Transform target, Vector3 targetRotation, float duration, System.Action onComplete = null)
        {
            if (target == null) return;
            var tween = target.gameObject.GetComponent<TweenSystem>();
            if (tween == null)
            {
                tween = target.gameObject.AddComponent<TweenSystem>();
            }
            tween.StartCoroutine(tween.TweenRotationCoroutine(target, targetRotation, duration, onComplete));
        }

        public static void TweenScale(Transform target, Vector3 targetScale, float duration, System.Action onComplete = null)
        {
            if (target == null) return;
            var tween = target.gameObject.GetComponent<TweenSystem>();
            if (tween == null)
            {
                tween = target.gameObject.AddComponent<TweenSystem>();
            }
            tween.StartCoroutine(tween.TweenScaleCoroutine(target, targetScale, duration, onComplete));
        }

        public static void TweenTranslation(Transform target, Vector3 targetPosition, float duration, System.Action onComplete = null)
        {
            if (target == null) return;
            var tween = target.gameObject.GetComponent<TweenSystem>();
            if (tween == null)
            {
                tween = target.gameObject.AddComponent<TweenSystem>();
            }
            tween.StartCoroutine(tween.TweenTranslationCoroutine(target, targetPosition, duration, onComplete));
        }

        private IEnumerator TweenRotationCoroutine(Transform target, Vector3 targetRotation, float duration, System.Action onComplete)
        {
            Vector3 startRotation = target.localEulerAngles;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                target.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
                yield return null;
            }

            target.localEulerAngles = targetRotation;
            onComplete?.Invoke();
        }

        private IEnumerator TweenScaleCoroutine(Transform target, Vector3 targetScale, float duration, System.Action onComplete)
        {
            Vector3 startScale = target.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                target.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            target.localScale = targetScale;
            onComplete?.Invoke();
        }

        private IEnumerator TweenTranslationCoroutine(Transform target, Vector3 targetPosition, float duration, System.Action onComplete)
        {
            Vector3 startPosition = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                target.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            target.localPosition = targetPosition;
            onComplete?.Invoke();
        }
    }
}

