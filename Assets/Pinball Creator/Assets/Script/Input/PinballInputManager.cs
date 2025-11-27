// PinballInputManager: Central manager for all pinball input using Unity's new Input System
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PinballInputManager : MonoBehaviour
{
    public static PinballInputManager Instance { get; private set; }

    [Header("Input Actions Asset")]
    [SerializeField] private InputActionAsset inputActions;

    // Pinball Action Map
    private InputActionMap pinballMap;

    // Actions - Public read-only access
    public InputAction FlipperLeft { get; private set; }
    public InputAction FlipperRight { get; private set; }
    public InputAction Plunger { get; private set; }
    public InputAction PauseGame { get; private set; }
    public InputAction ChangeCamera { get; private set; }
    public InputAction ShakeLeft { get; private set; }
    public InputAction ShakeRight { get; private set; }
    public InputAction ShakeUp { get; private set; }
    public InputAction NavigateHorizontal { get; private set; }

    // Touch state - updated each frame
    public bool LeftFlipperTouched { get; private set; }
    public bool RightFlipperTouched { get; private set; }
    public bool PlungerTouched { get; private set; }
    public bool LeftFlipperTouchBegan { get; private set; }
    public bool RightFlipperTouchBegan { get; private set; }
    public bool PlungerTouchBegan { get; private set; }

    [Header("Touch Settings")]
    [Tooltip("Vertical threshold (0-1) below which touch activates flippers")]
    [SerializeField] private float flipperTouchHeightThreshold = 0.6f;

    [Header("Debug")]
    [SerializeField] private bool debugTouchInput;

    private bool enhancedTouchEnabled;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Enable Enhanced Touch for mobile support
        if (!EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Enable();
            enhancedTouchEnabled = true;
        }

        InitializeActions();
    }

    private void InitializeActions()
    {
        if (inputActions == null)
        {
            Debug.LogError("PinballInputManager: InputActionAsset not assigned!");
            return;
        }

        // Get the Pinball action map
        pinballMap = inputActions.FindActionMap("Pinball");
        if (pinballMap == null)
        {
            Debug.LogError("PinballInputManager: 'Pinball' action map not found in InputActionAsset!");
            return;
        }

        // Get all actions
        FlipperLeft = pinballMap.FindAction("FlipperLeft");
        FlipperRight = pinballMap.FindAction("FlipperRight");
        Plunger = pinballMap.FindAction("Plunger");
        PauseGame = pinballMap.FindAction("PauseGame");
        ChangeCamera = pinballMap.FindAction("ChangeCamera");
        ShakeLeft = pinballMap.FindAction("ShakeLeft");
        ShakeRight = pinballMap.FindAction("ShakeRight");
        ShakeUp = pinballMap.FindAction("ShakeUp");
        NavigateHorizontal = pinballMap.FindAction("NavigateHorizontal");

        // Enable the action map
        pinballMap.Enable();
    }

    private void OnEnable()
    {
        pinballMap?.Enable();
    }

    private void OnDisable()
    {
        pinballMap?.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (enhancedTouchEnabled && EnhancedTouchSupport.enabled)
        {
            EnhancedTouchSupport.Disable();
        }
    }

    private void Update()
    {
        UpdateTouchState();
    }

    private void UpdateTouchState()
    {
        // Reset touch began states (they're only true for one frame)
        LeftFlipperTouchBegan = false;
        RightFlipperTouchBegan = false;
        PlungerTouchBegan = false;

        // Reset held states
        bool leftTouched = false;
        bool rightTouched = false;
        bool plungerTouched = false;

        foreach (var touch in Touch.activeTouches)
        {
            Vector2 screenPos = touch.screenPosition;
            float normalizedX = screenPos.x / Screen.width;
            float normalizedY = screenPos.y / Screen.height;

            // Check for plunger touch (right side, upper area or specific collider)
            // Plunger detection will be handled by raycast in SpringLauncher for precision
            // Here we just track if right side bottom is touched (where plunger usually is)
            bool isPlungerArea = normalizedX > 0.7f && normalizedY < 0.4f;

            // Check flipper areas (lower portion of screen, left/right halves)
            bool isFlipperArea = normalizedY < flipperTouchHeightThreshold;
            bool isLeftSide = normalizedX < 0.5f;
            bool isRightSide = normalizedX >= 0.5f;

            if (isPlungerArea)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    PlungerTouchBegan = true;
                }
                plungerTouched = true;
            }
            else if (isFlipperArea)
            {
                if (isLeftSide)
                {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        LeftFlipperTouchBegan = true;
                    }
                    leftTouched = true;
                }
                else if (isRightSide)
                {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        RightFlipperTouchBegan = true;
                    }
                    rightTouched = true;
                }
            }

            if (debugTouchInput && touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Debug.Log($"Touch at ({normalizedX:F2}, {normalizedY:F2}) - Left: {isLeftSide && isFlipperArea}, Right: {isRightSide && isFlipperArea}, Plunger: {isPlungerArea}");
            }
        }

        LeftFlipperTouched = leftTouched;
        RightFlipperTouched = rightTouched;
        PlungerTouched = plungerTouched;
    }

    #region Helper Methods for Common Input Checks

    /// <summary>
    /// Returns true if the left flipper input is currently held (keyboard, gamepad, or touch)
    /// </summary>
    public bool IsLeftFlipperHeld()
    {
        return (FlipperLeft != null && FlipperLeft.IsPressed()) || LeftFlipperTouched;
    }

    /// <summary>
    /// Returns true if the left flipper input was just pressed this frame
    /// </summary>
    public bool WasLeftFlipperPressed()
    {
        return (FlipperLeft != null && FlipperLeft.WasPressedThisFrame()) || LeftFlipperTouchBegan;
    }

    /// <summary>
    /// Returns true if the right flipper input is currently held (keyboard, gamepad, or touch)
    /// </summary>
    public bool IsRightFlipperHeld()
    {
        return (FlipperRight != null && FlipperRight.IsPressed()) || RightFlipperTouched;
    }

    /// <summary>
    /// Returns true if the right flipper input was just pressed this frame
    /// </summary>
    public bool WasRightFlipperPressed()
    {
        return (FlipperRight != null && FlipperRight.WasPressedThisFrame()) || RightFlipperTouchBegan;
    }

    /// <summary>
    /// Returns true if the plunger input is currently held
    /// </summary>
    public bool IsPlungerHeld()
    {
        return (Plunger != null && Plunger.IsPressed()) || PlungerTouched;
    }

    /// <summary>
    /// Returns true if the plunger input was just pressed this frame
    /// </summary>
    public bool WasPlungerPressed()
    {
        return (Plunger != null && Plunger.WasPressedThisFrame()) || PlungerTouchBegan;
    }

    /// <summary>
    /// Returns true if pause was pressed this frame
    /// </summary>
    public bool WasPausePressed()
    {
        return PauseGame != null && PauseGame.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns true if camera change was pressed this frame
    /// </summary>
    public bool WasCameraChangePressed()
    {
        return ChangeCamera != null && ChangeCamera.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns true if shake left was pressed this frame
    /// </summary>
    public bool WasShakeLeftPressed()
    {
        return ShakeLeft != null && ShakeLeft.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns true if shake right was pressed this frame
    /// </summary>
    public bool WasShakeRightPressed()
    {
        return ShakeRight != null && ShakeRight.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns true if shake up was pressed this frame
    /// </summary>
    public bool WasShakeUpPressed()
    {
        return ShakeUp != null && ShakeUp.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns the horizontal navigation axis value (-1 to 1)
    /// </summary>
    public float GetNavigateHorizontal()
    {
        return NavigateHorizontal != null ? NavigateHorizontal.ReadValue<float>() : 0f;
    }

    #endregion
}

