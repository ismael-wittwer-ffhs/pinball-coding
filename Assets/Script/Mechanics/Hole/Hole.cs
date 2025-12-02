// Hole : Description : Manage table mechanic. When a ball enter on collision with this object this ball is respawn to an other position.
// 1) It is possible to choose the time before the ball go to the respawn point.
// 2) It is possible to choose the time before adding a force to the ball.
// 3) It is possible to choose the direction of the ball when a force is added. The direction is chosen by the direction of ''Spawn'' gameObject (vecteur Forward ''z'').
// 4) If you want to create kickback it is possible. See the prefab Kickback on Project folder.

using System.Collections;
using UnityEngine;

public class Hole : MonoBehaviour
{
    #region --- Exposed Fields ---

    public AudioClip Sfx_Ball_Respawn; // Play when the ball respawn

    [Header("Sound Fx")]
    public AudioClip Sfx_Load_Ball; // Play when the ball enter the hole

    public AudioClip Sfx_Shoot_Ball; // Play when force is added to the ball

    public bool b_Open;
    public bool bClose;

    [Header("Kickback")]
    public bool Mission_Kickback; // use to create a kickback mission if true

    [Header("Force you want to add")]
    public float Explosion_force = 5; // Force added to the ball

    [Header("Time Before the ball go to the respawn position")]
    public float Part_1_TimeToRespawn = 2; // Time Before the ball go to the respawn position

    [Header("Time Before a force is added to the ball")]
    public float Part_2_TimeBeforeAddingForce; // Time to wait before adding force to the ball after the respawn			

    [Header("The time until the door closes")]
    public float Part_3_TimeBeforeActivateObjTarget = .5f; // Time to wait before Activate obj_Target			

    public float randomForce;

    [Header("add a led")]
    public GameObject obj_Led; // It is possible to add a led. Usefull for kickback

    [Header("Door for Kickback")]
    public GameObject obj_Target; // Add a drop target here 

    [Header("Toy connected to the bumper")]
    public GameObject Toy_Enter; // Connect a toy or particule system

    public GameObject Toy_Exit;

    [Header("Connect here the mission that use this object")]
    public GameObject[] Parent_Manager; // Connect here the missions that used this object.

    public int AnimNunEnter; // Choose the animation you want to play
    public int AnimNunExit; // Choose the animation you want to play

    [Header("Choose a unique ID")]
    public int index; // Choose an index. Used to be recognized by a mission.

    public int KickbackLedAnimation; // Choose Led ANimation if it's a kickback mission

    [Header("Points added when the ball enter the hole")]
    public int Points; // Points when the object is hitting 

    [Header("Object with animation Curve")]
    public movingObject MovingObjects; // Connect a toy or particule system

    public string functionToCall = "Counter"; // Call a function when OnCollisionEnter -> true;

    [Header("Position when ball respawn. Default : below the hole")]
    public Vector3 RespawnDir = new(0, -1, 0); // position relative to the object Spawn

    #endregion

    #region --- Private Fields ---

    private AudioSource sound_; // Access AudioSource component 
    private bool b_Part_1 = true;
    private bool b_Part_2 = true;
    private bool b_Part_3 = true;

    private bool Pause;

    private BoxCollider Box_Col;
    private ChangeSpriteRenderer led_Renderer;
    private float tmp_Time;
    private float tmp_Time_2;
    private float tmp_Time_3;
    private GameManager gameManager; // access to the script ManagerGame.js on gameObject ManagerGame on the hierarchy
    private GameObject tmp_Ball;

    private Rigidbody rb; // Access to the rigidbody of this object
    private Target target;
    private Toys toyEnter;
    private Toys toyExit; // Connect a toy or particule system

    private Transform Spawn; // Use to know where to spawn the ball

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        // --> Function Start
        Box_Col = GetComponent<BoxCollider>();
        sound_ = GetComponent<AudioSource>(); // Access AudioSource component 
        gameManager = GameManager.Instance; // access to the script ManagerGame.js on gameObject ManagerGame on the hierarchy

