// AudioTriggerAction.cs : Description : Plays audio when triggered

using UnityEngine;

namespace TriggerSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTriggerAction : TriggerAction
    {
        [Header("Audio Settings")]
        [SerializeField] private bool playOnEnter = true;
        [SerializeField] private bool playOnExit = false;
        [SerializeField] private bool playOnStay = false;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool useOneShot = true;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null && audioClip != null)
            {
                audioSource.clip = audioClip;
            }
        }

        public override void Execute(TriggerContext context)
        {
            if (audioSource == null) return;

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
                    PlayAudio();
                }
            }
        }

        private void PlayAudio()
        {
            if (audioClip == null) return;

            if (useOneShot)
            {
                audioSource.PlayOneShot(audioClip);
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
        }
    }
}

