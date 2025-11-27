// ManagerGrpLeds.js : Description : Init a group of leds

using UnityEngine;

public class ManagerGrpLeds : MonoBehaviour
{
    #region --- Exposed Fields ---

    public GameObject[] obj_Led; // Connect Leds you want on this group

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> Init
        GetComponent<Manager_Led_Animation>().Init_Obj_Led_Animation(obj_Led); // Init Script Manager_Led_Animation.js
    }

    #endregion
}