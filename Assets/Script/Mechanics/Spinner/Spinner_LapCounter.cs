// Spinner_LapCounter : Description : Count the spinner rotation. This object is used by mission 

using UnityEngine;

public class Spinner_LapCounter : MonoBehaviour
{
    #region --- Exposed Fields ---

    //private int tmp_CheckLap;

    [Header("Spinner rotation sound")]
    public AudioClip Sfx_Rotation; // Sound when ball hit Spinner

    public GameObject[] Parent_Manager; // Connect on the inspector the missions that use this object


    [Header("Infos to missions")]
    public int index; // choose a number. Used to create script mission.

    [Header("Points when the spinner rotate")]
    public int Points = 1000; // Points you win when the object is hitting 

    public string functionToCall = "Counter"; // Call a function when OnCollisionEnter -> true;

    #endregion

    #region --- Private Fields ---

    private AudioSource sound_; // AudioSource component
    private GameManager gameManager; // ManagerGame Component from singleton

    #endregion

    #region --- Static and Constant Fields ---

    private static int Lap;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> init
        gameManager = GameManager.Instance; // Access ManagerGame from singleton
        sound_ = GetComponent<AudioSource>(); // Access AudioSource Component
    }

    #endregion

    #region --- Methods ---

    public int index_info()
    {
        // return the index of the object. Use by missions
        return index;
    }

    #endregion

    #region --- Callbacks ---

    private void OnTriggerExit(Collider other)
    {
        // --> When ball enter on the trigger
        Lap++;
        //tmp_CheckLap = Lap;
        for (var j = 0; j < Parent_Manager.Length; j++) Parent_Manager[j].SendMessage(functionToCall, index); // Call Parents Mission script
        if (Sfx_Rotation) sound_.PlayOneShot(Sfx_Rotation); // Play soiund if needed
        if (gameManager) gameManager.F_Mode_BONUS_Counter(); // Send Message to the gameManager(ManagerGame.js) Add 1 to BONUS_Global_Hit_Counter
        if (gameManager)
        {
            // Use ball position or transform position for trigger-based mechanics
            var position = other.transform != null ? other.transform.position : transform.position;
            gameManager.Add_Score(Points, position); // Send Message to the gameManager(ManagerGame.js) Add Points to Add_Score
        }
    }

    #endregion
}