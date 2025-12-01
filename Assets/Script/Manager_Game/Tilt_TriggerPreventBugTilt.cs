// Tilt_TriggerPreventBugTilt : Description : Use to enable or disable nudge MOde on Mobile Version

using UnityEngine;

public class Tilt_TriggerPreventBugTilt : MonoBehaviour
{
    #region --- Exposed Fields ---

    public bool b_Enable;

    #endregion

    #region --- Private Fields ---

    private GameManager gameManager; // access ManagerGame component from ManagerGame GameObject on the hierarchy

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> Function Start
        gameManager = GameManager.Instance; // Access ManagerGame gameComponent from singleton
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