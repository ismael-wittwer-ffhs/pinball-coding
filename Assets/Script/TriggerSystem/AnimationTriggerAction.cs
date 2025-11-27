// AnimationTriggerAction.cs : Description : Triggers animations using TweenSystem

using UnityEngine;

namespace TriggerSystem
{
    public class AnimationTriggerAction : TriggerAction
    {
        [Header("Animation Settings")]
        [SerializeField] private bool playOnEnter = true;
        [SerializeField] private bool playOnExit = false;
        [SerializeField] private bool playOnStay = false;
        [SerializeField] private AnimationType animationType = AnimationType.Rotation;
        [SerializeField] private float duration = 1f;

        [Header("Animation Values")]
        [SerializeField] private Vector3 targetRotation = Vector3.zero;
        [SerializeField] private Vector3 targetScale = Vector3.one;
        [SerializeField] private Vector3 targetPosition = Vector3.zero;

        [Header("Reset Settings")]
        [SerializeField] private bool resetOnExit = false;
        [SerializeField] private Vector3 resetRotation = Vector3.zero;
        [SerializeField] private Vector3 resetScale = Vector3.one;
        [SerializeField] private Vector3 resetPosition = Vector3.zero;

        private Vector3 originalRotation;
        private Vector3 originalScale;
        private Vector3 originalPosition;
        private bool hasOriginalValues = false;

        private void Awake()
        {
            if (!hasOriginalValues)
            {
                originalRotation = transform.localEulerAngles;
                originalScale = transform.localScale;
                originalPosition = transform.localPosition;
                hasOriginalValues = true;
            }
        }

        public override void Execute(TriggerContext context)
        {
            if (context is CollisionContext collisionContext)
            {
                bool shouldPlay = false;

                switch (collisionContext.Type)
                {
                    case TriggerType.Enter:
                        shouldPlay = playOnEnter;
                        break;
                    case TriggerType.Exit:
                        shouldPlay = playOnExit;
                        if (shouldPlay && resetOnExit)
                        {
                            ResetAnimation();
                            return;
                        }
                        break;
                    case TriggerType.Stay:
                        shouldPlay = playOnStay;
                        break;
                }

                if (shouldPlay)
                {
                    PlayAnimation();
                }
            }
        }

        private void PlayAnimation()
        {
            if ((animationType & AnimationType.Rotation) != 0)
            {
                TweenSystem.TweenRotation(transform, targetRotation, duration);
            }
            if ((animationType & AnimationType.Scale) != 0)
            {
                TweenSystem.TweenScale(transform, targetScale, duration);
            }
            if ((animationType & AnimationType.Translation) != 0)
            {
                TweenSystem.TweenTranslation(transform, targetPosition, duration);
            }
        }

        private void ResetAnimation()
        {
            Vector3 resetRot = resetRotation == Vector3.zero ? originalRotation : resetRotation;
            Vector3 resetScl = resetScale == Vector3.one ? originalScale : resetScale;
            Vector3 resetPos = resetPosition == Vector3.zero ? originalPosition : resetPosition;

            if ((animationType & AnimationType.Rotation) != 0)
            {
                TweenSystem.TweenRotation(transform, resetRot, duration);
            }
            if ((animationType & AnimationType.Scale) != 0)
            {
                TweenSystem.TweenScale(transform, resetScl, duration);
            }
            if ((animationType & AnimationType.Translation) != 0)
            {
                TweenSystem.TweenTranslation(transform, resetPos, duration);
            }
        }
    }
}

