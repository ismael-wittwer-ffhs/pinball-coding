// MutilBall.js : Description : This script manage how to eject ball when Mutli ball is activated. 

using UnityEngine;

public class MultiBall : MonoBehaviour
{
    #region --- Exposed Fields ---

    public AudioClip s_Load_Ball;
    public AudioClip s_Shoot_Ball;

    public bool Kickback;

    public float Slingshot_force = 4;

    public float Time_Part_1 = 2; // Respawn Timer 

    public float Time_Part_2; // Time to wait before adding force to the ball after the respawn			

    public float Time_Part_3 = .5f; // Time to wait before adding force to the ball after the respawn			

    public GameObject obj_Door;
    //private float tmp_Time_3 = 0;
    //private bool b_Part_3 = true;

    public GameObject obj_Led;
    public GameObject tmp_Ball;
    public GameObject[] Gestionnaire_Parent;

    public int ball_Number = 3;
    public int counter;

    public int index;
    public Rigidbody rb;
    public string functionToCall = "Counter"; // Call a function when OnCollisionEnter -> true;

    public Transform Spawn;
    public Transform Spawn_tmp;

    #endregion

    #region --- Private Fields ---

    private AudioSource source;
    private bool b_Part_1 = true;
    private bool b_Part_2 = true;

    private bool Pause;
    private BoxCollider Box;
    private CameraSmoothFollow pivotCam; // access component CameraSmoothFollow. Use to avoid that the camera move too harshly when the ball respawn on the plunger  
    private float tmp_Time;
    private float tmp_Time_2;
    private GameManager gameManager;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        Box = GetComponent<BoxCollider>();
        source = GetComponent<AudioSource>();
        //gameManager = ManagerGame.Instance; // Uncomment if needed

        var tmp = GameObject.Find("Pivot_Cam");
        if (tmp) pivotCam = tmp.GetComponent<CameraSmoothFollow>(); // Access Component CameraSmoothFollow from the main camera
    }


    private void Update()
    {
        if (!Pause)
        {
            if (!b_Part_1)
            {
                // Respawn Timer 
                tmp_Time = Mathf.MoveTowards(tmp_Time, Time_Part_1,
                    Time.deltaTime);
                if (tmp_Time == Time_Part_1)
                {
                    tmp_Time = 0;
                    b_Part_1 = true;
                    b_Part_2 = false;
                    if (!Kickback)
                        Ball_Respawn();
                }
            }

            if (!b_Part_2)
            {
                // Time to wait before adding force to the ball after the respawn
                tmp_Time_2 = Mathf.MoveTowards(tmp_Time_2, Time_Part_2,
                    Time.deltaTime);
                if (tmp_Time_2 == Time_Part_2)
                {
                    tmp_Time_2 = 0;
                    b_Part_2 = true;
                    Ball_AddForceExplosion();
                }
            }
        }
    }

    #endregion

    #region --- Methods ---

    public void Ball_AddForceExplosion()
    {
        rb.AddForce(Spawn.transform.forward * Slingshot_force, ForceMode.VelocityChange);
        if (Slingshot_force > 0)
        {
            source.clip = s_Shoot_Ball;
            source.Play();
        }

        if (pivotCam) pivotCam.ChangeSmoothTimeInit(); // Call CameraSmoothFollow.js
    }

    public void Ball_Respawn()
    {
        tmp_Ball.transform.position = Spawn.position;
        rb.isKinematic = false;
        rb.linearVelocity = new Vector3(0, 0, 0);
    }

    public void F_Pause()
    {
        if (!Pause) Pause = true;
        else Pause = false;
    }


    public void initHole()
    {
        // Use by Game_Manager function InitGame_GoToMainMenu()
        b_Part_1 = true;
        b_Part_2 = true;
        Box.isTrigger = true;
        rb = null;
    }

    public void KickBack_MultiOnOff()
    {
        if (Box.isTrigger) Box.isTrigger = false;
        else Box.isTrigger = true;
    }

    #endregion

    #region --- Callbacks ---

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ball")
        {
            tmp_Ball = collision.gameObject;
            rb = tmp_Ball.GetComponent<Rigidbody>();
            if (!Kickback)
            {
                rb.isKinematic = true;
                tmp_Ball.transform.position = Spawn.position;
            }

            b_Part_1 = false;
            if (obj_Led) obj_Led.GetComponent<ChangeSpriteRenderer>().F_ChangeSprite_Off();

            if (Gestionnaire_Parent.Length > 0)
                for (var j = 0; j < Gestionnaire_Parent.Length; j++)
                    Gestionnaire_Parent[j].SendMessage(functionToCall, index); // Call Parents Mission script
        }
    }

    #endregion
}