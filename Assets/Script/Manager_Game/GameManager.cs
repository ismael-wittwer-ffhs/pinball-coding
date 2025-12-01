// ManagerGame : Description 

using System;
using System.Collections;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class GameManager : MonoBehaviour
{
    #region --- Nested Types ---

    [Serializable]
    public class Leds_Pattern_Multi
    {
        #region --- Exposed Fields ---

        public GameObject[] obj = new GameObject[1]; // array to connect object with the Tag "Leds_Groups","Missions" 
        public int[] num_pattern = new int[1]; // choose the led animation you want to play. The order as obj
        public Manager_Led_Animation[] manager_Led_Animation = new Manager_Led_Animation[1]; // access component

        #endregion
    }

    #endregion

    #region --- Statics ---

    public static GameManager Instance { get; private set; }

    #endregion

    #region --- Exposed Fields ---

    public AudioClip a_BallSave; // Play a sound when the player lose a ball
    public AudioClip a_Bonus_Screen; // Play a sound during the bonus score 
    public AudioClip a_LoseBall; // Play a sound when the player lose a ball
    public AudioClip s_Load_Ball; // play a sound when the ball respan
    public AudioClip s_Tilt; // Play this sound if mode Tilt start
    public AudioClip s_Warning; // Play this sound when player hit the table
    public bool b_Ball_Saver; // if true : Ball Saver is enabled

    [Header("Bonus Extra Ball")] // --> Bonus Extra Ball
    public bool b_ExtraBall; // if true : extra ball is enabled

    public bool b_InsertCoin_GameStart; // Use when a new game start
    public bool b_Respawn_Timer_Ball_Saver; // use to respawn the ball when ball lost
    public bool Loop_AnimDemoPlayfield = true; // loop global leds animations

    [Header("Bonus Ball Saver")] // --> Bonus Ball Saver
    public bool StartGameWithBallSaver; // If true : player start a new ball with BallSaver

    [Header("Tilt Mode")] // --> Tilt Mode
    public float MinTimeTilt = 1; // minimum time in seconds between two shake 

    public float Respawn_Timer_BallSaver = 2; // use to respawn the ball when ball lost


    [Header("Ball Lost")] // --> BALL LOST (There is 3 parts : Part 1 
    public float Time_Ballout_Part_1_BallOut = 2; // Part 1 : Ball Lost : Choose the duration of this part

    public float Time_Ballout_Part_2_Bonus = 2; // Part 2 : Bonus calculation : Choose the duration of this part					

    public float Time_Ballout_Part_3_TotalScore = 2; // Part 3 : Next Ball or GameOver : Choose the duration of this part		
    public float TimeToWaitMulti = 3; // Time To Wait before the multi ball start 

    [Header("Mode Multi Ball")] // --> Mode Multi Ball
    public GameObject obj_Launcher_MultiBall; // Object that manage the multi-ball on playfield. Manage how the ball is ejected on playfield

    public GameObject obj_Led_Ball_Saver; // Connect here a Led
    public GameObject obj_Led_ExtraBall; // Connect here a Led 
    public GameObject[] Deactivate_Obj; // Deactivate Target when multiball start

    public GameObject[] Deactivate_Obj_WhenMultIsFinished = { }; // Deactivate Target when multiball start

    public GameObject[] obj_Multiplier_Leds; // Connect here 5 Leds
    public int AnimDemoPlayfield; // Choose leds animation used when the game is over or when the scene starts
    public int BallSaverLedAnimation; // Choose Led ANimation when after a ball saved 
    public int Bonus_Base = 100; // 
    public int BONUS_Global_Hit_Counter; // Record the number of object that hit the ball during the current ball
    public int GameOverLedAnimation; // Choose Led Animation when the player lose a ball 

    // Variables used for the gameplay
    [Header("Player Life and Score")]
    public int Life = 3; // --> Life

    public int Mulitplier_SuperBonus = 1000000; // add points if multiplier = 10

    [Header("Bonus Multiplier")] // --> Bonus Multiplier (Bonus Multiplier = multiplier x Bonus_Base x BONUS_Global_Hit_Counter)
    public int multiplier = 1; // multiplier could be x1 x2 x4 x6 x 8 x10							

    public int NewBallLedAnimation; // Choose Led ANimation when there is a new ball on plunger after ball lost 

    public int StartDuration = -1;

    // leds_Multi[0] is auto connected. leds_Multi[0] plays the first animation of each object with the Tag "Leds_Groups","Missions" or "Led_animation"
    // It is better to create your own leds pattern from leds_Multi[1]
    [Header("Global Leds pattern manager")] // --> Global Leds pattern manager
    public Leds_Pattern_Multi[] leds_Multi = new Leds_Pattern_Multi[1];

    public MultiBall multiBall; // Access MultiBall component from obj_Launcher_MultiBall gameObject;

    public SpringLauncher[] spring_Launcher; // Plunger : access component SpringLauncher

    [Header("Score is saved with this name")]
    public string BestScoreName = "BestScore";

    [Header("Text used during game")] // --> Text used during game
    public string[] Txt_Game; // Array : All the text use by the game Manager


    [Header("Ball")] // --> Ball
    public Transform ball; // Connect the ball Prefab

    #endregion

    #region --- Private Fields ---

    [Header("Audio : Sfx")] // --> Audio
    private AudioSource sound; // access AudioSOurce omponent

    private bool b_Ballout_Part_1 = true;
    private bool b_Ballout_Part_2 = true;
    private bool b_Ballout_Part_3 = true;
    private bool b_Game; // True : Player start the game . False : Game is over
    private bool b_Pause; // use to pause the script
    private bool b_Timer_Ball_Saver;
    private bool b_touch_TiltLeft; // use for mobile device
    private bool b_touch_TiltRight; // use for mobile device
    private bool Debug_Game;
    private bool LCD_Wait_Start_Game = true; // use to switch between best score and insert coin

    private bool Mission_Multi_Ball_Ended; // use to know if the multi ball stop

    //private bool OnceTouchLeft = true;				// use for mobile device
    //private bool OnceTouchRight = true;				// use for mobile device
    private bool MobileNudge; // use for mobile device to enable or disable nudge mode
    private bool Multi_Ball; // Mode Multi ball activate or not
    private bool TimeToWaitBeforeMultiBallStart; // prevent bug when a multi ball start after enter a hole

    //private GameObject Camera_Board;					// (auto connected) Camera with the tag "Main Camera"
    private Camera_Movement camera_Movement; // access component Camera_Movement
    private CameraSmoothFollow pivotCam; // access component CameraSmoothFollow. Use to avoid that the camera move too harshly when the ball respawn on the plunger  
    private ChangeSpriteRenderer led_Ball_Saver_Renderer; // Access ChangeSpriteRenderer component from obj_Led_Ball_Saver if a led is connected
    private ChangeSpriteRenderer led_ExtraBall; // Access ChangeSpriteRenderer component from obj_Led_ExtraBall if a led is connected
    private ChangeSpriteRenderer[] led_Multiplier_Renderer; // Access ChangeSpriteRenderer component from obj_Multiplier_Leds if leds are connected
    private float Tilt_Timer; // timer to know if we need to start tilt mode
    private float Timer_Ball_Saver = 2; // Ball Saver duration					
    private float Timer_Multi = 1; // ejection time between two balls 
    private float TimerMultiBall;
    private float tmp_Ball_Saver; // use for the ball saver timer 
    private float tmp_Ballout_Time;
    private float tmp_Ballout_Time_2;
    private float tmp_Ballout_Time_3;

    private GameObject obj_Skillshot_Mission; // (auto connected) with the function F_Init_Skillshot_Mission(obj : GameObject)
    // Use for Tilting mode
    // Txt_Game[0] : Tilt
    // Txt_Game[1] : Tilt Warning

    // Txt_Game[2] + player_Score : Display the score when there is nothing else to display 
    // Txt_Game[3] + (Ball_num+1) : Display the the ball number
    // Txt_Game[4] : Display a text when we are wait a player start the game

    // Bonus text when player lose a ball (There is 3 parts)
    // Part 1 // Txt_Game[5],Txt_Game[6],Txt_Game[7] :   : Txt_Game[5] + "\n" + tmp_BONUS_Global_Hit_Counter + Txt_Game[6] + Bonus_Base + "\n" + tmp_Multiplier + Txt_Game[7], Time_Ballout_Part_2_Bonus);
    // Part 2 // Txt_Game[8] : Txt_Game[8] + "\n" + player_Score.ToString() (display the player score)
    // Part 3 // Txt_Game[9] : Display this text if player life > 0 

    // Txt_Game[10] : Text for ball saver
    // Txt_Game[11] : Text for Extra ball
    // Txt_Game[12] : Text for Ball lost
    // Txt_Game[13] : Text for Game Over
    // Txt_Game[14] : Text when a game start
    // Txt_Game[15] : Text when the scene start


    [Header("Plunger")] // --> Plunger
    private GameObject spawnBall; // (connected automatically) The GameObject that manage the ejection after a ball respawn


    private GameObject[] obj_Managers; // (connected automatically) Array with GameObjects that manage each mission that you can find on playfield
    private GameObject[] obj_Tmp_Leds_Groups;

    private GameObject[] obj_Tmp_Mission;
    private int AnimInProgress; // Use to know the name of global Leds animation is being played
    private int b_Tilt; // 0 : Tilt Desactivate	 	1 : Player shakes the playfield			2 : Tilt Mode Enable  
    private int Ball_num; // the number of ball played by the player
    private int globAnimCount; // Use to know if all the leds animation on playfield are finish
    private int Number_Of_Ball_On_Board; // Know the number of board. 
    private int player_Score; // player score
    private int ReloadNumber = 3; // number of ball for the multi ball mode. Send by the mission with the function gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
    private int tmp_BONUS_Global_Hit_Counter; // Use to calculate the score
    private int tmp_Bonus_Score; // Use to calculate the score
    private int tmp_count;
    private int tmp_index_Info = -1; // lets you know what mission start Multi ball. Send by the mission with the function gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
    private int tmp_Life; // tmp_Life : the number of remaining life
    private int tmp_Multiplier; // Use to calculate the score
    private int tmp_ReloadNumber = 3;
    private int[] Missions_Index; // (connected automatically) Array with the ID of all the mission you can find on playfield


    // variables used for debuging
    private ManagerInputSetting manager_Input_Setting;
    private Pause_Mission[] pause_Mission; // (connected automatically) Access the Pause_Mission component for all the mission on playfield
    private PinballInputManager inputManager;
    private UiManager uiManager;

    #endregion

    #region --- Unity Methods ---

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        manager_Input_Setting = GetComponent<ManagerInputSetting>();
        inputManager = PinballInputManager.Instance;
        uiManager = UiManager.Instance;

        if (inputManager == null) Debug.LogWarning("ManagerGame: PinballInputManager not found. Make sure it exists in the scene.");
        if (uiManager    == null) Debug.LogWarning("ManagerGame: UiManager not found. Make sure it exists in the scene.");

        // Initialize UiManager with text array and best score name
        if (uiManager != null)
        {
            uiManager.Txt_Game = Txt_Game;
            uiManager.BestScoreName = BestScoreName;
        }


        var gos = GameObject.FindGameObjectsWithTag("MainCamera"); // Connect the main camera
        foreach (var go_ in gos)
        {
            if (go_.GetComponent<Camera_Movement>())
                //Camera_Board = go_;
                camera_Movement = go_.GetComponent<Camera_Movement>();
        }


        if (obj_Launcher_MultiBall) // Connect obj_Launcher_MultiBall to the script
            multiBall = obj_Launcher_MultiBall.GetComponent<MultiBall>();

        sound = GetComponent<AudioSource>(); // Access Audiosource component

        if (spawnBall == null) // Connect the Mission to the gameObject : "spawnBall"
            spawnBall = GameObject.Find("Plunger_Spawn");

        gos = GameObject.FindGameObjectsWithTag("Plunger");
        spring_Launcher = new SpringLauncher[gos.Length]; // Prepare array size 

        tmp_count = 0;


        foreach (var go_ in gos)
        {
            spring_Launcher[tmp_count] = go_.GetComponent<SpringLauncher>(); // access Plungers SpringLauncher component
            tmp_count++;
        }

        gos = GameObject.FindGameObjectsWithTag("Missions"); // Find all game objects with tag Missions and put them on arrays. 
        obj_Managers = new GameObject[gos.Length]; // Prepare array size
        Missions_Index = new int[gos.Length]; // Prepare array size

        tmp_count = 0;
        foreach (var go4 in gos)
        {
            obj_Managers[tmp_count] = go4; // Save information inside obj_Managers[] : the Mission gameObject 
            Missions_Index[tmp_count] = go4.GetComponent<MissionIndex>().F_index(); // Save information inside Missions_Index[] : save the index of the Mission
            tmp_count++;
        }

        pause_Mission = new Pause_Mission[obj_Managers.Length]; // Prepare array size
        for (var i = 0; i < obj_Managers.Length; i++) pause_Mission[i] = obj_Managers[i].GetComponent<Pause_Mission>(); // access Pause_Mission component from all object inside obj_Managers[]

        if (obj_Led_ExtraBall) // Init obj_Led_ExtraBall
            led_ExtraBall = obj_Led_ExtraBall.GetComponent<ChangeSpriteRenderer>();
        if (obj_Led_Ball_Saver) // Init obj_Led_Ball_Saver
            led_Ball_Saver_Renderer = obj_Led_Ball_Saver.GetComponent<ChangeSpriteRenderer>();


        if (obj_Multiplier_Leds.Length > 0)
        {
            // Init obj_Multiplier_Leds[] and access ChangeSpriteRenderer component
            led_Multiplier_Renderer = new ChangeSpriteRenderer[obj_Multiplier_Leds.Length];
            for (var i = 0; i < obj_Multiplier_Leds.Length; i++) led_Multiplier_Renderer[i] = obj_Multiplier_Leds[i].GetComponent<ChangeSpriteRenderer>();
        }

        if (leds_Multi[0].obj[0] == null)
        {
            // if nothing is connected to Leds_Multi (LEDS Patterns). automaticaly create a led pattern with objects with the Tag "Leds_Groups","Missions"
            tmp_count = 0;

            var gos_t = GameObject.FindGameObjectsWithTag("Missions");
            obj_Tmp_Mission = new GameObject[gos_t.Length];
            foreach (var go10 in gos_t)
            {
                obj_Tmp_Mission[tmp_count] = go10;
                tmp_count++;
            }

            tmp_count = 0;
            gos_t = GameObject.FindGameObjectsWithTag("Leds_Groups");
            obj_Tmp_Leds_Groups = new GameObject[gos_t.Length];
            foreach (var go10 in gos_t)
            {
                obj_Tmp_Leds_Groups[tmp_count] = go10;
                tmp_count++;
            }

            tmp_count = obj_Tmp_Mission.Length + obj_Tmp_Leds_Groups.Length;

            leds_Multi[0].obj = new GameObject[tmp_count];
            leds_Multi[0].num_pattern = new int[tmp_count];

            tmp_count = 0; //obj_Tmp_Mission.Length + obj_Tmp_Leds_Groups.Length;
            for (var i = 0; i < obj_Tmp_Mission.Length; i++)
            {
                leds_Multi[0].obj[i] = obj_Tmp_Mission[i];
                leds_Multi[0].num_pattern[tmp_count] = 0;
                tmp_count++;
            }

            for (var i = 0; i < obj_Tmp_Leds_Groups.Length; i++)
            {
                leds_Multi[0].obj[tmp_count] = obj_Tmp_Leds_Groups[i];
                leds_Multi[0].num_pattern[tmp_count] = 0;
                tmp_count++;
            }
        }

        for (var j = 0; j < leds_Multi.Length; j++)
        {
            leds_Multi[j].manager_Led_Animation = new Manager_Led_Animation[leds_Multi[j].obj.Length];
            for (var k = 0; k < leds_Multi[j].obj.Length; k++) leds_Multi[j].manager_Led_Animation[k] = leds_Multi[j].obj[k].GetComponent<Manager_Led_Animation>();
        }

        tmp_ReloadNumber = ReloadNumber; // Init the number of ball for Multi player mode

        var tmp = GameObject.Find("Pivot_Cam");
        if (tmp) pivotCam = tmp.GetComponent<CameraSmoothFollow>(); // Access Component CameraSmoothFollow from the main camera

        Debug_Game = manager_Input_Setting.F_Debug_Game();
        //yield WaitForEndOfFrame();
        StartCoroutine("WaitToInit");
    }

    private void Update()
    {
        // UI Navigation and global controls
        if (inputManager != null && uiManager != null)
        {
            // UI: Select a button if nothing is selected
            if (uiManager.IsUIActive() && (Mathf.Abs(inputManager.GetNavigateHorizontal()) == 1 || inputManager.WasPlungerPressed()))
                uiManager.SelectLastButton();

            // Pause Mode
            if (inputManager.WasPausePressed()) F_Pause_Game();

            // Change the Camera view
            if (inputManager.WasCameraChangePressed())
                if (camera_Movement)
                    camera_Movement.Selected_Cam();
        }


        if (!b_Pause)
        {
            /////////////////////////////////	SECTION : Player Input : START /////////////
            // New Game Start
            if (inputManager != null && (uiManager == null || !uiManager.IsUIActive()) && inputManager.WasPlungerPressed())
                if (!b_Game)
                    F_InsertCoin_GameStart();

            if (b_Game)
                if (MobileNudge)
                    // Touch Screen part for TILT Mode using Enhanced Touch
                    foreach (var touch in Touch.activeTouches)
                    {
                        var normalizedX = touch.screenPosition.x / Screen.width;
                        var normalizedY = touch.screenPosition.y / Screen.height;

                        // Left side tilt zone
                        if (normalizedX < 0.5f && normalizedY > 0.6f && normalizedY < 0.8f)
                            if (touch.phase == TouchPhase.Began && b_Tilt <= 1)
                                b_touch_TiltLeft = true;

                        // Right side tilt zone
                        if (normalizedX > 0.5f && normalizedY > 0.6f && normalizedY < 0.8f)
                            if (touch.phase == TouchPhase.Began && b_Tilt <= 1)
                                b_touch_TiltRight = true;
                    }


            if (b_Tilt == 1)
            {
                // --> Player shakes the playfield	: First Time
                Tilt_Timer = Mathf.MoveTowards(Tilt_Timer, MinTimeTilt, Time.deltaTime); // A timer start to know the time between two shake on the table
                if (Tilt_Timer == MinTimeTilt)
                {
                    // Tilt_Timer is init after  MinTimeTilt 
                    b_Tilt = 0; // Init Tilt Mode
                    Tilt_Timer = 0; // init Tilt timer
                }
            }

            // Shake/Tilt input handling (consolidated from legacy dual code paths)
            if (inputManager != null)
            {
                // --> Player shakes the playfield : Right
                if ((inputManager.WasShakeRightPressed() && b_Game) || (b_touch_TiltRight && b_Game))
                {
                    if (b_Tilt == 1)
                    {
                        // Start The Tilt Mode
                        Start_Pause_Mode(-1);
                        if (camera_Movement) camera_Movement.Shake_Cam(1);
                        if (s_Tilt) sound.PlayOneShot(s_Tilt);
                        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 0) uiManager.Add_Info_To_Array(Txt_Game[0], 3);
                        b_Tilt = 2;
                        Tilt_Timer = 0;
                        F_Mode_Ball_Saver_Off();
                        b_ExtraBall = false;
                        if (obj_Led_ExtraBall) led_ExtraBall.F_ChangeSprite_Off();
                        Flippers_Plunger_State_Tilt_Mode("Desactivate");
                        b_touch_TiltRight = false;
                    }
                    else if (b_Tilt == 0)
                    {
                        // First warning
                        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 1) uiManager.Add_Info_To_Array(Txt_Game[1], 1);
                        if (s_Warning) sound.PlayOneShot(s_Warning);
                        if (camera_Movement) camera_Movement.Shake_Cam(1);
                        Shake_AddForce_ToBall(new Vector3(-1, 0, 0));
                        b_Tilt = 1;
                        b_touch_TiltRight = false;
                    }
                }

                // --> Player shakes the playfield : Left
                if ((inputManager.WasShakeLeftPressed() && b_Game) || (b_touch_TiltLeft && b_Game))
                {
                    if (b_Tilt == 1)
                    {
                        Start_Pause_Mode(-1);
                        if (camera_Movement) camera_Movement.Shake_Cam(2);
                        if (s_Tilt) sound.PlayOneShot(s_Tilt);
                        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 0) uiManager.Add_Info_To_Array(Txt_Game[0], 3);
                        b_Tilt = 2;
                        Tilt_Timer = 0;
                        F_Mode_Ball_Saver_Off();
                        b_ExtraBall = false;
                        if (obj_Led_ExtraBall) led_ExtraBall.F_ChangeSprite_Off();
                        Flippers_Plunger_State_Tilt_Mode("Desactivate");
                        b_touch_TiltLeft = false;
                    }
                    else if (b_Tilt == 0)
                    {
                        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 1) uiManager.Add_Info_To_Array(Txt_Game[1], 1);
                        if (s_Warning) sound.PlayOneShot(s_Warning);
                        if (camera_Movement) camera_Movement.Shake_Cam(2);
                        Shake_AddForce_ToBall(new Vector3(1, 0, 0));
                        b_Tilt = 1;
                        b_touch_TiltLeft = false;
                    }
                }

                // --> Player shakes the playfield : Up
                if (inputManager.WasShakeUpPressed() && b_Game)
                {
                    if (b_Tilt == 1)
                    {
                        Start_Pause_Mode(-1);
                        if (camera_Movement) camera_Movement.Shake_Cam(3);
                        if (s_Tilt) sound.PlayOneShot(s_Tilt);
                        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 0) uiManager.Add_Info_To_Array(Txt_Game[0], 3);
                        b_Tilt = 2;
                        Tilt_Timer = 0;
                        F_Mode_Ball_Saver_Off();
                        b_ExtraBall = false;
                        if (obj_Led_ExtraBall) led_ExtraBall.F_ChangeSprite_Off();
                        Flippers_Plunger_State_Tilt_Mode("Desactivate");
                    }
                    else if (b_Tilt == 0)
                    {
                        if (s_Warning) sound.PlayOneShot(s_Warning);
                        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 1) uiManager.Add_Info_To_Array(Txt_Game[1], 1);
                        if (camera_Movement) camera_Movement.Shake_Cam(3);
                        Shake_AddForce_ToBall(new Vector3(0, 0, 1));
                        b_Tilt = 1;
                    }
                }
            }
            /////////////////////////////////	SECTION END /////////////

            /////////////////////////////////	SECTION : INFO : START /////////////
            // Update default LCD display when no special message is showing
            if (uiManager != null && Txt_Game != null) uiManager.UpdateDefaultDisplay(player_Score, Ball_num, LCD_Wait_Start_Game, tmp_Life, PlayerPrefs.GetInt(BestScoreName));

            /////////////////////////////////	SECTION END 	/////////////

            /////////////////////////////////	SECTION : MULTI BALL /////////////
            if (Multi_Ball && Number_Of_Ball_On_Board < 3 && Timer_Multi > 1 && tmp_ReloadNumber > 0)
            {
                // --> Multi ball. condition to create a new ball on playfied
                newBall(spawnBall.transform.position);
                Timer_Multi = 0;
                tmp_ReloadNumber--;
            }

            if (Multi_Ball)
            {
                // Wait between two balls
                Timer_Multi += Time.deltaTime;

                if (tmp_ReloadNumber == 0 && Timer_Multi > 1) // if there is no more ball to launch
                    F_Mode_MultiBall();
            }

            /////////////////////////////////	SECTION : MULTI BALL : END	/////////////

            /////////////////////////////////	SECTION : BALL SAVER /////////////
            if (b_Respawn_Timer_Ball_Saver)
            {
                // --> Ball Saver : use to respawn the ball when ball is lost and BallSaver = true
                Respawn_Timer_BallSaver = Mathf.MoveTowards(Respawn_Timer_BallSaver, 1, // Wait
                    Time.deltaTime);
                if (Respawn_Timer_BallSaver == 1)
                {
                    // Respan the ball
                    b_Respawn_Timer_Ball_Saver = false;
                    multiBall.KickBack_MultiOnOff();
                }
            }

            if (b_Timer_Ball_Saver)
            {
                // --> Ball Saver Duration : Check if we need to disable ball saver
                tmp_Ball_Saver = Mathf.MoveTowards(tmp_Ball_Saver, Timer_Ball_Saver,
                    Time.deltaTime);
                if (Timer_Ball_Saver == tmp_Ball_Saver)
                {
                    // Stop the ball saver
                    b_Timer_Ball_Saver = false;
                    tmp_Ball_Saver = 0;
                    F_Mode_Ball_Saver_Off();
                }
            }

            if (TimeToWaitBeforeMultiBallStart)
            {
                // Add a delay before starting multi ball
                TimerMultiBall = Mathf.MoveTowards(TimerMultiBall, TimeToWaitMulti, Time.deltaTime);
                if (TimerMultiBall == TimeToWaitMulti)
                {
                    TimerMultiBall = 0;
                    TimeToWaitBeforeMultiBallStart = false;
                    F_Mode_MultiBall(); // Start the Mode_Multi ball by calling F_Mode_MultiBall()
                }
            }
            /////////////////////////////////	SECTION : TIMER BALL SAVER : END	/////////////

            /////////////////////////////////	SECTION : END OF A BALL /////////////
            if (!b_Ballout_Part_1)
            {
                // --> End of a ball : 
                b_Tilt = 2; // when player lose a ball. Disable Tilt Mode

                for (var i = 0; i < spring_Launcher.Length; i++) spring_Launcher[i].F_Desactivate(); // Desactivate plunger
                if (obj_Skillshot_Mission)
                    obj_Skillshot_Mission.SendMessage("Disable_Skillshot_Mission"); // Disable the skillshot mission

                tmp_Ballout_Time = Mathf.MoveTowards(tmp_Ballout_Time, Time_Ballout_Part_1_BallOut, // Wait
                    Time.deltaTime);
                if (tmp_Ballout_Time == Time_Ballout_Part_1_BallOut)
                {
                    // Part 1
                    if (a_Bonus_Screen)
                    {
                        // Play a sound
                        sound.clip = a_Bonus_Screen;
                        sound.Play();
                    }

                    tmp_Ballout_Time = 0;
                    b_Ballout_Part_1 = true;
                    b_Ballout_Part_2 = false; // start Part 2
                    Add_Info_To_Array(Txt_Game[5]                                + "\n" // display a text
                                                                                 + tmp_BONUS_Global_Hit_Counter + Txt_Game[6] + Bonus_Base
                                                                                 + " x "                        + "\n"        + tmp_Multiplier + Txt_Game[7], Time_Ballout_Part_2_Bonus);
                    Total_Ball_Score(); // Add Bonus Points to player_Score
                }
            }

            if (!b_Ballout_Part_2)
            {
                // Part 2
                tmp_Ballout_Time_2 = Mathf.MoveTowards(tmp_Ballout_Time_2, Time_Ballout_Part_2_Bonus, // Wait
                    Time.deltaTime);
                if (tmp_Ballout_Time_2 == Time_Ballout_Part_2_Bonus)
                {
                    tmp_Ballout_Time_2 = 0;
                    b_Ballout_Part_2 = true;
                    b_Ballout_Part_3 = false; // start Part 3
                    Add_Info_To_Array(Txt_Game[8] + "\n" + player_Score, Time_Ballout_Part_3_TotalScore); // display a text
                }
            }

            if (!b_Ballout_Part_3)
            {
                // Part 3
                tmp_Ballout_Time_3 = Mathf.MoveTowards(tmp_Ballout_Time_3, Time_Ballout_Part_3_TotalScore, // Wait
                    Time.deltaTime);
                if (tmp_Ballout_Time_3 == Time_Ballout_Part_3_TotalScore)
                {
                    tmp_Ballout_Time_3 = 0;
                    b_Ballout_Part_3 = true;
                    if (tmp_Life >= 1)
                    {
                        // If tmp_Life > 1 new ball for the player
                        PlayMultiLeds(NewBallLedAnimation);
                        newBall(spawnBall.transform.position);
                        if (StartGameWithBallSaver) // If StartGameWithBallSaver = true. A new ball start with BallSaver
                            F_Mode_Ball_Saver_On(StartDuration); // if value = -1 Ball saver stay enable until the player lose the ball 	
                        Add_Info_To_Array(Txt_Game[9], 2); // display a text
                        b_Tilt = 0; // init Tilt Mode When player lose a ball
                        if (obj_Skillshot_Mission) // Use if you a mission to be a skillshot mission
                            obj_Skillshot_Mission.SendMessage("Enable_Skillshot_Mission"); // The skillshot mission is enabled
                        Stop_Pause_Mode(); // Stop Pause Mode for all the missions. useful if the player loses the ball because of the tilt mode
                    }
                    else
                    {
                        // -> if GameOver
                        PlayMultiLeds(AnimDemoPlayfield); // Play a loop animation until the game Start
                        Loop_AnimDemoPlayfield = true; // enable to loop global leds animations
                        b_Game = false;
                        b_Tilt = 0; // init Tilt Mode When player lose a ball
                        Stop_Pause_Mode(); // Stop Pause Mode for all the missions. useful if the player loses the ball because of the tilt mode
                        if (uiManager != null)
                            uiManager.ShowGameOverUI();
                    }
                }
            }
            /////////////////////////////////	SECTION : END OF A BALL : END	/////////////
        }

        ////////////////////////////////	DEBUG
        if (Debug_Game) Debug_Input(); // Input for debugging
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion

    #region --- Methods ---

    public void Add_Info_To_Array(string inf, float Timer)
    {
        // --> Score Text : Call This function to add text to LCD screen
        if (uiManager != null)
            uiManager.Add_Info_To_Array(inf, Timer);
    }

    public void Add_Info_To_Timer(string inf)
    {
        // --> Timer Text : Call This function to add text to timer
        if (uiManager != null)
            uiManager.Add_Info_To_Timer(inf);
    }

    public void Add_Score(int addScore)
    {
        // --> Call This function to add points
        player_Score += addScore;

        if (player_Score > 999999999) // Max score is 999,999,999 points					
            player_Score = 999999999;

        // Update UI with new score
        if (uiManager != null && Txt_Game != null && Txt_Game.Length > 3)
        {
            uiManager.UpdateScore(player_Score, Ball_num, LCD_Wait_Start_Game, tmp_Life);
            uiManager.SetLCDWaitStartGame(LCD_Wait_Start_Game);
        }
    }

    public bool Ball_Saver_State()
    {
        return b_Ball_Saver;
    }


    public void checkGlobalAnimationEnded()
    {
        // --> Check if a global leds animation is finished. Every mission call this function when her animation is finished.  
        globAnimCount++;
        if (globAnimCount == leds_Multi[AnimInProgress].obj.Length - 1)
        {
            globAnimCount = 0;
            if (Loop_AnimDemoPlayfield) PlayMultiLeds(AnimDemoPlayfield);
        }
    }


    /////////////////////////////////	SECTION : Gameplay : END /////////////


    /////////////////////////////////	SECTION : Debug : START /////////////
    public void Debug_Input()
    {
        if (!b_Pause)
        {
            /*if(Input.GetKeyDown(manager_Input_Setting.F_Start_Pause_Mode())){Start_Pause_Mode(-1);}					// Start_Pause_Mode(the_Mission_Who_Must_Keep_Active : int) Correspond au numéro de la mission du obj_Managers l'on veut stopper. -1 correspond à tout le monde en pause
        if(Input.GetKeyDown(manager_Input_Setting.F_Stop_Pause_Mode())){Stop_Pause_Mode();}
        if(Input.GetKeyDown(manager_Input_Setting.F_Init_All_Mission())){Init_All_Mission();}*/
            //if(Input.GetKeyDown(manager_Input_Setting.F_PlayMultiLeds())){PlayMultiLeds(0);}
            /*if(Input.GetKeyDown(manager_Input_Setting.F_newBall())){newBall(spawnBall.transform.position);};
        if(Input.GetKeyDown(manager_Input_Setting.F_Mode_MultiBall())){F_Mode_MultiBall();}*/
            //if(Input.GetKeyDown(manager_Input_Setting.F_Ball_Saver_and_ExtraBall())){F_Mode_Ball_Saver_On(-1); F_Mode_ExtraBall();}
            //if(Input.GetKeyDown(manager_Input_Setting.F_Ball_Saver_Off())){F_Mode_Ball_Saver_Off();}
        }
    }

    public bool ExtraBall_State()
    {
        return b_ExtraBall;
    }

    public void F_Ball_Saver_Off()
    {
        if (!b_Pause) F_Mode_Ball_Saver_Off();
    }

    public void F_Ball_Saver_On()
    {
        if (!b_Pause) F_Mode_Ball_Saver_On(-1);
    }

    public void F_ExtraBall()
    {
        if (!b_Pause) F_Mode_ExtraBall();
    }

    public void F_Init_All_Mission()
    {
        if (!b_Pause) Init_All_Mission();
    }

    public void F_Init_Skillshot_Mission(GameObject obj)
    {
        // --> Call by the skillshot mission
        obj_Skillshot_Mission = obj; // 
    }

    public void F_InsertCoin_GameStart()
    {
        if (!b_Pause) InsertCoin_GameStart();
        if (uiManager != null)
            uiManager.ShowGameStartUI();
    }


    public void F_Mission_MultiBall(int index_Info, int nbr_of_Ball)
    {
        // Use for Mission
        ReloadNumber = nbr_of_Ball; // Choose the number of balls of you want for the multi ball
        tmp_ReloadNumber = ReloadNumber; // init var tmp_ReloadNumber. the variable is used to determine how much ball remains before the multi-ball end.
        tmp_index_Info = index_Info; // Save the mission that start the multi-ball
        if (camera_Movement) camera_Movement.Camera_MultiBall_Start(); // Change the Camera view To Camera 4
        TimeToWaitBeforeMultiBallStart = true; // add delay to start Multi-Ball. Use to prevent bug when multi ball after enter a hole. You need to add a delay to be sure multi ball start after the ball respawn from the hole
        if (Deactivate_Obj.Length > 0) // activate Object after multiball. Only work with drop target
            for (var i = 0; i < Deactivate_Obj.Length; i++)
                Deactivate_Obj[i].SendMessage("Activate_Object");

        if (Deactivate_Obj_WhenMultIsFinished.Length > 0) // activate Object after multiball. Only work with drop target
            for (var i = 0; i < Deactivate_Obj_WhenMultIsFinished.Length; i++)
                Deactivate_Obj_WhenMultIsFinished[i].SendMessage("Activate_Object");
    }

    public void F_Mode_Ball_Saver_Off()
    {
        // Use for Mission. Disabled Ball saver
        Timer_Ball_Saver = 0; // Init				
        tmp_Ball_Saver = 0; // Init 																	
        b_Timer_Ball_Saver = false; // init
        b_Ball_Saver = false; // Ball Saver is disable
        if (obj_Led_Ball_Saver) led_Ball_Saver_Renderer.F_ChangeSprite_Off(); // Switch Off a light on the playfield
    }


    public void F_Mode_Ball_Saver_On(int value)
    {
        // Use for Mission. Enabled ball saver
        Timer_Ball_Saver = value; // Choose the duration of the Ball Saver				
        tmp_Ball_Saver = 0; // Init saver timer 
        if (value != -1) // if value = -1 Ball saver stay enable until the player lose the ball 								
            b_Timer_Ball_Saver = true; // Ball Saver timer start

        b_Ball_Saver = true; // Ball Saver is enable
        if (obj_Led_Ball_Saver) led_Ball_Saver_Renderer.F_ChangeSprite_On(); // Switch On a light on the playfield
    }

    public void F_Mode_BONUS_Counter()
    {
        // --> BONUS
        BONUS_Global_Hit_Counter++;
    }

    public void F_Mode_ExtraBall()
    {
        // Use for Mission
        b_ExtraBall = true;
        if (obj_Led_ExtraBall) led_ExtraBall.F_ChangeSprite_On();
    }


    public void F_Mode_MultiBall()
    {
        // --> MULTI BALL
        if (Multi_Ball)
        {
            //if(ball_Follow)ball_Follow.Start_BallFollow();											
            tmp_ReloadNumber = ReloadNumber; // Muli Ball Stop
            Multi_Ball = false;
            multiBall.KickBack_MultiOnOff();
            Mission_Multi_Ball_Ended = true;

            if (Deactivate_Obj.Length > 0) // Deactivate Object after multiball. Only work with drop target
                for (var i = 0; i < Deactivate_Obj.Length; i++)
                    Deactivate_Obj[i].SendMessage("Desactivate_Object");
        }
        else
        {
            //if(ball_Follow)ball_Follow.Stop_BallFollow();
            Multi_Ball = true; // Muli Ball Start
            multiBall.KickBack_MultiOnOff();
        }
    }

    public void F_MultiBall()
    {
        if (!b_Pause) F_Mode_MultiBall();
    }

    // Initialize multiplier when a ball is lost
    public void F_Multiplier()
    {
        // --> MULTIPLIER : use to multiply bonus score at the end of ball. Multiplier is initialize when the ball is lost. 
        if (multiplier == 1)
        {
            // multiplier increase  : x2 x4 x6 x8 x10 then Mulitplier_SuperBonus
            multiplier = 2;
            if (obj_Multiplier_Leds.Length > 0) led_Multiplier_Renderer[0].F_ChangeSprite_On();
        }
        else if (multiplier < 10)
        {
            // Max multiplier is 10
            var valueTmp = multiplier * .5f;
            if (obj_Multiplier_Leds.Length > 0) led_Multiplier_Renderer[(int)valueTmp].F_ChangeSprite_On();
            multiplier += 2;
        }
        else if (multiplier >= 10)
        {
            Add_Score(Mulitplier_SuperBonus);
            if (multiplier == 10) multiplier += 2;
        }
    }


    public void F_NewBall()
    {
        if (!b_Pause) newBall(spawnBall.transform.position);
    }


    public void F_Pause_Game()
    {
        if (uiManager != null && b_Game)
        {
            // If UI connected and game start
            Pause_Game(); // Toggle pause state
            uiManager.TogglePauseUI(b_Pause, b_Game);
        }
        else
        {
            // If no UI connected
            Pause_Game();
        }
    }

    public void F_PlayMultiLeds()
    {
        if (!b_Pause) PlayMultiLeds(0);
    }

    public void F_Quit_No()
    {
        // Stop pause
        Pause_Game();
        if (uiManager != null)
            uiManager.ShowQuitNoUI();
    }

    public void F_Quit_Yes()
    {
        // return to the main menu	
        InitGame_GoToMainMenu();
        if (uiManager != null)
            uiManager.ShowQuitYesUI();
    }

    public int F_return_Mulitplier_SuperBonus()
    {
        return Mulitplier_SuperBonus;
    }

    public int F_return_multiplier()
    {
        return multiplier;
    }

    public void F_Start_Pause_Mode()
    {
        if (!b_Pause) Start_Pause_Mode(-1);
    }

    public void F_Stop_Pause_Mode()
    {
        if (!b_Pause) Stop_Pause_Mode();
    }

    public void Flippers_Plunger_State_Tilt_Mode(string State)
    {
        // --> Activate or Desactivate the flipper and Plunger when table is tilted
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Flipper"); // Find all game objects with tag Flipper
        foreach (var go in gos)
        {
            if (State == "Activate")
                go.GetComponent<Flippers>().F_Activate(); //  Activate Flippers
            else
                go.GetComponent<Flippers>().F_Desactivate(); // Desactivate Flippers
        }

        gos = GameObject.FindGameObjectsWithTag("Plunger"); // Find all game objects with tag Flipper
        foreach (var go in gos)
        {
            if (State == "Activate")
                go.GetComponent<SpringLauncher>().F_Activate_After_Tilt(); //  Activate plunger
            else
                go.GetComponent<SpringLauncher>().Tilt_Mode(); // Desactivate plunger					
        }
    }
    /////////////////////////////////	SECTION : Put the Game on PAUSE MODE : END /////////////


    /////////////////////////////////	SECTION : Gameplay : START /////////////
    public void gamePlay(GameObject other)
    {
        // -->  The player lose a ball. Call by the script Pinball_TriggerForBall.js on object Out_Hole_TriggerDestroyBall on the hierarchy. This object detect when a ball is lost										
        Destroy(other); // Destroy the ball
        if (pivotCam) pivotCam.ChangeSmoothTimeWhenBallIsLost(); //  It avoids that the camera move too harshly when the ball respawn on the plunger  
        Number_Of_Ball_On_Board--; // Number_Of_Ball_On_Board -1

        if (Mission_Multi_Ball_Ended && Number_Of_Ball_On_Board == 1)
        {
            // Condition to stop Multi Ball : Mission_Multi_Ball_Ended && Number_Of_Ball_On_Board == 1
            if (camera_Movement) camera_Movement.Camera_MultiBall_Stop(); // change the camera view
            for (var i = 0; i < Missions_Index.Length; i++)
            {
                if (Missions_Index[i] == tmp_index_Info && tmp_index_Info != -1)
                    obj_Managers[i].SendMessage("Mode_MultiBall_Ended"); // Stop multi ball	
            }

            Mission_Multi_Ball_Ended = false;
        }

        if (b_Ball_Saver && Number_Of_Ball_On_Board == 0)
        {
            // --> Condition to Activate Ball saver
            PlayMultiLeds(BallSaverLedAnimation);
            multiBall.KickBack_MultiOnOff();
            newBall(spawnBall.transform.position);
            Add_Info_To_Array(Txt_Game[10], 3);
            Respawn_Timer_BallSaver = 0;
            b_Respawn_Timer_Ball_Saver = true;
            b_Ball_Saver = false;
            if (obj_Led_Ball_Saver) led_Ball_Saver_Renderer.F_ChangeSprite_Off();

            if (a_BallSave)
            {
                sound.clip = a_BallSave;
                sound.Play();
            }
        }
        else if (b_ExtraBall && Number_Of_Ball_On_Board == 0)
        {
            // --> Condition to Activate Extra ball
            Add_Info_To_Array(Txt_Game[11], 3);
            newBall(spawnBall.transform.position);
            b_ExtraBall = false;
            if (obj_Led_ExtraBall) led_ExtraBall.F_ChangeSprite_Off();
        }
        else if (tmp_Life > 1 && Number_Of_Ball_On_Board == 0)
        {
            // --> New Ball
            Add_Info_To_Array(Txt_Game[12], 3);
            b_Ballout_Part_1 = false;
            if (a_LoseBall)
            {
                sound.clip = a_LoseBall;
                sound.Play();
            }

            init_Param_After_Ball_Lost();
            tmp_Life--;
            var gos = GameObject.FindGameObjectsWithTag("Missions");
            foreach (var go in gos) go.SendMessage("Mission_Intialisation_AfterBallLost"); // --> Init missions only if the mission must init after player lose the ball
            PlayMultiLeds(GameOverLedAnimation);
            Ball_num++;
            MobileNudge = false; // Nudge Mode on mobile is disable
        }
        else if (Number_Of_Ball_On_Board <= 0)
        {
            // --> Game Over
            // GAME OVER
            if (uiManager != null) uiManager.ClearBallInfo(); // We don't want to display the ball number when there is player information on LCD Fake Screen
            Add_Info_To_Array(Txt_Game[13], 3);
            b_Ballout_Part_1 = false;
            if (a_LoseBall)
            {
                sound.clip = a_LoseBall;
                sound.Play();
            }

            init_Param_After_Ball_Lost();
            tmp_Life--;
            Init_All_Mission();
            PlayMultiLeds(GameOverLedAnimation);
            Save_Best_Score(); // Check if the player have beaten the best score
            MobileNudge = false; // Nudge Mode on mobile is disable
        }
    }

    public int HowManyAnimation()
    {
        return leds_Multi.Length;
    }

    public void Init_All_Mission()
    {
        // --> Initialize all the mission

        var gos = GameObject.FindGameObjectsWithTag("Missions"); // Find all game objects with tag Missions
        foreach (var go in gos)
        {
            go.SendMessage("Mission_Intialisation_StartGame"); // Init missions when the game start
            if (tmp_Life == 0 || b_InsertCoin_GameStart)
            {
                // When player is game Over or when a new game start
                go.SendMessage("InitLedMission"); // Init the leds that indicate a mission is complete
                init_Param_After_Game_Over();
            }
        }

        Stop_Pause_Mode();
    }

    public void init_Param_After_Ball_Lost()
    {
        // --> INIT : init_Param_After_Ball_Lost 
        if (multiplier > 10) multiplier = 10;
        tmp_Bonus_Score = BONUS_Global_Hit_Counter * Bonus_Base * multiplier; // Calculate Bonus_Score
        tmp_Multiplier = multiplier;
        tmp_BONUS_Global_Hit_Counter = BONUS_Global_Hit_Counter;
        BONUS_Global_Hit_Counter = 0;
        multiplier = 1;
        if (obj_Multiplier_Leds.Length > 0)
            for (var i = 0; i < led_Multiplier_Renderer.Length; i++)
                led_Multiplier_Renderer[i].F_ChangeSprite_Off(); // Switch off Multiplier Leds

        Flippers_Plunger_State_Tilt_Mode("Activate"); // Usefull when the main playfield is tilted. Reactivate the flippers and plunger after Tilt Mode
    }

    public void init_Param_After_Game_Over()
    {
        // --> INIT : init_Param_After_Ball_Lost 

        Multi_Ball = false;
        Mission_Multi_Ball_Ended = false;
        b_ExtraBall = false;
        b_Ball_Saver = false;
        b_Respawn_Timer_Ball_Saver = false;
        tmp_Ball_Saver = 0;
        b_Timer_Ball_Saver = false;

        if (Deactivate_Obj.Length > 0) // Deactivate Deactivate_Obj.
            for (var i = 0; i < Deactivate_Obj.Length; i++)
                Deactivate_Obj[i].SendMessage("Desactivate_Object");

        if (Deactivate_Obj_WhenMultIsFinished.Length > 0) // Deactivate Deactivate_Obj.
            for (var i = 0; i < Deactivate_Obj_WhenMultIsFinished.Length; i++)
                Deactivate_Obj_WhenMultIsFinished[i].SendMessage("Desactivate_Object");


        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Ball"); // Find all game objects with tag Ball
        foreach (var go in gos) Destroy(go); // Destroy Ball on board
        if (StartGameWithBallSaver) // If StartGameWithBallSaver = true. A new ball start with BallSaver
            F_Mode_Ball_Saver_On(StartDuration); // if value = -1 Ball saver stay enable until the player lose the ball 	
        Flippers_Plunger_State_Tilt_Mode("Activate"); // Usefull when the main playfield is tilted. Reactivate the flippers and plunger after Tilt Mode
    }

    public void InitGame_GoToMainMenu()
    {
        // --> Init Game when you choose No on UI Menu
        //Loop_AnimDemoPlayfield = true;												// Leds animation : start the loop animation

        b_Game = false; // Game could Start
        b_InsertCoin_GameStart = true;
        if (b_Pause) // Stop Pause
            Pause_Game();


        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Ball"); // Find all game objects with tag Ball
        foreach (var go in gos) Destroy(go); // Destroy Ball on board

        gos = GameObject.FindGameObjectsWithTag("Led_animation"); // Find all game objects with tag Led_animation
        foreach (var go in gos) go.GetComponent<Anim_On_LCD>().DestoyAnimGameobject(); //Destroy

        Init_All_Mission(); // Initialise all the missions

        gos = GameObject.FindGameObjectsWithTag("Flipper"); // Find all game objects with tag Flipper
        foreach (var go in gos) go.GetComponent<Flippers>().F_Activate(); // Activate Flippers
        gos = GameObject.FindGameObjectsWithTag("Plunger"); // Find all game objects with tag Plunger
        foreach (var go in gos) go.GetComponent<SpringLauncher>().F_Activate(); // Activate Plunger

        gos = GameObject.FindGameObjectsWithTag("Hole"); // Find all game objects with tag Hole
        foreach (var go in gos) go.GetComponent<Hole>().initHole(); // Init Hole. Usefull if player stop the game and a ball is on hole
        gos = GameObject.FindGameObjectsWithTag("Hole_Multi"); // Find all game objects with tag Hole_Multi
        foreach (var go in gos) go.GetComponent<MultiBall>().initHole(); // Pause_Mode for these objects


        tmp_Life = 0; // Init Life
        player_Score = 0; // Init Score
        Ball_num = 0;
        Number_Of_Ball_On_Board = 0;
        tmp_Ballout_Time = 0;
        b_Ballout_Part_1 = true;
        tmp_Ballout_Time_2 = 0;
        b_Ballout_Part_2 = true;
        tmp_Ballout_Time_3 = 0;
        b_Ballout_Part_3 = true;
        BONUS_Global_Hit_Counter = 0;
        multiplier = 1;
        init_Param_After_Ball_Lost();
        init_Param_After_Game_Over();

        if (obj_Skillshot_Mission) // Use if you a mission to be a skillshot mission
            obj_Skillshot_Mission.SendMessage("Disable_Skillshot_Mission"); // The skillshot mission is enabled

        Multi_Ball = false; // init Multi Ball
        tmp_ReloadNumber = 0;
        Timer_Multi = 2;
        if (camera_Movement) camera_Movement.Camera_MultiBall_Stop(); // change the camera view
        for (var i = 0; i < Missions_Index.Length; i++)
        {
            if (Missions_Index[i] == tmp_index_Info && tmp_index_Info != -1)
                obj_Managers[i].SendMessage("Mode_MultiBall_Ended"); // Stop multi ball	
        }

        Mission_Multi_Ball_Ended = false;

        //if(spawnBall != null)newBall(spawnBall.transform.position);	
        b_InsertCoin_GameStart = false;

        Add_Info_To_Array(Txt_Game[4], 3);
        LCD_Wait_Start_Game = true;
        //if(Gui_Txt_Score)Gui_Txt_Score.text =  Txt_Game[4] + "\n" + "<size=17>" + Txt_Game[16] + PlayerPrefs.GetInt(BestScoreName).ToString() + "</size>";


        StartCoroutine("InitGameWaitForFrame");
    }

    public void InitObjAfterMultiball()
    {
        // Deactivate Deactivate_Obj_WhenMultIsFinished when multiball mission is finished
        //Debug.Log("Here");
        if (Deactivate_Obj_WhenMultIsFinished.Length > 0) // Deactivate Deactivate_Obj.
            for (var i = 0; i < Deactivate_Obj_WhenMultIsFinished.Length; i++)
                Deactivate_Obj_WhenMultIsFinished[i].SendMessage("Desactivate_Object");
    }

    public void InsertCoin_GameStart()
    {
        // --> Insert Coin : Initialisation when Game Start
        LCD_Wait_Start_Game = false;
        Loop_AnimDemoPlayfield = false; // Leds animation : Stop the loop animation
        Add_Info_To_Array(Txt_Game[14], 3);
        b_Game = true; // Game Start
        b_InsertCoin_GameStart = true;
        if (b_Pause) // Stop Pause
            Pause_Game();


        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Ball"); // Find all game objects with tag Ball
        foreach (var go in gos) Destroy(go); // Destroy Ball on board

        gos = GameObject.FindGameObjectsWithTag("Led_animation"); // Find all game objects with tag Led_animation
        foreach (var go in gos) go.GetComponent<Anim_On_LCD>().DestoyAnimGameobject(); //Destroy

        Init_All_Mission(); // Initialise all the missions

        gos = GameObject.FindGameObjectsWithTag("Flipper"); // Find all game objects with tag Flipper
        foreach (var go in gos) go.GetComponent<Flippers>().F_Activate(); // Activate Flippers
        gos = GameObject.FindGameObjectsWithTag("Plunger"); // Find all game objects with tag Plunger
        foreach (var go in gos) go.GetComponent<SpringLauncher>().F_Activate(); // Activate Plunger

        tmp_Life = Life; // Init Life
        player_Score = 0; // Init Score
        Ball_num = 0;
        Number_Of_Ball_On_Board = 0;
        tmp_Ballout_Time = 0;
        b_Ballout_Part_1 = true;
        tmp_Ballout_Time_2 = 0;
        b_Ballout_Part_2 = true;
        tmp_Ballout_Time_3 = 0;
        b_Ballout_Part_3 = true;
        BONUS_Global_Hit_Counter = 0;
        multiplier = 1;
        init_Param_After_Ball_Lost();
        init_Param_After_Game_Over();

        if (obj_Skillshot_Mission) // Use if you a mission to be a skillshot mission
            obj_Skillshot_Mission.SendMessage("Enable_Skillshot_Mission"); // The skillshot mission is enabled

        if (spawnBall != null) newBall(spawnBall.transform.position);
        PlayMultiLeds(NewBallLedAnimation);
        b_InsertCoin_GameStart = false;
    }

    public void newBall(Vector3 pos)
    {
        // --> NEW BALL . Create a ball on playfield
        if (s_Load_Ball) sound.PlayOneShot(s_Load_Ball);
        Instantiate(ball, pos, Quaternion.identity);
        Number_Of_Ball_On_Board++;
    }

    public void NewValueForUi(int value)
    {
        if (uiManager != null)
            uiManager.NewValueForUi(value);
    }


    public void NudgeEnable(bool value)
    {
        MobileNudge = value;
    }

    /////////////////////////////////	SECTION : Put the Game on PAUSE MODE : START /////////////
    public void Pause_Game()
    {
        if (!b_Pause) b_Pause = true; // Pause ManagerGame 
        else b_Pause = false;

        GameObject[] gos;


        gos = GameObject.FindGameObjectsWithTag("MainCamera"); // Find all game objects with tag MainCamera
        foreach (var go in gos)
        {
            if (b_Pause)
            {
                if (go.GetComponent<Camera_Movement>()) go.GetComponent<Camera_Movement>().StartPauseMode(); // Desactivate Camera Movement
            }
            else
            {
                if (go.GetComponent<Camera_Movement>()) go.GetComponent<Camera_Movement>().StopPauseMode(); // Activate Camera Movement
            }
        }

        gos = GameObject.FindGameObjectsWithTag("PivotCam"); // Find all game objects with tag PivotCam
        foreach (var go in gos)
        {
            if (b_Pause)
                go.GetComponent<CameraSmoothFollow>().StartPauseMode(); // Desactivate Camera Movement
            else
                go.GetComponent<CameraSmoothFollow>().StopPauseMode(); // Activate Camera Movement
        }


        gos = GameObject.FindGameObjectsWithTag("Ball"); // Find all game objects with tag Ball
        foreach (var go in gos) go.GetComponent<Ball>().Ball_Pause(); // Pause_Mode for these objects

        gos = GameObject.FindGameObjectsWithTag("Leds_Groups"); // Find all game objects with tag Leds_Groups
        foreach (var go3 in gos) go3.GetComponent<Manager_Led_Animation>().Pause_Anim(); // Pause_Mode for these objects
        gos = GameObject.FindGameObjectsWithTag("Missions"); // Find all game objects with tag Missions
        foreach (var go4 in gos) go4.GetComponent<Pause_Mission>().Pause_Game(); // Pause_Mode for these objects

        gos = GameObject.FindGameObjectsWithTag("Led_animation"); // Find all game objects with tag Led_animation
        foreach (var go5 in gos) go5.GetComponent<Anim_On_LCD>().Pause_Anim(); // Pause_Mode for these objects


        gos = GameObject.FindGameObjectsWithTag("Flipper"); // Find all game objects with tag Flipper
        foreach (var go in gos)
        {
            if (b_Pause)
                go.GetComponent<Flippers>().F_Pause_Start(); // Desactivate Flippers
            else
                go.GetComponent<Flippers>().F_Pause_Stop(); // Activate Flippers
        }

        gos = GameObject.FindGameObjectsWithTag("Plunger"); // Find all game objects with tag Plunger
        foreach (var go in gos)
        {
            if (b_Pause)
                go.GetComponent<SpringLauncher>().F_Desactivate(); // Desactivate Plunger
            else
                go.GetComponent<SpringLauncher>().F_Activate(); // Activate Plunger
        }


        GetComponent<Blink>().Pause_Blinking(); // Pause the blinking system


        gos = GameObject.FindGameObjectsWithTag("AnimatedObject"); // Find all game objects with tag AnimatedObject
        foreach (var go in gos)
        {
            if (go.GetComponent<Toys>())
                go.GetComponent<Toys>().Pause_Anim(); // Pause_Mode for these objects
            if (go.GetComponent<movingObject>())
                go.GetComponent<movingObject>().Pause_Anim(); // Pause_Mode for these objects
        }

        gos = GameObject.FindGameObjectsWithTag("Hole_Multi"); // Find all game objects with tag Hole_Multi
        foreach (var go in gos) go.GetComponent<MultiBall>().F_Pause(); // Pause_Mode for these objects
        gos = GameObject.FindGameObjectsWithTag("Hole"); // Find all game objects with tag Hole
        foreach (var go in gos) go.GetComponent<Hole>().F_Pause(); // Pause_Mode for these objects

        gos = GameObject.FindGameObjectsWithTag("spinner"); // Find all game objects with tag spinner
        foreach (var go in gos)
        {
            if (b_Pause)
                go.GetComponent<Spinner_Rotation>().F_Pause_Start(); // Pause_Mode for these objects
            else
                go.GetComponent<Spinner_Rotation>().F_Pause_Stop(); // Activate spinner
        }

        if (b_Pause && sound.isPlaying)
            sound.Pause();
        else
            sound.UnPause();
    }


    public void PlayMultiLeds(int Seq_Num)
    {
        // --> Use play a global leds animation. this function is called by the mission. 			
        AnimInProgress = Seq_Num;
        //Debug.Log("ici");
        for (var i = 0; i < leds_Multi[Seq_Num].obj.Length; i++) leds_Multi[Seq_Num].manager_Led_Animation[i].Play_New_Pattern(leds_Multi[Seq_Num].num_pattern[i]);
    }

    public void Save_Best_Score()
    {
        // Check if the player have beaten the best score

        PlayerPrefs.SetInt("CurrentScore", player_Score + tmp_Bonus_Score); // Use by the script LeaderBoardSystem to know the score when game end


        if (PlayerPrefs.HasKey(BestScoreName))
        {
            // Check if PlayerPrefs(BestScoreName) exist
            if (PlayerPrefs.GetInt(BestScoreName) < player_Score + tmp_Bonus_Score)
                // Check if the player beat has beaten the best score
                PlayerPrefs.SetInt(BestScoreName, player_Score + tmp_Bonus_Score); // if true save the player_Score on PlayerPrefs(BestScoreName)
        }
        else
        {
            // Check if PlayerPrefs(BestScoreName) doesn't exist
            PlayerPrefs.SetInt(BestScoreName, player_Score + tmp_Bonus_Score); // Save the player_Score on PlayerPrefs(BestScoreName)
        }

        // Update UI with final score
        if (uiManager != null)
        {
            uiManager.UpdateCanvasScore(player_Score + tmp_Bonus_Score);
            uiManager.UpdateCanvasBestScore(PlayerPrefs.GetInt(BestScoreName));
        }
    }

    public void SelectLastButton()
    {
        // Delegate to UiManager
        if (uiManager != null)
            uiManager.SelectLastButton();
    }

    public void Shake_AddForce_ToBall(Vector3 Direction)
    {
        // --> Add force to ball when the the player shake the flipper
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Ball"); // Find all game objects with tag Ball
        foreach (var go in gos) go.GetComponent<Ball>().Ball_Shake(Direction); // Add force to the ball
    }


    public void Start_Pause_Mode(int KeepAlive)
    {
        // --> Pause all the mission unless the mission with Index = KeepAlive. this function is called by the mission. 		
        for (var i = 0; i < obj_Managers.Length; i++)
        {
            if (KeepAlive != Missions_Index[i])
                pause_Mission[i].Start_Pause_Mission();
        }
    }

    public void Stop_Pause_Mode()
    {
        // --> Stop Pause all the mission. this function is called by the mission. 
        for (var i = 0; i < obj_Managers.Length; i++) pause_Mission[i].Stop_Pause_Mission();
    }


    public void Total_Ball_Score()
    {
        player_Score += tmp_Bonus_Score;
    }

    private IEnumerator InitGameWaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        if (camera_Movement) camera_Movement.PlayIdle();
        Loop_AnimDemoPlayfield = true; // Leds animation : start the loop animation
        PlayMultiLeds(AnimDemoPlayfield);
    }

    private IEnumerator WaitToInit()
    {
        yield return new WaitForEndOfFrame();
        if (Loop_AnimDemoPlayfield) PlayMultiLeds(AnimDemoPlayfield); // Play a loop animation until the game Start. 
    }

    #endregion
}