        if (obj_Led) led_Renderer = obj_Led.GetComponent<ChangeSpriteRenderer>(); // obj_Led != null Access ChangeSpriteRenderer component 
        if (obj_Target) target = obj_Target.GetComponent<Target>(); // obj_Target != null Access Target component 

        var tmp = gameObject.GetComponentsInChildren<Transform>(true); // Find Spawn Transform. WARNING Don't move spawn gameObject outside the Hole GameObject.
        foreach (var child in tmp)
        {
            if (child.name == "Spawn_Hole")
                Spawn = child;
        }

        if (Toy_Enter) toyEnter = Toy_Enter.GetComponent<Toys>(); // Access component Toys from Toy_Enter
        if (Toy_Exit) toyExit = Toy_Exit.GetComponent<Toys>(); // Access component Toys from Toy_Exit 

        if (MovingObjects) MovingObjects.GetComponent<movingObject>();
    }

    private void Update()
    {
        if (!Pause)
        {
            // --> Update function
            if (!b_Part_1)
            {
                // Part 1 : Part Before the ball go to the respawn position
                if (!Mission_Kickback)
                    tmp_Ball.transform.position = // the ball disappears into the ground
                        Vector3.MoveTowards(tmp_Ball.transform.position,
                            transform.position + Vector3.down * .023f, Time.deltaTime * 0.4f);

                if (tmp_Ball.transform.position == transform.position + Vector3.down * .023f || Mission_Kickback)
                {
                    // The ball has disappeared
                    tmp_Time = Mathf.MoveTowards(tmp_Time, Part_1_TimeToRespawn,
                        Time.deltaTime);
                    if (tmp_Time == Part_1_TimeToRespawn)
                    {
                        // Wait for Part_1_TimeToRespawn
                        tmp_Time = 0;
                        b_Part_1 = true;
                        b_Part_2 = false; // Start Part 2
                        if (!Mission_Kickback)
                        {
                            // if it's not a kickback mission
                            tmp_Ball.transform.position = Spawn.position + RespawnDir * .023f; // the ball go to his spawn position
                            if (!rb.isKinematic) rb.linearVelocity = new Vector3(0, 0, 0);
                            if (Sfx_Ball_Respawn) sound_.PlayOneShot(Sfx_Ball_Respawn); // play a sound : Sfx_Ball_Respawn
                        }
                    }
                }
            }

            if (!b_Part_2)
            {
                // Part 1 : Part before adding force to the ball after the respawn
                Box_Col.enabled = false;


                tmp_Ball.transform.position
                    = Vector3.MoveTowards(tmp_Ball.transform.position,
                        Spawn.position, Time.deltaTime * 0.4f);

                if (tmp_Ball.transform.position == Spawn.position)
                {
                    // the ball appears into the ground
                    tmp_Time_2 = Mathf.MoveTowards(tmp_Time_2,
                        Part_2_TimeBeforeAddingForce, Time.deltaTime);

                    if (tmp_Time_2 == Part_2_TimeBeforeAddingForce)
                    {
                        // Wait for Part_2_TimeBeforeAddingForce
                        tmp_Time_2 = 0;
                        b_Part_2 = true;
                        Ball_AddForceExplosion(); // Add a force to the ball
                        if (Mission_Kickback) b_Part_3 = false; // if it is kickback mission start Part 3
                    }
                }
            }

            if (!b_Part_3)
            {
                // Time to wait before you close the door
                tmp_Time_3 = Mathf.MoveTowards(tmp_Time_3, Part_3_TimeBeforeActivateObjTarget,
                    Time.deltaTime);
                if (tmp_Time_3 == Part_3_TimeBeforeActivateObjTarget)
                {
                    // Wait for Part_3_TimeBeforeActivateObjTarget			
                    tmp_Time_3 = 0;
                    b_Part_3 = true;
                    if (obj_Target) target.Activate_Object(); // Close the door
                    if (Mission_Kickback) Box_Col.isTrigger = false;
                }
            }
        }
    }

    #endregion

    #region --- Methods ---

    public void Ball_AddForceExplosion()
    {
        // --> function to add a force to the ball
        rb.isKinematic = false; // 
        var tmp_randomForce = Random.Range(0, randomForce);
        rb.AddForce(Spawn.transform.forward * (Explosion_force + tmp_randomForce), ForceMode.VelocityChange); // add a force
        rb = null; // A new ball could enter to the hole
        if (Explosion_force > 0) // if a force is added to the ball 
            if (Sfx_Shoot_Ball)
                sound_.PlayOneShot(Sfx_Shoot_Ball); // play a sound : Sfx_Shoot_Ball
        if (Toy_Exit) toyExit.PlayAnimationNumber(AnimNunExit); // Play Toy if connected
        tmp_Ball.GetComponent<Ball>().OutsideHole(); // The ball is not in the hole
        StartCoroutine("I_Ball_AddForceExplosion");


        /* if(MovingObjects && bClose){
             MovingObjects.closeDoor();
         }
          */
    }

    public void F_Pause()
    {
        // Pause the hole
        if (!Pause) Pause = true;
        else Pause = false;
    }

    public int index_info()
    {
        // return the index of the Hole. Use by the mission
        return index;
    }

    public void initHole()
    {
        // Use by Game_Manager function InitGame_GoToMainMenu()
        b_Part_1 = true;
        b_Part_2 = true;
        b_Part_3 = true;
        rb = null;
    }


    private IEnumerator I_Ball_AddForceExplosion()
    {
        yield return new WaitForSeconds(.1f);
        Box_Col.enabled = true;
    }

    #endregion

    #region --- Callbacks ---

    private void OnCollisionEnter(Collision collision)
    {
        // --> OnCollisionEnter
        if (collision.transform.tag == "Ball" && rb == null)
        {
            //	OnCollisionEnter with a ball and There is no ball in the hole actualy
            if (Mission_Kickback)
            {
                Box_Col.isTrigger = true;
                if (gameManager != null) gameManager.PlayMultiLeds(KickbackLedAnimation); // Choose Led ANimation if it's a kickback mission
            }

            tmp_Ball = collision.gameObject;

            rb = tmp_Ball.GetComponent<Rigidbody>();
            rb.isKinematic = true; // rb become kinematic
            rb.position = transform.position;
            rb.rotation = Quaternion.identity;

            if (!sound_.isPlaying && Sfx_Load_Ball) sound_.PlayOneShot(Sfx_Load_Ball); // Play sound : Sfx_Load_Ball

            b_Part_1 = false; // start the first phase
            if (obj_Led) led_Renderer.F_ChangeSprite_Off(); // switch off the led					

            if (Parent_Manager.Length > 0)
                for (var j = 0; j < Parent_Manager.Length; j++)
                    Parent_Manager[j].SendMessage(functionToCall, index); // Call Parents Mission script

            if (gameManager != null)
            {
                gameManager.F_Mode_BONUS_Counter(); // Add +1 to the Bonus counter. Manage by the ManagerGame game object on the hierarchy
                // Get position from collision contact point
                var position = collision.contactCount > 0 ? collision.contacts[0].point : transform.position;
                gameManager.Add_Score(Points, position); // Add points. Manage by the ManagerGame game object on the hierarchy
            }

            if (Toy_Enter) toyEnter.PlayAnimationNumber(AnimNunEnter); // Play Toy if connected

            if (MovingObjects && b_Open)
                MovingObjects.openDoor();
            else if (MovingObjects && bClose) MovingObjects.closeDoor();

            tmp_Ball.GetComponent<Ball>().OnHole(); // The ball is in the hole
        }
    }

    #endregion
}