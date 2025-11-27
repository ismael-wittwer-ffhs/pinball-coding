// ManagerInputSetting: Legacy input settings wrapper
// Note: This class is now simplified as input is handled by PinballInputManager
using UnityEngine;

public class ManagerInputSetting : MonoBehaviour
{
    [Header("Debug Shortcuts")]
    [SerializeField] private bool PinballDebugMode = false;

    // Note: The Start() method that called F_InputGetButton on all components
    // has been removed as input is now handled by PinballInputManager

    public bool F_Debug_Game()
    {
        return PinballDebugMode;
    }

    #region Legacy Properties (kept for backward compatibility, will be removed)

    // These properties are kept temporarily for any code that still references them
    // They are no longer used by the new Input System

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_flipper_Left() => "leftShift";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_flipper_Right() => "rightShift";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_Plunger() => "enter";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_Pause_Game() => "e";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_Change_Camera() => "c";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_Shake_Left() => "r";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_Shake_Right() => "t";

    [System.Obsolete("Input is now handled by PinballInputManager")]
    public string F_Shake_Up() => "f";

    #endregion
}
