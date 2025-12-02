// ScoreTriggerAction.cs : Description : Awards score points when triggered

using UnityEngine;

namespace TriggerSystem
{
    public class ScoreTriggerAction : TriggerAction
    {
        #region --- Private Fields ---

        [Header("Bonus Counter")]
        [Tooltip("Increment the bonus counter when awarding score")]
        [SerializeField]
        private bool incrementBonusCounter = true;

        [Header("Score Settings")]
        [Tooltip("Award score when the ball enters the trigger")]
        [SerializeField]
        private bool scoreOnEnter = true;

        [Tooltip("Award score when the ball exits the trigger")]
        [SerializeField]
        private bool scoreOnExit;

        [Tooltip("Award score continuously while the ball stays in the trigger")]
        [SerializeField]
        private bool scoreOnStay;

        private GameManager gameManager;

        [Header("Points")]
        [Tooltip("The number of points to award")]
        [SerializeField]
        private int points = 1000;

        private UiManager uiManager;

        #endregion

        #region --- Unity Methods ---

        private void Start()
        {
            gameManager = GameManager.Instance;
            uiManager = UiManager.Instance;
        }

        #endregion

        #region --- Methods ---

        public override void Execute(TriggerContext context)
        {
            if (gameManager == null) return;

            if (context is CollisionContext collisionContext)
            {
                var shouldAwardScore = false;

                switch (collisionContext.Type)
                {
                    case TriggerType.Enter:
                        shouldAwardScore = scoreOnEnter;
                        break;
                    case TriggerType.Exit:
                        shouldAwardScore = scoreOnExit;
                        break;
                    case TriggerType.Stay:
                        shouldAwardScore = scoreOnStay;
                        break;
                }

                if (shouldAwardScore)
                {
                    var position = GetScoreTextPosition(collisionContext);
                    AwardScore(position);
                }
            }
        }

        private void AwardScore(Vector3 position)
        {
            if (incrementBonusCounter) gameManager.F_Mode_BONUS_Counter();

            gameManager.Add_Score(points);

            // Show score text popup
            if (uiManager != null) uiManager.ShowScoreText(points, position);
        }

        private Vector3 GetScoreTextPosition(CollisionContext context)
        {
            // Try to get position from collision contact point if available
            if (context.CollisionData != null && context.CollisionData.contactCount > 0) return context.CollisionData.contacts[0].point;

            // Fall back to triggering object position
            if (context.TriggeringObject != null) return context.TriggeringObject.transform.position;

            // Last resort: use this transform's position
            return transform.position;
        }

        #endregion
    }
}