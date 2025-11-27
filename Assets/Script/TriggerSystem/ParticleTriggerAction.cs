// ParticleTriggerAction.cs : Description : Plays particle effects when triggered

using UnityEngine;

namespace TriggerSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleTriggerAction : TriggerAction
    {
        [Header("Particle Settings")]
        [SerializeField] private bool playOnEnter = true;
        [SerializeField] private bool playOnExit = false;
        [SerializeField] private bool playOnStay = false;
        [SerializeField] private float emissionDuration = 1f;

        private ParticleSystem particleSystem;
        private ParticleSystem.EmissionModule emissionModule;
        private float timer;
        private bool isEmitting;

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                emissionModule = particleSystem.emission;
            }
        }

        public override void Execute(TriggerContext context)
        {
            if (particleSystem == null) return;

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
                        break;
                    case TriggerType.Stay:
                        shouldPlay = playOnStay;
                        break;
                }

                if (shouldPlay)
                {
                    StartParticleEmission();
                }
            }
        }

        private void StartParticleEmission()
        {
            if (!isEmitting)
            {
                particleSystem.Play();
                emissionModule.enabled = true;
                timer = 0f;
                isEmitting = true;
            }
        }

        private void Update()
        {
            if (isEmitting)
            {
                timer += Time.deltaTime;
                if (timer >= emissionDuration)
                {
                    emissionModule.enabled = false;
                    isEmitting = false;
                    timer = 0f;
                }
            }
        }
    }
}

