// switch : Description : Manage Switch table mechanics

using UnityEngine;

public class switchMech : MonoBehaviour
{
    #region --- Exposed Fields ---

    public AudioClip Sfx_Hit;
    public GameObject[] Parent_Manager;
    public int index;

    public int Points = 1000; // Points you win when the object is hitting 
    public string functionToCall = "Counter"; // Call a function when OnCollisionEnter -> true;

    #endregion

    #region --- Private Fields ---

    private AudioSource sound_;
    private GameManager gameManager;

    #endregion

    #region --- Unity Methods ---

    private void Awake()
    {
        Physics.IgnoreLayerCollision(8, 12, true);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        sound_ = GetComponent<AudioSource>();
    }

    #endregion

    #region --- Callbacks ---

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ball")
        {
            for (var j = 0; j < Parent_Manager.Length; j++) Parent_Manager[j].SendMessage(functionToCall, index); // Call Parents Mission script

            if (!sound_.isPlaying && Sfx_Hit) sound_.PlayOneShot(Sfx_Hit); // Play a sound

            if (gameManager) gameManager.F_Mode_BONUS_Counter(); // Add Points to bonus counter
            if (gameManager)
            {
                // Get position from collision contact point
                var position = collision.contactCount > 0 ? collision.contacts[0].point : transform.position;
                gameManager.Add_Score(Points, position); // Add point to score
            }
        }
    }

    #endregion
}