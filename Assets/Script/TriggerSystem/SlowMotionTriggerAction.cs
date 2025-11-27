// SlowMotionTriggerAction.cs : Description : Slows time on enter, resets on exit

using UnityEngine;

namespace TriggerSystem
{
    public class SlowMotionTriggerAction : TriggerAction
    {
        [Header("Slow Motion Settings")]
        [SerializeField] private float slowMotionTimeScale = 0.5f;
        [SerializeField] private bool slowOnEnter = true;
        [SerializeField] private bool resetOnExit = true;

        private float originalTimeScale;
        private bool hasOriginalTimeScale = false;

        private void Awake()
        {
            if (!hasOriginalTimeScale)
            {
                originalTimeScale = Time.timeScale;
                hasOriginalTimeScale = true;
            }
        }

        public override void Execute(TriggerContext context)
        {
            if (context is CollisionContext collisionContext)
            {
                switch (collisionContext.Type)
                {
                    case TriggerType.Enter:
                        if (slowOnEnter)
                        {
                            ApplySlowMotion();
                        }
                        break;
                    case TriggerType.Exit:
                        if (resetOnExit)
                        {
                            ResetTimeScale();
                        }
                        break;
                }
            }
        }

        private void ApplySlowMotion()
        {
            if (!hasOriginalTimeScale)
            {
                originalTimeScale = Time.timeScale;
                hasOriginalTimeScale = true;
            }
            Time.timeScale = slowMotionTimeScale;
        }

        private void ResetTimeScale()
        {
            if (hasOriginalTimeScale)
            {
                Time.timeScale = originalTimeScale;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        private void OnDestroy()
        {
            // Ensure time scale is reset when component is destroyed
            if (hasOriginalTimeScale)
            {
                Time.timeScale = originalTimeScale;
            }
        }
    }
}

