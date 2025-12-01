// Bumper : Description : Bumper Manager

using UnityEngine;

public class Bumper_js : MonoBehaviour
{
    #region --- Exposed Fields ---

    [Header("Bumper sound")]
    public AudioClip Sfx_Hit; // Sound when ball hit bumper

    [Header("Force applied to the ball")]
    public float bumperForce = .6f; // modify the force applied to the ball

    [Header("LED connected to the bumper")]
    public GameObject obj_Led; // Usefull if you want a led blinked when the slingshot is hitting

    [Header("Toy connected to the bumper")] // Connect a GameObject or paticule system with the script Toys.js attached
    public GameObject Toy;

    public GameObject[] Parent_Manager; // Connect on the inspector the missions that use this object
    public int AnimNum;

    [Header("Infos to missions")]
    public int index; // choose a number. Used to create script mission.

    [Header("Points when the bumper is hit")]
    public int Points = 1000; // Points you win when the object is hitting 

    public int[] AnimNums;
    public string functionToCall = "Counter"; // Call a function when OnCollisionEnter -> true;

    [Header("Connected More than One Animation to the bumper")] // Connect a GameObject or paticule system with the script Toys.js attached
    public Toys[] Toys;

    #endregion

    #region --- Private Fields ---

    private AudioSource sound_; // AudioSource Component
    private ChangeSpriteRenderer Led_Renderer; // ChangeSpriteRenderer Component from obj_Led
    private GameManager gameManager; // ManagerGame Component from singleton
    private Toys toy;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> function Start
        gameManager = GameManager.Instance; // Access ManagerGame from singleton
        sound_ = GetComponent<AudioSource>(); // Access AudioSource Component

        if (obj_Led) Led_Renderer = obj_Led.GetComponent<ChangeSpriteRenderer>(); // If obj_Led = true; Access ChangeSpriteRenderer Component
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

    private void OnCollisionEnter(Collision collision)
    {
        // --> Detect collision when bumper enter on collision with other objects
        var tmpContact = collision.contacts;
        foreach (var contact in tmpContact)
        {
            // if there is a collision : 
            var rb = contact.otherCollider.GetComponent<Rigidbody>(); // Access rigidbody Component
            var t = collision.relativeVelocity.magnitude; // save the collision.relativeVelocity.magnitude value
            if (!rb.isKinematic) rb.linearVelocity = new Vector3(rb.linearVelocity.x * .25f, rb.linearVelocity.y * .25f, rb.linearVelocity.z * .25f); // reduce the velocity at the impact. Better feeling with the slingshot
            rb.AddForce(-1 * contact.normal * bumperForce, ForceMode.VelocityChange); // Add Force
        }

        if (Sfx_Hit) sound_.PlayOneShot(Sfx_Hit); // Play a sound

        for (var j = 0; j < Parent_Manager.Length; j++) Parent_Manager[j].SendMessage(functionToCall, index); // Call Parents Mission script

        if (gameManager != null)
        {
            gameManager.F_Mode_BONUS_Counter(); // Send Message to the gameManager(ManagerGame.js) Add 1 to BONUS_Global_Hit_Counter
            gameManager.Add_Score(Points); // Send Message to the gameManager(ManagerGame.js) Add Points to Add_Score
        }

        if (obj_Led) Led_Renderer.Led_On_With_Timer(.2f); // If Obj_Led. Switch On the Led during .2 seconds
        if (Toy) toy.PlayAnimationNumber(AnimNum); // Play toy animation if needed


        if (Toys.Length > 0) // Play more than One animation
            for (var i = 0; i < Toys.Length; i++)
                Toys[i].PlayAnimationNumber(AnimNums[i]);
    }

    #endregion
}