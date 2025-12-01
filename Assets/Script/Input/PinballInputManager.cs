// PinballInputManager: Central manager for all pinball input using Unity's new Input System
using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
    }

    #region Helper Methods for Common Input Checks

    /// <summary>
    /// Returns true if the left flipper input is currently held (keyboard or gamepad)
    /// </summary>
    public bool IsLeftFlipperHeld()
    {
        return FlipperLeft != null && FlipperLeft.IsPressed();
    }

    /// <summary>
    /// Returns true if the left flipper input was just pressed this frame
    /// </summary>
    public bool WasLeftFlipperPressed()
    {
        return FlipperLeft != null && FlipperLeft.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns true if the right flipper input is currently held (keyboard or gamepad)
    /// </summary>
    public bool IsRightFlipperHeld()
    {
        return FlipperRight != null && FlipperRight.IsPressed();
    }

    /// <summary>
    /// Returns true if the right flipper input was just pressed this frame
    /// </summary>
    public bool WasRightFlipperPressed()
    {
        return FlipperRight != null && FlipperRight.WasPressedThisFrame();
    }

    /// <summary>
    /// Returns true if the plunger input is currently held
    /// </summary>
    public bool IsPlungerHeld()
    {
        return Plunger != null && Plunger.IsPressed();
    }

    /// <summary>
    /// Returns true if the plunger input was just pressed this frame
    /// </summary>
    public bool WasPlungerPressed()
    {
        return Plunger != null && Plunger.WasPressedThisFrame();
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

