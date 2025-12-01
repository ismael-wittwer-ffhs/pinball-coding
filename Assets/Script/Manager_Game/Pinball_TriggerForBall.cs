// Pinball_TriggerForBall : Description : Detect when a ball is lost : Use by Out_Hole_TriggerDestroyBall gameObject on the hierarchy

using UnityEngine;

public class Pinball_TriggerForBall : MonoBehaviour
{
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
            gameManager.gamePlay(other.gameObject); // Send Message to the obj_Game_Manager.  
    }

    #endregion
}