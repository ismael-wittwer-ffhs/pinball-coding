// SlowMotionTriggerAction.cs : Description : Slows time on enter, resets on exit

using UnityEngine;

namespace TriggerSystem
{
    public class SlowMotionTriggerAction : TriggerAction
    {
        #region --- Exposed Fields ---

        [SerializeField]
        public bool ResetOnExit = true;

        [SerializeField]
        public bool SlowOnEnter = true;

        [Header("Slow Motion Settings")]
        [SerializeField]
        public float SlowMotionTimeScale = 0.5f;

        #endregion

        #region --- Private Fields ---

        private float _originalTimeScale;

        #endregion

        #region --- Unity Methods ---

        private void Awake()
        {
            _originalTimeScale = Time.timeScale;
        }

        private void OnDestroy()
        {
            Time.timeScale = _originalTimeScale;
        }

        #endregion

        #region --- Methods ---

        public override void Execute(TriggerContext context)
        {
            if (context is CollisionContext collisionContext)
                switch (collisionContext.Type)
                {
                    case TriggerType.Enter:
                        if (SlowOnEnter) ApplySlowMotion();
                        break;
                    case TriggerType.Exit:
                        if (ResetOnExit) ResetTimeScale();
                        break;
                }
        }


        private void ApplySlowMotion()
        {
            Time.timeScale = SlowMotionTimeScale;
        }

        private void ResetTimeScale()
        {
            Time.timeScale = _originalTimeScale;
        }

        #endregion
    }
}