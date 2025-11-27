// Toys. : Description : This script is used to play animation with animator or use Particle system.

using UnityEngine;

public class Toys : MonoBehaviour
{
    #region --- Exposed Fields ---

    [Header("Play a sound")]
    public AudioClip sfx; // if a sound is connected a sound is played when animation or Particle system start

    [Header("Play ParticleSystem or Animation when ball is OnTriggerEnter")]
    public bool _trigger; // If you want to start Particle system or animation when ball Is	On Trigger Enter		

    [Header("Object with animation")]
    public bool Mode_Animator = true; // True if the object use animation

    [Header("Particle System")]
    public bool Mode_Particle; // True if the object use Particle system

    [Header("Emission duration")]
    public float ParticleTime; // Duration of Particle emission

    [Header("Animation Number")]
    public int _trigger_animation_number; // Choose animation number if _trigger = true

    #endregion

    #region --- Private Fields ---

    private Animator anim; // Access Animator Component
    private AudioSource _source; // Access audio component

    private bool Mode_Pause; // Use to pause animation or Particle system
    private bool ParticleEnable; // use to manage Particle Pause Mode		
    private BoxCollider Box; // Access Component Box Collider
    private ParticleSystem.EmissionModule _Particle; // access Component
    private float timer; // use with variable ParticleTime
    private int MoveHash = Animator.StringToHash("b_Move"); // access parameter from Animator
    private int MoveStateHash_1 = Animator.StringToHash("Base.Move_1"); // access state from Animator
    private int MoveStateHash_2 = Animator.StringToHash("Base.Move_2"); // access state from Animator
    private int MoveStateHash_3 = Animator.StringToHash("Base.Move_3"); // access state from Animator
    private int MoveStateHash_4 = Animator.StringToHash("Base.Move_4"); // access state from Animator
    private int MoveStateHash_5 = Animator.StringToHash("Base.Move_5"); // access state from Animator
    private int NumberHash = Animator.StringToHash("number"); // access parameter from Animator
    private ParticleSystem _particleSystem; // access Component

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> Start
        _source = GetComponent<AudioSource>(); // Access Audiosource Component
        if (Mode_Animator) anim = GetComponent<Animator>(); // Access ANimator Component if needed
        if (Mode_Particle)
        {
            // Access Particle system component if needed
            _particleSystem = gameObject.GetComponent<ParticleSystem>();
            _Particle = _particleSystem.emission;
        }

        if (_trigger)
        {
            Box = GetComponent<BoxCollider>();
            Box.enabled = true;
        }
    }

    private void Update()
    {
        // --> Update
        if (!Mode_Pause)
        {
            // Mode Pause = false
            if (Mode_Animator)
            {
                // 
                var stateInfo = anim.GetCurrentAnimatorStateInfo(0); // know what animation is active
                if (stateInfo.fullPathHash    == MoveStateHash_1
                    || stateInfo.fullPathHash == MoveStateHash_2
                    || stateInfo.fullPathHash == MoveStateHash_3
                    || stateInfo.fullPathHash == MoveStateHash_4
                    || stateInfo.fullPathHash == MoveStateHash_5)
                {
                    anim.SetBool(MoveHash, false); // init parameter
                    anim.SetInteger(NumberHash, 0); // init parameter
                }
            }

            if (Mode_Particle && ParticleEnable)
            {
                // Play Particle system
                timer = Mathf.MoveTowards(timer, ParticleTime, Time.deltaTime);

                if (timer == ParticleTime)
                {
                    ParticleEnable = false;
                    _Particle.enabled = false;
                    timer = 0;
                }
            }
        }
    }

    #endregion

    #region --- Methods ---

    public void Pause_Anim()
    {
        // --> Pause Animation
        if (Mode_Animator)
        {
            if (anim.speed == 0) Stop_Pause_Anim();
            else Start_Pause_Anim();
            if (_source.isPlaying) _source.Pause();
            else _source.UnPause();
        }

        if (Mode_Particle) PauseParicule();
    }

    public void PauseParicule()
    {
        // --> Pause Particle system
        if (Mode_Pause)
        {
            Mode_Pause = false;
            if (_particleSystem.isPaused) _particleSystem.Play();
            _source.UnPause();
        }
        else
        {
            Mode_Pause = true;
            if (_particleSystem.isPlaying) _particleSystem.Pause();
            if (_source.isPlaying) _source.Pause();
        }
    }

    public void PlayAnimation()
    {
        // Play an animation. This function is not used
        if (!anim.GetBool(MoveHash)) anim.SetBool(MoveHash, true); // if anim is not playing.
    }

    public void PlayAnimationNumber(int num)
    {
        // Play animation with number of the animation.
        if (Mode_Particle)
        {
            timer = 0;
            ParticleEnable = true;
            _particleSystem.Play();
            _Particle.enabled = true;
        }
        else
        {
            switch (num)
            {
                case 0: // Play Anim state : Move_1
                    if (!anim.GetBool(MoveHash))
                    {
                        anim.SetBool(MoveHash, true); // if anim is not playing.
                        anim.SetInteger(NumberHash, 1);
                    }

                    break;
                case 1: // Play Anim state : Move_2
                    if (!anim.GetBool(MoveHash))
                    {
                        anim.SetBool(MoveHash, true); // if anim is not playing.
                        anim.SetInteger(NumberHash, 2);
                    }

                    break;
                case 2: // Play Anim state : Move_3
                    if (!anim.GetBool(MoveHash))
                    {
                        anim.SetBool(MoveHash, true); // if anim is not playing.
                        anim.SetInteger(NumberHash, 3);
                    }

                    break;
                case 3: // Play Anim state : Move_4
                    if (!anim.GetBool(MoveHash))
                    {
                        anim.SetBool(MoveHash, true); // if anim is not playing.
                        anim.SetInteger(NumberHash, 4);
                    }

                    break;
                case 4: // Play Anim state : Move_5
                    if (!anim.GetBool(MoveHash))
                    {
                        anim.SetBool(MoveHash, true); // if anim is not playing.
                        anim.SetInteger(NumberHash, 5);
                    }

                    break;
            }
        }

        if (sfx)
        {
            // Play a sound
            _source.clip = sfx;
            _source.Play();
        }
    }


    public void PlayIdle()
    {
        // Play an animation.
    }

    public void Start_Pause_Anim()
    {
        // --> Start Pause Animation
        if (Mode_Animator) anim.speed = 0;
    }


    public void Stop_Pause_Anim()
    {
        // --> Stop Pause animation
        if (Mode_Animator) anim.speed = 1;
    }

    #endregion

    #region --- Callbacks ---

    private void OnTriggerEnter(Collider other)
    {
        // --> OnTriggerEnter : Use If Box Collider = true. 
        if (other.transform.tag == "Ball")
        {
            if (Mode_Animator) PlayAnimationNumber(_trigger_animation_number); // Play animation
            if (Mode_Particle)
            {
                // patricule emission
                timer = 0;
                ParticleEnable = true;
                _particleSystem.Play();
                _Particle.enabled = true;
            }

            if (sfx)
            {
                // Play a sound
                _source.clip = sfx;
                _source.Play();
            }
        }
    }

    #endregion
}