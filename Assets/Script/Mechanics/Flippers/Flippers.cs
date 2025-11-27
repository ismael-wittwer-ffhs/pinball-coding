// Flippers: Manages flipper movements using Unity's new Input System

using UnityEngine;
using Random = UnityEngine.Random;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Flippers : MonoBehaviour
{
    #region --- Exposed Fields ---

    [Header("-> Audio when key input is pressed")]
    public AudioClip Sfx_Flipper;

    [Header("-> Know if the flipper is activated or not")]
    public bool Activate;

    [Header("-> Choose between left or right flipper (only one)")]
    public bool b_Flipper_Left;

    public bool b_Flipper_Right;

    public HingeJoint hinge;

    #endregion

    #region --- Private Fields ---

    private AudioSource source;
    private bool b_Debug = false;
    private bool b_Pause;
    private bool b_PullPlunger; // If pulling plunger, can't use right flippers

    private bool b_touch;

    private bool wasPressed; // Track if input was pressed (for sound)

    private PinballInputManager inputManager;

    #endregion

    #region --- Unity Methods ---

    private void Awake()
    {
        // Ignore collision between layers
        Physics.IgnoreLayerCollision(8, 9, true); // Board / Paddle
        Physics.IgnoreLayerCollision(10, 9, true);
        Physics.IgnoreLayerCollision(11, 9, true);
        Physics.IgnoreLayerCollision(12, 9, true);
        Physics.IgnoreLayerCollision(13, 9, true);
        Physics.IgnoreLayerCollision(0, 9, true); // Default / Paddle

        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        hinge = GetComponent<HingeJoint>();
        inputManager = PinballInputManager.Instance;

        if (inputManager == null) Debug.LogWarning("Flippers: PinballInputManager not found. Make sure it exists in the scene.");
    }

    private void Update()
    {
        if (Activate)
        {
            // Prevent flipper getting stuck when returning to init position
            var hingeSpring = hinge.spring;
            hingeSpring.spring = Random.Range(1.99f, 2.01f);
            hinge.spring = hingeSpring;
            var motor = hinge.motor;

            // Update touch state
            UpdateTouchInput();

            // Check if input is held (keyboard/gamepad or touch)
            var inputHeld = IsInputHeld();

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

    #endregion

    #region --- Methods ---

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

    public void F_Activate()
    {
        if (!b_Debug) Activate = true;
    }

    public void F_Debug()
    {
        // b_Debug = true; Activate = true;
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

    public void PreventBugWhenOrientationChange()
    {
        b_touch = false;
    }

    private bool IsInputHeld()
    {
        if (inputManager == null) return b_touch;

        if (b_Flipper_Left) return inputManager.IsLeftFlipperHeld() || b_touch;

        if (b_Flipper_Right)
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

    private void UpdateTouchInput()
    {
        // Check for plunger touch to prevent right flipper activation
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                if (Physics.Raycast(ray, out var hit, 100))
                    if (hit.transform.name == "Mobile_Collider_zl" || hit.transform.name == "Mobile_Collider")
                    {
                        b_PullPlunger = true;
                        return;
                    }

                b_PullPlunger = false;
            }
        }

        // Update touch state from PinballInputManager
        if (inputManager != null)
        {
            if (b_Flipper_Left)
                b_touch = inputManager.LeftFlipperTouched;
            else if (b_Flipper_Right && !b_PullPlunger)
                b_touch = inputManager.RightFlipperTouched;
            else if (b_Flipper_Right && b_PullPlunger) b_touch = false;
        }

        // Reset plunger flag when no touches
        if (Touch.activeTouches.Count == 0) b_PullPlunger = false;
    }

    #endregion
}