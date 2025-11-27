// Tilt_TriggerPreventBugTilt : Description : Use to enable or disable nudge MOde on Mobile Version

using UnityEngine;

public class Tilt_TriggerPreventBugTilt : MonoBehaviour
{
    #region --- Exposed Fields ---

    public bool b_Enable;

    #endregion

    #region --- Private Fields ---

    private GameObject obj_Game_Manager; // ManagerGame GameObject
    private ManagerGame gameManager; // access ManagerGame component from ManagerGame GameObject on the hierarchy

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> Function Start
        if (obj_Game_Manager == null) // Connect the Mission to the gameObject : "ManagerGame"
            obj_Game_Manager = GameObject.Find("ManagerGame");

        gameManager = obj_Game_Manager.GetComponent<ManagerGame>(); // Access ManagerGame gameComponent from obj_Game_Manager
    }

    #endregion

    #region --- Callbacks ---

    private void OnTriggerEnter(Collider other)
    {
        // --> Function OnTriggerEnter
        if (other.transform.tag == "Ball") // If it's a ball 
            gameManager.NudgeEnable(b_Enable); // Send Message to the obj_Game_Manager.  
    }

    #endregion
}