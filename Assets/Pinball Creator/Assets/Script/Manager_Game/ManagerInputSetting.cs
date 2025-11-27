// ManagerInputSetting: Debug settings for pinball game
using UnityEngine;

public class ManagerInputSetting : MonoBehaviour
{
    [Header("Debug Shortcuts")]
    [SerializeField] private bool PinballDebugMode;

    public bool F_Debug_Game()
    {
        return PinballDebugMode;
    }
}
