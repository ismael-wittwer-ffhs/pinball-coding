// Spring_Launcher: Manages the spring launcher (plunger) using Unity's new Input System
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Spring_Launcher : MonoBehaviour
{
    [Header("Manual or Auto launcher")]
    public bool Auto_Mode = false;

    private Rigidbody rb_Ball;
    private bool Activate = false;

    [Header("Force apply to the ball")]
    public float _Spring_Force = 7;
    private float tmp_Spring_Force;

    [Header("Sound fx")]
    public AudioClip Sfx_Pull;
    private bool Play_Once = false;
    public AudioClip Sfx_Kick;
    private AudioSource sound_;

    private float Spring_Max_Position = 0;
    private float Spring_Min_Position = -.6f;

    private Camera_Movement camera_Movement;

    [Header("Spring force to change cam view")]
    public float Cam_Change_Min = .4f;

    [Header("Time To wait before player could launch the ball")]
    public float Timer = .5f;
    private float tmp_Timer = 0;
    private bool b_Timer = false;

    private bool Ball_ExitThePlunger = false;

    private bool b_Debug = false;
    private bool b_touch = false;
    private bool b_Tilt = false;
    private GameObject obj_Mission_SkillShot;

    public bool springIsPulled = false;

    public BoxCollider _BoxCollider;

    public GameObject Mobile_Collider_zl;

    private PinballInputManager inputManager;

    void Start()
    {
        // Init Camera
        GameObject cameraBoard = GameObject.Find("Main Camera");
        if (!b_Debug && cameraBoard != null)
        {
            camera_Movement = cameraBoard.GetComponent<Camera_Movement>();
        }

        sound_ = GetComponent<AudioSource>();
        inputManager = PinballInputManager.Instance;

        if (inputManager == null)
        {
            Debug.LogWarning("Spring_Launcher: PinballInputManager not found. Make sure it exists in the scene.");
        }
    }

    void Update()
    {
        if (rb_Ball)
        {
            if (Mobile_Collider_zl && !Mobile_Collider_zl.activeInHierarchy)
            {
                Mobile_Collider_zl.SetActive(true);
            }

            // Update touch state
            UpdateTouchInput();

            // Auto Mode: press to launch
            if (Activate && Auto_Mode)
            {
                if (WasPlungerPressed() || b_touch)
                {
                    Ball_AddForceExplosion();
                    if (!b_Debug && camera_Movement) camera_Movement.PlayIdle();
                    F_Desactivate();
                }
            }

            // Manual Mode: hold to pull, release to launch
            if (Activate && !Auto_Mode)
            {
                if (IsPlungerHeld() || b_touch)
                {
                    // Pull the spring back
                    if (transform.localPosition.z >= Spring_Min_Position)
                    {
                        transform.localPosition = new Vector3(
                            transform.localPosition.x,
                            transform.localPosition.y,
                            Mathf.MoveTowards(transform.localPosition.z, Spring_Min_Position, 1 * Time.deltaTime)
                        );

                        if (!sound_.isPlaying && Sfx_Pull && !Play_Once)
                        {
                            sound_.clip = Sfx_Pull;
                            sound_.volume = .7f;
                            sound_.Play();
                            Play_Once = true;
                        }

                        springIsPulled = true;
                    }
                    tmp_Spring_Force = _Spring_Force * .5f * transform.localPosition.z * transform.localPosition.z;
                }
                else
                {
                    // Release - spring moves back and launches ball
                    if (Activate && transform.localPosition.z < Spring_Max_Position)
                    {
                        transform.localPosition = new Vector3(
                            transform.localPosition.x,
                            transform.localPosition.y,
                            Mathf.MoveTowards(transform.localPosition.z, Spring_Max_Position, 15 * Time.deltaTime)
                        );

                        if (transform.localPosition.z == 0 && springIsPulled)
                        {
                            if (Play_Once)
                            {
                                sound_.Stop();
                                Play_Once = false;
                            }
                            springIsPulled = false;
                            Ball_AddForceExplosion();
                        }
                    }
                }

                if (Ball_ExitThePlunger && transform.localPosition.z == 0)
                {
                    F_Desactivate();
                }
            }
            else if (!Activate && !Auto_Mode)
            {
                // Move spring launcher to init position
                if (transform.localPosition.z < Spring_Max_Position)
                {
                    transform.localPosition = new Vector3(
                        transform.localPosition.x,
                        transform.localPosition.y,
                        Mathf.MoveTowards(transform.localPosition.z, Spring_Max_Position, 15 * Time.deltaTime)
                    );

                    if (transform.localPosition.z == 0)
                    {
                        Play_Once = false;
                    }
                }
            }

            // Timer before player can launch ball after respawn
            if (b_Timer)
            {
                tmp_Timer = Mathf.MoveTowards(tmp_Timer, Timer, Time.deltaTime);
                if (Timer == tmp_Timer)
                {
                    tmp_Timer = 0;
                    b_Timer = false;
                    Activate = true;
                }
            }

            // Tilt mode - kick ball out
            if (b_Tilt && rb_Ball)
            {
                rb_Ball.AddForce(transform.forward * _Spring_Force, ForceMode.VelocityChange);
                if (!b_Debug && camera_Movement) camera_Movement.PlayIdle();
                if (!sound_.isPlaying && Sfx_Kick) sound_.PlayOneShot(Sfx_Kick);
                rb_Ball = null;
            }
        }
        else
        {
            if (Mobile_Collider_zl && Mobile_Collider_zl.activeInHierarchy)
            {
                Mobile_Collider_zl.SetActive(false);
            }
        }
    }

    private void UpdateTouchInput()
    {
        bool touchActive = false;

        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100))
                {
                    if (hit.transform.name == "Mobile_Collider" || hit.transform.name == "Mobile_Collider_zl")
                    {
                        touchActive = true;
                    }
                }
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                     touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                // Keep touch active while held
                if (b_touch) touchActive = true;
            }
        }

        // Also check PinballInputManager for generic plunger touch area
        if (inputManager != null && inputManager.PlungerTouched)
        {
            touchActive = true;
        }

        b_touch = touchActive;

        // Reset when no touches
        if (Touch.activeTouches.Count == 0)
        {
            b_touch = false;
        }
    }

    private bool IsPlungerHeld()
    {
        if (inputManager == null) return false;
        return inputManager.IsPlungerHeld();
    }

    private bool WasPlungerPressed()
    {
        if (inputManager == null) return false;
        return inputManager.WasPlungerPressed();
    }

    public void F_Activate()
    {
        b_Timer = true;
        Ball_ExitThePlunger = false;
        if (_BoxCollider) _BoxCollider.enabled = true;
    }

    public void F_Desactivate()
    {
        Activate = false;
        if (_BoxCollider) _BoxCollider.enabled = false;
    }

    public void F_Activate_After_Tilt()
    {
        b_Timer = true;
        Ball_ExitThePlunger = false;
        b_Tilt = false;
        if (_BoxCollider) _BoxCollider.enabled = true;
    }

    public void Tilt_Mode()
    {
        b_Tilt = true;
    }

    public void Ball_AddForceExplosion()
    {
        if (rb_Ball != null)
        {
            if (!Auto_Mode)
            {
                rb_Ball.AddForce(transform.forward * _Spring_Force * tmp_Spring_Force * tmp_Spring_Force, ForceMode.VelocityChange);
                if (Cam_Change_Min < tmp_Spring_Force)
                {
                    if (camera_Movement) camera_Movement.PlayIdle();
                    Ball_ExitThePlunger = true;
                    if (obj_Mission_SkillShot)
                    {
                        obj_Mission_SkillShot.SendMessage("Skillshot_Mission");
                    }
                }
            }
            else
            {
                if (!b_Debug && camera_Movement) camera_Movement.PlayIdle();
                rb_Ball.AddForce(transform.forward * _Spring_Force, ForceMode.VelocityChange);
            }

            if (!sound_.isPlaying && Sfx_Kick) sound_.PlayOneShot(Sfx_Kick);

            tmp_Spring_Force = 0;
            rb_Ball = null;
        }
    }

    public void BallOnPlunger(Rigidbody rb_obj)
    {
        if (!b_Debug && camera_Movement && rb_obj) camera_Movement.PlayPlunger();

        if (rb_obj) F_Activate();
        else F_Desactivate();

        rb_Ball = rb_obj;
    }

    public void F_Debug()
    {
        b_Debug = true;
    }

    public void Connect_Plunger_To_Skillshot_Mission(GameObject obj)
    {
        obj_Mission_SkillShot = obj;
    }

    /// <summary>
    /// Activate plunger from external script. Call via SendMessage("ActivatePlunger")
    /// </summary>
    public void ActivatePlunger()
    {
        if (!sound_.isPlaying && Sfx_Pull && !Play_Once)
        {
            sound_.clip = Sfx_Pull;
            sound_.volume = .7f;
            sound_.Play();
            Play_Once = true;
        }
        b_touch = true;
    }

    /// <summary>
    /// Deactivate plunger from external script. Call via SendMessage("DeactivatePlunger")
    /// </summary>
    public void DeactivatePlunger()
    {
        b_touch = false;
    }

    #region Legacy Compatibility (can be removed after full migration)

    /// <summary>
    /// Legacy method - no longer needed with new Input System.
    /// Kept for backward compatibility during migration.
    /// </summary>
    [System.Obsolete("No longer needed with new Input System. Will be removed in future version.")]
    public void F_InputGetButton()
    {
        // No-op: New input system handles this automatically
    }

    #endregion
}
