// ScoreTriggerAction.cs : Description : Awards score points when triggered

using UnityEngine;

namespace TriggerSystem
{
    public class ScoreTriggerAction : TriggerAction
    {
        [Header("Score Settings")]
        [Tooltip("Award score when the ball enters the trigger")]
        [SerializeField] private bool scoreOnEnter = true;
        [Tooltip("Award score when the ball exits the trigger")]
        [SerializeField] private bool scoreOnExit = false;
        [Tooltip("Award score continuously while the ball stays in the trigger")]
        [SerializeField] private bool scoreOnStay = false;
        
        [Header("Points")]
        [Tooltip("The number of points to award")]
        [SerializeField] private int points = 1000;
        
        [Header("Bonus Counter")]
        [Tooltip("Increment the bonus counter when awarding score")]
        [SerializeField] private bool incrementBonusCounter = true;

        private GameManager gameManager;

        private void Awake()
        {
            gameManager = GameManager.Instance;
        }

        public override void Execute(TriggerContext context)
        {
            if (gameManager == null) return;

            if (context is CollisionContext collisionContext)
            {
                bool shouldAwardScore = false;

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
                    AwardScore();
                }
            }
        }

        private void AwardScore()
        {
            if (incrementBonusCounter)
            {
                gameManager.F_Mode_BONUS_Counter();
            }
            
            gameManager.Add_Score(points);
        }
    }
}

