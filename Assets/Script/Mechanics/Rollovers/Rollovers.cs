// Rollovers.js : Description : Manage the rollover mechanics

using UnityEngine;

public class Rollovers : MonoBehaviour
{
    #region --- Exposed Fields ---

    [Header("Sound fx")]
    public AudioClip Sfx_Hit; // Sound when ball hit Rollover

    [Header("Toy connected to the Rollover")] // Connect a GameObject or paticule system with the script Toys.js attached
    public GameObject Toy;

    public GameObject[] Parent_Manager; // Connect on the inspector the missions that use this object
    public int AnimNum;

    [Header("Infos to missions")]
    public int index; // choose a number. Used to create script mission.

    [Header("Points when ball go through rollover")]
    public int Points = 1000; // Points you win when the object is hitting 

    public string functionToCall = "Counter"; // Call a function when OnCollisionEnter -> true;

    #endregion

    #region --- Private Fields ---

    private AudioSource sound_;
    private GameManager gameManager; // ManagerGame Component from singleton
    private Toys toy;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> Init
        gameManager = GameManager.Instance; // Access ManagerGame from singleton
        sound_ = GetComponent<AudioSource>(); // Access AudioSource Component
        if (Toy) toy = Toy.GetComponent<Toys>(); // access Toys component if needed
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

    private void OnTriggerEnter(Collider other)
    {
        // --> When the ball enter the trigger
        if (other.tag == "Ball")
        {
            for (var j = 0; j < Parent_Manager.Length; j++) Parent_Manager[j].SendMessage(functionToCall, index); // Call Parents Mission script

            if (!sound_.isPlaying && Sfx_Hit) sound_.PlayOneShot(Sfx_Hit); // Play a sound if needed
            if (gameManager != null)
            {
                gameManager.F_Mode_BONUS_Counter(); // Send Message to the gameManager(ManagerGame.js) Add 1 to BONUS_Global_Hit_Counter
                gameManager.Add_Score(Points); // Send Message to the gameManager(ManagerGame.js) Add Points to Add_Score
            }

            if (Toy) toy.PlayAnimationNumber(AnimNum); // Play toy animation if needed
        }
    }

    #endregion
}