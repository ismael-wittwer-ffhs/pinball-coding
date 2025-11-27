// Flippers: Manages flipper movements using Unity's new Input System
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class Flippers : MonoBehaviour
{
    [Header("-> Choose between left or right flipper (only one)")]
    public bool b_Flipper_Left = false;
    public bool b_Flipper_Right = false;

    public HingeJoint hinge;

    [Header("-> Audio when key input is pressed")]
    public AudioClip Sfx_Flipper;
    private AudioSource source;

    [Header("-> Know if the flipper is activated or not")]
    public bool Activate = false;

    private PinballInputManager inputManager;

    private bool b_touch = false;
    private bool b_Pause = false;
    private bool b_Debug = false;

    private bool wasPressed = false;  // Track if input was pressed (for sound)
    private bool b_PullPlunger = false;  // If pulling plunger, can't use right flippers

    void Awake()
    {
        // Ignore collision between layers
        Physics.IgnoreLayerCollision(8, 9, true);   // Board / Paddle
        Physics.IgnoreLayerCollision(10, 9, true);
        Physics.IgnoreLayerCollision(11, 9, true);
        Physics.IgnoreLayerCollision(12, 9, true);
        Physics.IgnoreLayerCollision(13, 9, true);
        Physics.IgnoreLayerCollision(0, 9, true);   // Default / Paddle

        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        inputManager = PinballInputManager.Instance;

        if (inputManager == null)
        {
            Debug.LogWarning("Flippers: PinballInputManager not found. Make sure it exists in the scene.");
        }
    }

    public void F_Activate()
    {
        if (!b_Debug) Activate = true;
    }

    public void F_Desactivate()
    {
        if (!b_Debug) Activate = false;
    }

    public void F_Pause_Start()
    {
        b_Pause = true;
        F_Desactivate();
    }

    public void F_Pause_Stop()
    {
        b_Pause = false;
        F_Activate();
    }

    public void F_Debug()
    {
        // b_Debug = true; Activate = true;
    }

    public void PreventBugWhenOrientationChange()
    {
        b_touch = false;
    }

    void Update()
    {
        if (Activate)
        {
            // Prevent flipper getting stuck when returning to init position
            JointSpring hingeSpring = hinge.spring;
            hingeSpring.spring = Random.Range(1.99f, 2.01f);
            hinge.spring = hingeSpring;
            var motor = hinge.motor;

            // Update touch state
            UpdateTouchInput();

            // Check if input is held (keyboard/gamepad or touch)
            bool inputHeld = IsInputHeld();

            // Play sound on press
            if (inputHeld && !wasPressed)
            {
                PlayFlipperSound();
                wasPressed = true;
            }
            else if (!inputHeld)
            {
                wasPressed = false;
            }

            // Move flipper based on input
            if (inputHeld)
            {
                hinge.motor = motor;
                hinge.useMotor = true;
            }
            else
            {
                motor = hinge.motor;
                hinge.motor = motor;
                hinge.useMotor = false;
            }
        }
        else if (!b_Pause)
        {
            // When table is tilted - flipper deactivated but should return to init position
            var motor = hinge.motor;
            hinge.motor = motor;
            hinge.useMotor = false;
        }
    }

    private void UpdateTouchInput()
    {
        // Check for plunger touch to prevent right flipper activation
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100))
                {
                    if (hit.transform.name == "Mobile_Collider_zl" || hit.transform.name == "Mobile_Collider")
                    {
                        b_PullPlunger = true;
                        return;
                    }
                }
                b_PullPlunger = false;
            }
        }

        // Update touch state from PinballInputManager
        if (inputManager != null)
        {
            if (b_Flipper_Left)
            {
                b_touch = inputManager.LeftFlipperTouched;
            }
            else if (b_Flipper_Right && !b_PullPlunger)
            {
                b_touch = inputManager.RightFlipperTouched;
            }
            else if (b_Flipper_Right && b_PullPlunger)
            {
                b_touch = false;
            }
        }

        // Reset plunger flag when no touches
        if (Touch.activeTouches.Count == 0)
        {
            b_PullPlunger = false;
        }
    }

    private bool IsInputHeld()
    {
        if (inputManager == null) return b_touch;

        if (b_Flipper_Left)
        {
            return inputManager.IsLeftFlipperHeld() || b_touch;
        }
        else if (b_Flipper_Right)
        {
            // Don't activate right flipper if pulling plunger
            if (b_PullPlunger) return false;
            return inputManager.IsRightFlipperHeld() || b_touch;
        }

        return false;
    }

    private void PlayFlipperSound()
    {
        if (Sfx_Flipper != null && source != null)
        {
            source.volume = 1;
            source.PlayOneShot(Sfx_Flipper);
        }
    }

    /// <summary>
    /// Activate flipper from external script. Call via SendMessage("ActivateFlipper")
    /// </summary>
    public void ActivateFlipper()
    {
        PlayFlipperSound();
        b_touch = true;
    }

    /// <summary>
    /// Deactivate flipper from external script. Call via SendMessage("DeactivateFlipper")
    /// </summary>
    public void DeactivateFlipper()
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
