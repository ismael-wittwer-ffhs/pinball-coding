// SlowMotionTriggerAction.cs : Description : Slows time on enter, resets on exit

using UnityEngine;

namespace TriggerSystem
{
    public class SlowMotionTriggerAction : TriggerAction
    {
        #region --- Private Fields ---

        [SerializeField]
        private bool resetOnExit = true;

        [SerializeField]
        private bool slowOnEnter = true;

        private float originalTimeScale;

        [Header("Slow Motion Settings")]
        [SerializeField]
        private float slowMotionTimeScale = 0.5f;

        #endregion

        #region --- Unity Methods ---

        private void Awake()
        {
            originalTimeScale = Time.timeScale;
        }

        private void OnDestroy()
        {
            Time.timeScale = originalTimeScale;
        }

        #endregion

        #region --- Methods ---

        public override void Execute(TriggerContext context)
        {
            if (context is CollisionContext collisionContext)
                switch (collisionContext.Type)
                {
                    case TriggerType.Enter:
                        if (slowOnEnter) ApplySlowMotion();
                        break;
                    case TriggerType.Exit:
                        if (resetOnExit) ResetTimeScale();
                        break;
                }
        }

        private void ApplySlowMotion()
        {
            Time.timeScale = slowMotionTimeScale;
        }

        private void ResetTimeScale()
        {
            Time.timeScale = originalTimeScale;
        }

        #endregion
    }
}