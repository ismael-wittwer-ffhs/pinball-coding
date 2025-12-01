// ManagerGame : Description 

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region --- Nested Types ---

    [Serializable]
    public class LedsPatternMulti
    {
        #region --- Exposed Fields ---

        [FormerlySerializedAs("obj")]
        public GameObject[] Obj = new GameObject[1]; // array to connect object with the Tag "Leds_Groups","Missions" 

        [FormerlySerializedAs("num_pattern")]
        public int[] NumPattern = new int[1]; // choose the led animation you want to play. The order as obj

        [FormerlySerializedAs("manager_Led_Animation")]
        public Manager_Led_Animation[] ManagerLedAnimation = new Manager_Led_Animation[1]; // access component

        #endregion
    }

    #endregion

    #region --- Statics ---

    public static GameManager Instance { get; private set; }

    #endregion

    #region --- Exposed Fields ---

    [FormerlySerializedAs("a_BallSave")]
    public AudioClip ABallSave; // Play a sound when the player lose a ball

    [FormerlySerializedAs("a_Bonus_Screen")]
    public AudioClip ABonusScreen; // Play a sound during the bonus score 

    [FormerlySerializedAs("a_LoseBall")]
    public AudioClip ALoseBall; // Play a sound when the player lose a ball

    [FormerlySerializedAs("s_Load_Ball")]
    public AudioClip SLoadBall; // play a sound when the ball respan

    [FormerlySerializedAs("s_Tilt")]
    public AudioClip STilt; // Play this sound if mode Tilt start

    [FormerlySerializedAs("s_Warning")]
    public AudioClip SWarning; // Play this sound when player hit the table

    [FormerlySerializedAs("b_Ball_Saver")]
    public bool BBallSaver; // if true : Ball Saver is enabled

    [FormerlySerializedAs("b_ExtraBall")]
    [Header("Bonus Extra Ball")] // --> Bonus Extra Ball
    public bool BExtraBall; // if true : extra ball is enabled

    [FormerlySerializedAs("b_InsertCoin_GameStart")]
    public bool BInsertCoinGameStart; // Use when a new game start

    [FormerlySerializedAs("b_Respawn_Timer_Ball_Saver")]
    public bool BRespawnTimerBallSaver; // use to respawn the ball when ball lost

    [FormerlySerializedAs("Loop_AnimDemoPlayfield")]
    public bool LoopAnimDemoPlayfield = true; // loop global leds animations

    [Header("Bonus Ball Saver")] // --> Bonus Ball Saver
    public bool StartGameWithBallSaver; // If true : player start a new ball with BallSaver

    [Header("Tilt Mode")] // --> Tilt Mode
    public float MinTimeTilt = 1; // minimum time in seconds between two shake 

    [FormerlySerializedAs("Respawn_Timer_BallSaver")]
    public float RespawnTimerBallSaver = 2; // use to respawn the ball when ball lost


    [FormerlySerializedAs("Time_Ballout_Part_1_BallOut")]
    [Header("Ball Lost")] // --> BALL LOST (There is 3 parts : Part 1 
    public float TimeBalloutPart1BallOut = 2; // Part 1 : Ball Lost : Choose the duration of this part

    [FormerlySerializedAs("Time_Ballout_Part_2_Bonus")]
    public float TimeBalloutPart2Bonus = 2; // Part 2 : Bonus calculation : Choose the duration of this part					

    [FormerlySerializedAs("Time_Ballout_Part_3_TotalScore")]
    public float TimeBalloutPart3TotalScore = 2; // Part 3 : Next Ball or GameOver : Choose the duration of this part		

    public float TimeToWaitMulti = 3; // Time To Wait before the multi ball start 

    [FormerlySerializedAs("obj_Launcher_MultiBall")]
    [Header("Mode Multi Ball")] // --> Mode Multi Ball
    public GameObject ObjLauncherMultiBall; // Object that manage the multi-ball on playfield. Manage how the ball is ejected on playfield

    [FormerlySerializedAs("obj_Led_Ball_Saver")]
    public GameObject ObjLedBallSaver; // Connect here a Led

    [FormerlySerializedAs("obj_Led_ExtraBall")]
    public GameObject ObjLedExtraBall; // Connect here a Led 

    [FormerlySerializedAs("Deactivate_Obj")]
    public GameObject[] DeactivateObj; // Deactivate Target when multiball start

    [FormerlySerializedAs("Deactivate_Obj_WhenMultIsFinished")]
    public GameObject[] DeactivateObjWhenMultIsFinished = { }; // Deactivate Target when multiball start

    [FormerlySerializedAs("obj_Multiplier_Leds")]
    public GameObject[] ObjMultiplierLeds; // Connect here 5 Leds

    public int AnimDemoPlayfield; // Choose leds animation used when the game is over or when the scene starts
    public int BallSaverLedAnimation; // Choose Led ANimation when after a ball saved 

    [FormerlySerializedAs("Bonus_Base")]
    public int BonusBase = 100; // 

    [FormerlySerializedAs("BONUS_Global_Hit_Counter")]
    public int BonusGlobalHitCounter; // Record the number of object that hit the ball during the current ball

    public int GameOverLedAnimation; // Choose Led Animation when the player lose a ball 

    // Variables used for the gameplay
    [Header("Player Life and Score")]
    public int Life = 3; // --> Life

    [FormerlySerializedAs("Mulitplier_SuperBonus")]
    public int MulitplierSuperBonus = 1000000; // add points if multiplier = 10

    [FormerlySerializedAs("multiplier")]
    [Header("Bonus Multiplier")] // --> Bonus Multiplier (Bonus Multiplier = multiplier x Bonus_Base x BONUS_Global_Hit_Counter)
    public int Multiplier = 1; // multiplier could be x1 x2 x4 x6 x 8 x10							

    public int NewBallLedAnimation; // Choose Led ANimation when there is a new ball on plunger after ball lost 

    public int StartDuration = -1;

    // leds_Multi[0] is auto connected. leds_Multi[0] plays the first animation of each object with the Tag "Leds_Groups","Missions" or "Led_animation"
    // It is better to create your own leds pattern from leds_Multi[1]
    [FormerlySerializedAs("leds_Multi")]
    [Header("Global Leds pattern manager")] // --> Global Leds pattern manager
    public LedsPatternMulti[] LedsMulti = new LedsPatternMulti[1];

    [FormerlySerializedAs("multiBall")]
    public MultiBall MultiBall; // Access MultiBall component from obj_Launcher_MultiBall gameObject;

    [FormerlySerializedAs("spring_Launcher")]
    public SpringLauncher[] SpringLauncher; // Plunger : access component SpringLauncher

    [Header("Score is saved with this name")]
    public string BestScoreName = "BestScore";

    [FormerlySerializedAs("Txt_Game")]
    [Header("Text used during game")] // --> Text used during game
    public string[] TxtGame; // Array : All the text use by the game Manager


    [FormerlySerializedAs("ball")]
    [Header("Ball")] // --> Ball
    public Transform Ball; // Connect the ball Prefab

    #endregion

    #region --- Private Fields ---

    [Header("Audio : Sfx")] // --> Audio
    private AudioSource _sound; // access AudioSOurce omponent

    private bool _bBalloutPart1 = true;
    private bool _bBalloutPart2 = true;
    private bool _bBalloutPart3 = true;
    private bool _bGame; // True : Player start the game . False : Game is over
    private bool _bPause; // use to pause the script
    private bool _bTimerBallSaver;
    private bool _debugGame;
    private bool _lcdWaitStartGame = true; // use to switch between best score and insert coin

    private bool _missionMultiBallEnded; // use to know if the multi ball stop

    private bool _multiBall; // Mode Multi ball activate or not
    private bool _timeToWaitBeforeMultiBallStart; // prevent bug when a multi ball start after enter a hole

    //private GameObject Camera_Board;					// (auto connected) Camera with the tag "Main Camera"
    private Camera_Movement _cameraMovement; // access component Camera_Movement
    private CameraSmoothFollow _pivotCam; // access component CameraSmoothFollow. Use to avoid that the camera move too harshly when the ball respawn on the plunger  
    private ChangeSpriteRenderer _ledBallSaverRenderer; // Access ChangeSpriteRenderer component from obj_Led_Ball_Saver if a led is connected
    private ChangeSpriteRenderer _ledExtraBall; // Access ChangeSpriteRenderer component from obj_Led_ExtraBall if a led is connected
    private ChangeSpriteRenderer[] _ledMultiplierRenderer; // Access ChangeSpriteRenderer component from obj_Multiplier_Leds if leds are connected
    private float _tiltTimer; // timer to know if we need to start tilt mode
    private float _timerBallSaver = 2; // Ball Saver duration					
    private float _timerMulti = 1; // ejection time between two balls 
    private float _timerMultiBall;
    private float _tmpBalloutTime;
    private float _tmpBalloutTime2;
    private float _tmpBalloutTime3;
    private float _tmpBallSaver; // use for the ball saver timer 

    private GameObject _objSkillshotMission; // (auto connected) with the function F_Init_Skillshot_Mission(obj : GameObject)
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
    private GameObject _spawnBall; // (connected automatically) The GameObject that manage the ejection after a ball respawn


    private GameObject[] _objManagers; // (connected automatically) Array with GameObjects that manage each mission that you can find on playfield
    private GameObject[] _objTmpLedsGroups;

    private GameObject[] _objTmpMission;
    private int _animInProgress; // Use to know the name of global Leds animation is being played
    private int _ballNum; // the number of ball played by the player
    private int _bTilt; // 0 : Tilt Desactivate	 	1 : Player shakes the playfield			2 : Tilt Mode Enable  
    private int _globAnimCount; // Use to know if all the leds animation on playfield are finish
    private int _numberOfBallOnBoard; // Know the number of board. 
    private int _playerScore; // player score
    private int _reloadNumber = 3; // number of ball for the multi ball mode. Send by the mission with the function gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
    private int _tmpBonusGlobalHitCounter; // Use to calculate the score
    private int _tmpBonusScore; // Use to calculate the score
    private int _tmpCount;
    private int _tmpIndexInfo = -1; // lets you know what mission start Multi ball. Send by the mission with the function gameManager.F_Mission_MultiBall(missionIndex.F_index(),numberOfBall);
    private int _tmpLife; // tmp_Life : the number of remaining life
    private int _tmpMultiplier; // Use to calculate the score
    private int _tmpReloadNumber = 3;
    private int[] _missionsIndex; // (connected automatically) Array with the ID of all the mission you can find on playfield


    // variables used for debuging
    private ManagerInputSetting _managerInputSetting;
    private Pause_Mission[] _pauseMission; // (connected automatically) Access the Pause_Mission component for all the mission on playfield
    private PinballInputManager _inputManager;
    private UiManager _uiManager;

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
        _managerInputSetting = GetComponent<ManagerInputSetting>();
        _inputManager = PinballInputManager.Instance;
        _uiManager = UiManager.Instance;

        if (_inputManager == null) Debug.LogWarning("ManagerGame: PinballInputManager not found. Make sure it exists in the scene.");
        if (_uiManager    == null) Debug.LogWarning("ManagerGame: UiManager not found. Make sure it exists in the scene.");

        // Initialize UiManager with text array and best score name
        if (_uiManager != null)
        {
            _uiManager.Txt_Game = TxtGame;
            _uiManager.BestScoreName = BestScoreName;
        }


        var gos = GameObject.FindGameObjectsWithTag("MainCamera"); // Connect the main camera
        foreach (var go in gos)
        {
            if (go.GetComponent<Camera_Movement>())
                //Camera_Board = go_;
                _cameraMovement = go.GetComponent<Camera_Movement>();
        }


        if (ObjLauncherMultiBall) // Connect obj_Launcher_MultiBall to the script
            MultiBall = ObjLauncherMultiBall.GetComponent<MultiBall>();

        _sound = GetComponent<AudioSource>(); // Access Audiosource component

        if (_spawnBall == null) // Connect the Mission to the gameObject : "spawnBall"
            _spawnBall = GameObject.Find("Plunger_Spawn");

        gos = GameObject.FindGameObjectsWithTag("Plunger");
        SpringLauncher = new SpringLauncher[gos.Length]; // Prepare array size 

        _tmpCount = 0;


        foreach (var go in gos)
        {
            SpringLauncher[_tmpCount] = go.GetComponent<SpringLauncher>(); // access Plungers SpringLauncher component
            _tmpCount++;
        }

        gos = GameObject.FindGameObjectsWithTag("Missions"); // Find all game objects with tag Missions and put them on arrays. 
        _objManagers = new GameObject[gos.Length]; // Prepare array size
        _missionsIndex = new int[gos.Length]; // Prepare array size

        _tmpCount = 0;
        foreach (var go4 in gos)
        {
            _objManagers[_tmpCount] = go4; // Save information inside obj_Managers[] : the Mission gameObject 
            _missionsIndex[_tmpCount] = go4.GetComponent<MissionIndex>().F_index(); // Save information inside Missions_Index[] : save the index of the Mission
            _tmpCount++;
        }

        _pauseMission = new Pause_Mission[_objManagers.Length]; // Prepare array size
        for (var i = 0; i < _objManagers.Length; i++) _pauseMission[i] = _objManagers[i].GetComponent<Pause_Mission>(); // access Pause_Mission component from all object inside obj_Managers[]

        if (ObjLedExtraBall) // Init obj_Led_ExtraBall
            _ledExtraBall = ObjLedExtraBall.GetComponent<ChangeSpriteRenderer>();
        if (ObjLedBallSaver) // Init obj_Led_Ball_Saver
            _ledBallSaverRenderer = ObjLedBallSaver.GetComponent<ChangeSpriteRenderer>();


        if (ObjMultiplierLeds.Length > 0)
        {
            // Init obj_Multiplier_Leds[] and access ChangeSpriteRenderer component
            _ledMultiplierRenderer = new ChangeSpriteRenderer[ObjMultiplierLeds.Length];
            for (var i = 0; i < ObjMultiplierLeds.Length; i++) _ledMultiplierRenderer[i] = ObjMultiplierLeds[i].GetComponent<ChangeSpriteRenderer>();
        }

        if (LedsMulti[0].Obj[0] == null)
        {
            // if nothing is connected to Leds_Multi (LEDS Patterns). automaticaly create a led pattern with objects with the Tag "Leds_Groups","Missions"
            _tmpCount = 0;

            var gosT = GameObject.FindGameObjectsWithTag("Missions");
            _objTmpMission = new GameObject[gosT.Length];
            foreach (var go10 in gosT)
            {
                _objTmpMission[_tmpCount] = go10;
                _tmpCount++;
            }

            _tmpCount = 0;
            gosT = GameObject.FindGameObjectsWithTag("Leds_Groups");
            _objTmpLedsGroups = new GameObject[gosT.Length];
            foreach (var go10 in gosT)
            {
                _objTmpLedsGroups[_tmpCount] = go10;
                _tmpCount++;
            }

            _tmpCount = _objTmpMission.Length + _objTmpLedsGroups.Length;

            LedsMulti[0].Obj = new GameObject[_tmpCount];
            LedsMulti[0].NumPattern = new int[_tmpCount];

            _tmpCount = 0; //obj_Tmp_Mission.Length + obj_Tmp_Leds_Groups.Length;
            for (var i = 0; i < _objTmpMission.Length; i++)
            {
                LedsMulti[0].Obj[i] = _objTmpMission[i];
                LedsMulti[0].NumPattern[_tmpCount] = 0;
                _tmpCount++;
            }

            for (var i = 0; i < _objTmpLedsGroups.Length; i++)
            {
                LedsMulti[0].Obj[_tmpCount] = _objTmpLedsGroups[i];
                LedsMulti[0].NumPattern[_tmpCount] = 0;
                _tmpCount++;
            }
        }

        for (var j = 0; j < LedsMulti.Length; j++)
        {
            LedsMulti[j].ManagerLedAnimation = new Manager_Led_Animation[LedsMulti[j].Obj.Length];
            for (var k = 0; k < LedsMulti[j].Obj.Length; k++) LedsMulti[j].ManagerLedAnimation[k] = LedsMulti[j].Obj[k].GetComponent<Manager_Led_Animation>();
        }

        _tmpReloadNumber = _reloadNumber; // Init the number of ball for Multi player mode

        var tmp = GameObject.Find("Pivot_Cam");
        if (tmp) _pivotCam = tmp.GetComponent<CameraSmoothFollow>(); // Access Component CameraSmoothFollow from the main camera

        _debugGame = _managerInputSetting.F_Debug_Game();
        //yield WaitForEndOfFrame();
        StartCoroutine("WaitToInit");
    }

    private void Update()
    {
        // UI Navigation and global controls
        if (_inputManager != null && _uiManager != null)
        {
            // UI: Select a button if nothing is selected
            if (_uiManager.IsUIActive() && (Mathf.Abs(_inputManager.GetNavigateHorizontal()) == 1 || _inputManager.WasPlungerPressed()))
                _uiManager.SelectLastButton();

            // Pause Mode
            if (_inputManager.WasPausePressed()) F_Pause_Game();

            // Change the Camera view
            if (_inputManager.WasCameraChangePressed())
                if (_cameraMovement)
                    _cameraMovement.Selected_Cam();
        }


        if (!_bPause)
        {
            /////////////////////////////////	SECTION : Player Input : START /////////////
            // New Game Start
            if (_inputManager != null && (_uiManager == null || !_uiManager.IsUIActive()) && _inputManager.WasPlungerPressed())
                if (!_bGame)
                    F_InsertCoin_GameStart();

            if (_bTilt == 1)
            {
                // --> Player shakes the playfield	: First Time
                _tiltTimer = Mathf.MoveTowards(_tiltTimer, MinTimeTilt, Time.deltaTime); // A timer start to know the time between two shake on the table
                if (_tiltTimer == MinTimeTilt)
                {
                    // Tilt_Timer is init after  MinTimeTilt 
                    _bTilt = 0; // Init Tilt Mode
                    _tiltTimer = 0; // init Tilt timer
                }
            }

            // Shake/Tilt input handling
            if (_inputManager != null)
            {
                // --> Player shakes the playfield : Right
                if (_inputManager.WasShakeRightPressed() && _bGame)
                {
                    if (_bTilt == 1)
                    {
                        // Start The Tilt Mode
                        Start_Pause_Mode(-1);
                        if (_cameraMovement) _cameraMovement.Shake_Cam(1);
                        if (STilt) _sound.PlayOneShot(STilt);
                        if (_uiManager != null && TxtGame != null && TxtGame.Length > 0) _uiManager.Add_Info_To_Array(TxtGame[0], 3);
                        _bTilt = 2;
                        _tiltTimer = 0;
                        F_Mode_Ball_Saver_Off();
                        BExtraBall = false;
                        if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_Off();
                        Flippers_Plunger_State_Tilt_Mode("Desactivate");
                    }
                    else if (_bTilt == 0)
                    {
                        // First warning
                        if (_uiManager != null && TxtGame != null && TxtGame.Length > 1) _uiManager.Add_Info_To_Array(TxtGame[1], 1);
                        if (SWarning) _sound.PlayOneShot(SWarning);
                        if (_cameraMovement) _cameraMovement.Shake_Cam(1);
                        Shake_AddForce_ToBall(new Vector3(-1, 0, 0));
                        _bTilt = 1;
                    }
                }

                // --> Player shakes the playfield : Left
                if (_inputManager.WasShakeLeftPressed() && _bGame)
                {
                    if (_bTilt == 1)
                    {
                        Start_Pause_Mode(-1);
                        if (_cameraMovement) _cameraMovement.Shake_Cam(2);
                        if (STilt) _sound.PlayOneShot(STilt);
                        if (_uiManager != null && TxtGame != null && TxtGame.Length > 0) _uiManager.Add_Info_To_Array(TxtGame[0], 3);
                        _bTilt = 2;
                        _tiltTimer = 0;
                        F_Mode_Ball_Saver_Off();
                        BExtraBall = false;
                        if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_Off();
                        Flippers_Plunger_State_Tilt_Mode("Desactivate");
                    }
                    else if (_bTilt == 0)
                    {
                        if (_uiManager != null && TxtGame != null && TxtGame.Length > 1) _uiManager.Add_Info_To_Array(TxtGame[1], 1);
                        if (SWarning) _sound.PlayOneShot(SWarning);
                        if (_cameraMovement) _cameraMovement.Shake_Cam(2);
                        Shake_AddForce_ToBall(new Vector3(1, 0, 0));
                        _bTilt = 1;
                    }
                }

                // --> Player shakes the playfield : Up
                if (_inputManager.WasShakeUpPressed() && _bGame)
                {
                    if (_bTilt == 1)
                    {
                        Start_Pause_Mode(-1);
                        if (_cameraMovement) _cameraMovement.Shake_Cam(3);
                        if (STilt) _sound.PlayOneShot(STilt);
                        if (_uiManager != null && TxtGame != null && TxtGame.Length > 0) _uiManager.Add_Info_To_Array(TxtGame[0], 3);
                        _bTilt = 2;
                        _tiltTimer = 0;
                        F_Mode_Ball_Saver_Off();
                        BExtraBall = false;
                        if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_Off();
                        Flippers_Plunger_State_Tilt_Mode("Desactivate");
                    }
                    else if (_bTilt == 0)
                    {
                        if (SWarning) _sound.PlayOneShot(SWarning);
                        if (_uiManager != null && TxtGame != null && TxtGame.Length > 1) _uiManager.Add_Info_To_Array(TxtGame[1], 1);
                        if (_cameraMovement) _cameraMovement.Shake_Cam(3);
                        Shake_AddForce_ToBall(new Vector3(0, 0, 1));
                        _bTilt = 1;
                    }
                }
            }
            /////////////////////////////////	SECTION END /////////////

            /////////////////////////////////	SECTION : INFO : START /////////////
            // Update default LCD display when no special message is showing
            if (_uiManager != null && TxtGame != null) _uiManager.UpdateDefaultDisplay(_playerScore, _ballNum, _lcdWaitStartGame, _tmpLife, PlayerPrefs.GetInt(BestScoreName));

            /////////////////////////////////	SECTION END 	/////////////

            /////////////////////////////////	SECTION : MULTI BALL /////////////
            if (_multiBall && _numberOfBallOnBoard < 3 && _timerMulti > 1 && _tmpReloadNumber > 0)
            {
                // --> Multi ball. condition to create a new ball on playfied
                NewBall(_spawnBall.transform.position);
                _timerMulti = 0;
                _tmpReloadNumber--;
            }

            if (_multiBall)
            {
                // Wait between two balls
                _timerMulti += Time.deltaTime;

                if (_tmpReloadNumber == 0 && _timerMulti > 1) // if there is no more ball to launch
                    F_Mode_MultiBall();
            }

            /////////////////////////////////	SECTION : MULTI BALL : END	/////////////

            /////////////////////////////////	SECTION : BALL SAVER /////////////
            if (BRespawnTimerBallSaver)
            {
                // --> Ball Saver : use to respawn the ball when ball is lost and BallSaver = true
                RespawnTimerBallSaver = Mathf.MoveTowards(RespawnTimerBallSaver, 1, // Wait
                    Time.deltaTime);
                if (RespawnTimerBallSaver == 1)
                {
                    // Respan the ball
                    BRespawnTimerBallSaver = false;
                    MultiBall.KickBack_MultiOnOff();
                }
            }

            if (_bTimerBallSaver)
            {
                // --> Ball Saver Duration : Check if we need to disable ball saver
                _tmpBallSaver = Mathf.MoveTowards(_tmpBallSaver, _timerBallSaver,
                    Time.deltaTime);
                if (_timerBallSaver == _tmpBallSaver)
                {
                    // Stop the ball saver
                    _bTimerBallSaver = false;
                    _tmpBallSaver = 0;
                    F_Mode_Ball_Saver_Off();
                }
            }

            if (_timeToWaitBeforeMultiBallStart)
            {
                // Add a delay before starting multi ball
                _timerMultiBall = Mathf.MoveTowards(_timerMultiBall, TimeToWaitMulti, Time.deltaTime);
                if (_timerMultiBall == TimeToWaitMulti)
                {
                    _timerMultiBall = 0;
                    _timeToWaitBeforeMultiBallStart = false;
                    F_Mode_MultiBall(); // Start the Mode_Multi ball by calling F_Mode_MultiBall()
                }
            }
            /////////////////////////////////	SECTION : TIMER BALL SAVER : END	/////////////

            /////////////////////////////////	SECTION : END OF A BALL /////////////
            if (!_bBalloutPart1)
            {
                // --> End of a ball : 
                _bTilt = 2; // when player lose a ball. Disable Tilt Mode

                for (var i = 0; i < SpringLauncher.Length; i++) SpringLauncher[i].F_Desactivate(); // Desactivate plunger
                if (_objSkillshotMission)
                    _objSkillshotMission.SendMessage("Disable_Skillshot_Mission"); // Disable the skillshot mission

                _tmpBalloutTime = Mathf.MoveTowards(_tmpBalloutTime, TimeBalloutPart1BallOut, // Wait
                    Time.deltaTime);
                if (_tmpBalloutTime == TimeBalloutPart1BallOut)
                {
                    // Part 1
                    if (ABonusScreen)
                    {
                        // Play a sound
                        _sound.clip = ABonusScreen;
                        _sound.Play();
                    }

                    _tmpBalloutTime = 0;
                    _bBalloutPart1 = true;
                    _bBalloutPart2 = false; // start Part 2
                    Add_Info_To_Array(TxtGame[5]                             + "\n" // display a text
                                                                             + _tmpBonusGlobalHitCounter + TxtGame[6] + BonusBase
                                                                             + " x "                     + "\n"       + _tmpMultiplier + TxtGame[7], TimeBalloutPart2Bonus);
                    Total_Ball_Score(); // Add Bonus Points to player_Score
                }
            }

            if (!_bBalloutPart2)
            {
                // Part 2
                _tmpBalloutTime2 = Mathf.MoveTowards(_tmpBalloutTime2, TimeBalloutPart2Bonus, // Wait
                    Time.deltaTime);
                if (_tmpBalloutTime2 == TimeBalloutPart2Bonus)
                {
                    _tmpBalloutTime2 = 0;
                    _bBalloutPart2 = true;
                    _bBalloutPart3 = false; // start Part 3
                    Add_Info_To_Array(TxtGame[8] + "\n" + _playerScore, TimeBalloutPart3TotalScore); // display a text
                }
            }

            if (!_bBalloutPart3)
            {
                // Part 3
                _tmpBalloutTime3 = Mathf.MoveTowards(_tmpBalloutTime3, TimeBalloutPart3TotalScore, // Wait
                    Time.deltaTime);
                if (_tmpBalloutTime3 == TimeBalloutPart3TotalScore)
                {
                    _tmpBalloutTime3 = 0;
                    _bBalloutPart3 = true;
                    if (_tmpLife >= 1)
                    {
                        // If tmp_Life > 1 new ball for the player
                        PlayMultiLeds(NewBallLedAnimation);
                        NewBall(_spawnBall.transform.position);
                        if (StartGameWithBallSaver) // If StartGameWithBallSaver = true. A new ball start with BallSaver
                            F_Mode_Ball_Saver_On(StartDuration); // if value = -1 Ball saver stay enable until the player lose the ball 	
                        Add_Info_To_Array(TxtGame[9], 2); // display a text
                        _bTilt = 0; // init Tilt Mode When player lose a ball
                        if (_objSkillshotMission) // Use if you a mission to be a skillshot mission
                            _objSkillshotMission.SendMessage("Enable_Skillshot_Mission"); // The skillshot mission is enabled
                        Stop_Pause_Mode(); // Stop Pause Mode for all the missions. useful if the player loses the ball because of the tilt mode
                    }
                    else
                    {
                        // -> if GameOver
                        PlayMultiLeds(AnimDemoPlayfield); // Play a loop animation until the game Start
                        LoopAnimDemoPlayfield = true; // enable to loop global leds animations
                        _bGame = false;
                        _bTilt = 0; // init Tilt Mode When player lose a ball
                        Stop_Pause_Mode(); // Stop Pause Mode for all the missions. useful if the player loses the ball because of the tilt mode
                        if (_uiManager != null)
                            _uiManager.ShowGameOverUI();
                    }
                }
            }
            /////////////////////////////////	SECTION : END OF A BALL : END	/////////////
        }

        ////////////////////////////////	DEBUG
        if (_debugGame) Debug_Input(); // Input for debugging
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion

    #region --- Methods ---

    public void Add_Info_To_Array(string inf, float timer)
    {
        // --> Score Text : Call This function to add text to LCD screen
        if (_uiManager != null)
            _uiManager.Add_Info_To_Array(inf, timer);
    }

    public void Add_Info_To_Timer(string inf)
    {
        // --> Timer Text : Call This function to add text to timer
        if (_uiManager != null)
            _uiManager.Add_Info_To_Timer(inf);
    }

    public void Add_Score(int addScore)
    {
        // --> Call This function to add points
        _playerScore += addScore;

        if (_playerScore > 999999999) // Max score is 999,999,999 points					
            _playerScore = 999999999;

        // Update UI with new score
        if (_uiManager != null && TxtGame != null && TxtGame.Length > 3)
        {
            _uiManager.UpdateScore(_playerScore, _ballNum, _lcdWaitStartGame, _tmpLife);
            _uiManager.SetLCDWaitStartGame(_lcdWaitStartGame);
        }
    }

    public bool Ball_Saver_State()
    {
        return BBallSaver;
    }


    public void CheckGlobalAnimationEnded()
    {
        // --> Check if a global leds animation is finished. Every mission call this function when her animation is finished.  
        _globAnimCount++;
        if (_globAnimCount == LedsMulti[_animInProgress].Obj.Length - 1)
        {
            _globAnimCount = 0;
            if (LoopAnimDemoPlayfield) PlayMultiLeds(AnimDemoPlayfield);
        }
    }


    /////////////////////////////////	SECTION : Gameplay : END /////////////


    /////////////////////////////////	SECTION : Debug : START /////////////
    public void Debug_Input()
    {
        if (!_bPause)
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
        return BExtraBall;
    }

    public void F_Ball_Saver_Off()
    {
        if (!_bPause) F_Mode_Ball_Saver_Off();
    }

    public void F_Ball_Saver_On()
    {
        if (!_bPause) F_Mode_Ball_Saver_On(-1);
    }

    public void F_ExtraBall()
    {
        if (!_bPause) F_Mode_ExtraBall();
    }

    public void F_Init_All_Mission()
    {
        if (!_bPause) Init_All_Mission();
    }

    public void F_Init_Skillshot_Mission(GameObject obj)
    {
        // --> Call by the skillshot mission
        _objSkillshotMission = obj; // 
    }

    public void F_InsertCoin_GameStart()
    {
        if (!_bPause) InsertCoin_GameStart();
        if (_uiManager != null)
            _uiManager.ShowGameStartUI();
    }


    public void F_Mission_MultiBall(int indexInfo, int nbrOfBall)
    {
        // Use for Mission
        _reloadNumber = nbrOfBall; // Choose the number of balls of you want for the multi ball
        _tmpReloadNumber = _reloadNumber; // init var tmp_ReloadNumber. the variable is used to determine how much ball remains before the multi-ball end.
        _tmpIndexInfo = indexInfo; // Save the mission that start the multi-ball
        if (_cameraMovement) _cameraMovement.Camera_MultiBall_Start(); // Change the Camera view To Camera 4
        _timeToWaitBeforeMultiBallStart = true; // add delay to start Multi-Ball. Use to prevent bug when multi ball after enter a hole. You need to add a delay to be sure multi ball start after the ball respawn from the hole
        if (DeactivateObj.Length > 0) // activate Object after multiball. Only work with drop target
            for (var i = 0; i < DeactivateObj.Length; i++)
                DeactivateObj[i].SendMessage("Activate_Object");

        if (DeactivateObjWhenMultIsFinished.Length > 0) // activate Object after multiball. Only work with drop target
            for (var i = 0; i < DeactivateObjWhenMultIsFinished.Length; i++)
                DeactivateObjWhenMultIsFinished[i].SendMessage("Activate_Object");
    }

    public void F_Mode_Ball_Saver_Off()
    {
        // Use for Mission. Disabled Ball saver
        _timerBallSaver = 0; // Init				
        _tmpBallSaver = 0; // Init 																	
        _bTimerBallSaver = false; // init
        BBallSaver = false; // Ball Saver is disable
        if (ObjLedBallSaver) _ledBallSaverRenderer.F_ChangeSprite_Off(); // Switch Off a light on the playfield
    }


    public void F_Mode_Ball_Saver_On(int value)
    {
        // Use for Mission. Enabled ball saver
        _timerBallSaver = value; // Choose the duration of the Ball Saver				
        _tmpBallSaver = 0; // Init saver timer 
        if (value != -1) // if value = -1 Ball saver stay enable until the player lose the ball 								
            _bTimerBallSaver = true; // Ball Saver timer start

        BBallSaver = true; // Ball Saver is enable
        if (ObjLedBallSaver) _ledBallSaverRenderer.F_ChangeSprite_On(); // Switch On a light on the playfield
    }

    public void F_Mode_BONUS_Counter()
    {
        // --> BONUS
        BonusGlobalHitCounter++;
    }

    public void F_Mode_ExtraBall()
    {
        // Use for Mission
        BExtraBall = true;
        if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_On();
    }


    public void F_Mode_MultiBall()
    {
        // --> MULTI BALL
        if (_multiBall)
        {
            //if(ball_Follow)ball_Follow.Start_BallFollow();											
            _tmpReloadNumber = _reloadNumber; // Muli Ball Stop
            _multiBall = false;
            MultiBall.KickBack_MultiOnOff();
            _missionMultiBallEnded = true;

            if (DeactivateObj.Length > 0) // Deactivate Object after multiball. Only work with drop target
                for (var i = 0; i < DeactivateObj.Length; i++)
                    DeactivateObj[i].SendMessage("Desactivate_Object");
        }
        else
        {
            //if(ball_Follow)ball_Follow.Stop_BallFollow();
            _multiBall = true; // Muli Ball Start
            MultiBall.KickBack_MultiOnOff();
        }
    }

    public void F_MultiBall()
    {
        if (!_bPause) F_Mode_MultiBall();
    }

    // Initialize multiplier when a ball is lost
    public void F_Multiplier()
    {
        // --> MULTIPLIER : use to multiply bonus score at the end of ball. Multiplier is initialize when the ball is lost. 
        if (Multiplier == 1)
        {
            // multiplier increase  : x2 x4 x6 x8 x10 then Mulitplier_SuperBonus
            Multiplier = 2;
            if (ObjMultiplierLeds.Length > 0) _ledMultiplierRenderer[0].F_ChangeSprite_On();
        }
        else if (Multiplier < 10)
        {
            // Max multiplier is 10
            var valueTmp = Multiplier * .5f;
            if (ObjMultiplierLeds.Length > 0) _ledMultiplierRenderer[(int)valueTmp].F_ChangeSprite_On();
            Multiplier += 2;
        }
        else if (Multiplier >= 10)
        {
            Add_Score(MulitplierSuperBonus);
            if (Multiplier == 10) Multiplier += 2;
        }
    }


    public void F_NewBall()
    {
        if (!_bPause) NewBall(_spawnBall.transform.position);
    }


    public void F_Pause_Game()
    {
        if (_uiManager != null && _bGame)
        {
            // If UI connected and game start
            Pause_Game(); // Toggle pause state
            _uiManager.TogglePauseUI(_bPause, _bGame);
        }
        else
        {
            // If no UI connected
            Pause_Game();
        }
    }

    public void F_PlayMultiLeds()
    {
        if (!_bPause) PlayMultiLeds(0);
    }

    public void F_Quit_No()
    {
        // Stop pause
        Pause_Game();
        if (_uiManager != null)
            _uiManager.ShowQuitNoUI();
    }

    public void F_Quit_Yes()
    {
        // return to the main menu	
        InitGame_GoToMainMenu();
        if (_uiManager != null)
            _uiManager.ShowQuitYesUI();
    }

    public int F_return_Mulitplier_SuperBonus()
    {
        return MulitplierSuperBonus;
    }

    public int F_return_multiplier()
    {
        return Multiplier;
    }

    public void F_Start_Pause_Mode()
    {
        if (!_bPause) Start_Pause_Mode(-1);
    }

    public void F_Stop_Pause_Mode()
    {
        if (!_bPause) Stop_Pause_Mode();
    }

    public void Flippers_Plunger_State_Tilt_Mode(string state)
    {
        // --> Activate or Desactivate the flipper and Plunger when table is tilted
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Flipper"); // Find all game objects with tag Flipper
        foreach (var go in gos)
        {
            if (state == "Activate")
                go.GetComponent<Flippers>().F_Activate(); //  Activate Flippers
            else
                go.GetComponent<Flippers>().F_Desactivate(); // Desactivate Flippers
        }

        gos = GameObject.FindGameObjectsWithTag("Plunger"); // Find all game objects with tag Flipper
        foreach (var go in gos)
        {
            if (state == "Activate")
                go.GetComponent<SpringLauncher>().F_Activate_After_Tilt(); //  Activate plunger
            else
                go.GetComponent<SpringLauncher>().Tilt_Mode(); // Desactivate plunger					
        }
    }
    /////////////////////////////////	SECTION : Put the Game on PAUSE MODE : END /////////////


    /////////////////////////////////	SECTION : Gameplay : START /////////////
    public void GamePlay(GameObject other)
    {
        // -->  The player lose a ball. Call by the script Pinball_TriggerForBall.js on object Out_Hole_TriggerDestroyBall on the hierarchy. This object detect when a ball is lost										
        Destroy(other); // Destroy the ball
        if (_pivotCam) _pivotCam.ChangeSmoothTimeWhenBallIsLost(); //  It avoids that the camera move too harshly when the ball respawn on the plunger  
        _numberOfBallOnBoard--; // Number_Of_Ball_On_Board -1

        if (_missionMultiBallEnded && _numberOfBallOnBoard == 1)
        {
            // Condition to stop Multi Ball : Mission_Multi_Ball_Ended && Number_Of_Ball_On_Board == 1
            if (_cameraMovement) _cameraMovement.Camera_MultiBall_Stop(); // change the camera view
            for (var i = 0; i < _missionsIndex.Length; i++)
            {
                if (_missionsIndex[i] == _tmpIndexInfo && _tmpIndexInfo != -1)
                    _objManagers[i].SendMessage("Mode_MultiBall_Ended"); // Stop multi ball	
            }

            _missionMultiBallEnded = false;
        }

        if (BBallSaver && _numberOfBallOnBoard == 0)
        {
            // --> Condition to Activate Ball saver
            PlayMultiLeds(BallSaverLedAnimation);
            MultiBall.KickBack_MultiOnOff();
            NewBall(_spawnBall.transform.position);
            Add_Info_To_Array(TxtGame[10], 3);
            RespawnTimerBallSaver = 0;
            BRespawnTimerBallSaver = true;
            BBallSaver = false;
            if (ObjLedBallSaver) _ledBallSaverRenderer.F_ChangeSprite_Off();

            if (ABallSave)
            {
                _sound.clip = ABallSave;
                _sound.Play();
            }
        }
        else if (BExtraBall && _numberOfBallOnBoard == 0)
        {
            // --> Condition to Activate Extra ball
            Add_Info_To_Array(TxtGame[11], 3);
            NewBall(_spawnBall.transform.position);
            BExtraBall = false;
            if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_Off();
        }
        else if (_tmpLife > 1 && _numberOfBallOnBoard == 0)
        {
            // --> New Ball
            Add_Info_To_Array(TxtGame[12], 3);
            _bBalloutPart1 = false;
            if (ALoseBall)
            {
                _sound.clip = ALoseBall;
                _sound.Play();
            }

            init_Param_After_Ball_Lost();
            _tmpLife--;
            var gos = GameObject.FindGameObjectsWithTag("Missions");
            foreach (var go in gos) go.SendMessage("Mission_Intialisation_AfterBallLost"); // --> Init missions only if the mission must init after player lose the ball
            PlayMultiLeds(GameOverLedAnimation);
            _ballNum++;
        }
        else if (_numberOfBallOnBoard <= 0)
        {
            // --> Game Over
            // GAME OVER
            if (_uiManager != null) _uiManager.ClearBallInfo(); // We don't want to display the ball number when there is player information on LCD Fake Screen
            Add_Info_To_Array(TxtGame[13], 3);
            _bBalloutPart1 = false;
            if (ALoseBall)
            {
                _sound.clip = ALoseBall;
                _sound.Play();
            }

            init_Param_After_Ball_Lost();
            _tmpLife--;
            Init_All_Mission();
            PlayMultiLeds(GameOverLedAnimation);
            Save_Best_Score(); // Check if the player have beaten the best score
        }
    }

    public int HowManyAnimation()
    {
        return LedsMulti.Length;
    }

    public void Init_All_Mission()
    {
        // --> Initialize all the mission

        var gos = GameObject.FindGameObjectsWithTag("Missions"); // Find all game objects with tag Missions
        foreach (var go in gos)
        {
            go.SendMessage("Mission_Intialisation_StartGame"); // Init missions when the game start
            if (_tmpLife == 0 || BInsertCoinGameStart)
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
        if (Multiplier > 10) Multiplier = 10;
        _tmpBonusScore = BonusGlobalHitCounter * BonusBase * Multiplier; // Calculate Bonus_Score
        _tmpMultiplier = Multiplier;
        _tmpBonusGlobalHitCounter = BonusGlobalHitCounter;
        BonusGlobalHitCounter = 0;
        Multiplier = 1;
        if (ObjMultiplierLeds.Length > 0)
            for (var i = 0; i < _ledMultiplierRenderer.Length; i++)
                _ledMultiplierRenderer[i].F_ChangeSprite_Off(); // Switch off Multiplier Leds

        Flippers_Plunger_State_Tilt_Mode("Activate"); // Usefull when the main playfield is tilted. Reactivate the flippers and plunger after Tilt Mode
    }

    public void init_Param_After_Game_Over()
    {
        // --> INIT : init_Param_After_Ball_Lost 

        _multiBall = false;
        _missionMultiBallEnded = false;
        BExtraBall = false;
        BBallSaver = false;
        BRespawnTimerBallSaver = false;
        _tmpBallSaver = 0;
        _bTimerBallSaver = false;

        if (DeactivateObj.Length > 0) // Deactivate Deactivate_Obj.
            for (var i = 0; i < DeactivateObj.Length; i++)
                DeactivateObj[i].SendMessage("Desactivate_Object");

        if (DeactivateObjWhenMultIsFinished.Length > 0) // Deactivate Deactivate_Obj.
            for (var i = 0; i < DeactivateObjWhenMultIsFinished.Length; i++)
                DeactivateObjWhenMultIsFinished[i].SendMessage("Desactivate_Object");


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

        _bGame = false; // Game could Start
        BInsertCoinGameStart = true;
        if (_bPause) // Stop Pause
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


        _tmpLife = 0; // Init Life
        _playerScore = 0; // Init Score
        _ballNum = 0;
        _numberOfBallOnBoard = 0;
        _tmpBalloutTime = 0;
        _bBalloutPart1 = true;
        _tmpBalloutTime2 = 0;
        _bBalloutPart2 = true;
        _tmpBalloutTime3 = 0;
        _bBalloutPart3 = true;
        BonusGlobalHitCounter = 0;
        Multiplier = 1;
        init_Param_After_Ball_Lost();
        init_Param_After_Game_Over();

        if (_objSkillshotMission) // Use if you a mission to be a skillshot mission
            _objSkillshotMission.SendMessage("Disable_Skillshot_Mission"); // The skillshot mission is enabled

        _multiBall = false; // init Multi Ball
        _tmpReloadNumber = 0;
        _timerMulti = 2;
        if (_cameraMovement) _cameraMovement.Camera_MultiBall_Stop(); // change the camera view
        for (var i = 0; i < _missionsIndex.Length; i++)
        {
            if (_missionsIndex[i] == _tmpIndexInfo && _tmpIndexInfo != -1)
                _objManagers[i].SendMessage("Mode_MultiBall_Ended"); // Stop multi ball	
        }

        _missionMultiBallEnded = false;

        //if(spawnBall != null)newBall(spawnBall.transform.position);	
        BInsertCoinGameStart = false;

        Add_Info_To_Array(TxtGame[4], 3);
        _lcdWaitStartGame = true;
        //if(Gui_Txt_Score)Gui_Txt_Score.text =  Txt_Game[4] + "\n" + "<size=17>" + Txt_Game[16] + PlayerPrefs.GetInt(BestScoreName).ToString() + "</size>";


        StartCoroutine("InitGameWaitForFrame");
    }

    public void InitObjAfterMultiball()
    {
        // Deactivate Deactivate_Obj_WhenMultIsFinished when multiball mission is finished
        //Debug.Log("Here");
        if (DeactivateObjWhenMultIsFinished.Length > 0) // Deactivate Deactivate_Obj.
            for (var i = 0; i < DeactivateObjWhenMultIsFinished.Length; i++)
                DeactivateObjWhenMultIsFinished[i].SendMessage("Desactivate_Object");
    }

    public void InsertCoin_GameStart()
    {
        // --> Insert Coin : Initialisation when Game Start
        _lcdWaitStartGame = false;
        LoopAnimDemoPlayfield = false; // Leds animation : Stop the loop animation
        Add_Info_To_Array(TxtGame[14], 3);
        _bGame = true; // Game Start
        BInsertCoinGameStart = true;
        if (_bPause) // Stop Pause
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

        _tmpLife = Life; // Init Life
        _playerScore = 0; // Init Score
        _ballNum = 0;
        _numberOfBallOnBoard = 0;
        _tmpBalloutTime = 0;
        _bBalloutPart1 = true;
        _tmpBalloutTime2 = 0;
        _bBalloutPart2 = true;
        _tmpBalloutTime3 = 0;
        _bBalloutPart3 = true;
        BonusGlobalHitCounter = 0;
        Multiplier = 1;
        init_Param_After_Ball_Lost();
        init_Param_After_Game_Over();

        if (_objSkillshotMission) // Use if you a mission to be a skillshot mission
            _objSkillshotMission.SendMessage("Enable_Skillshot_Mission"); // The skillshot mission is enabled

        if (_spawnBall != null) NewBall(_spawnBall.transform.position);
        PlayMultiLeds(NewBallLedAnimation);
        BInsertCoinGameStart = false;
    }

    public void NewBall(Vector3 pos)
    {
        // --> NEW BALL . Create a ball on playfield
        if (SLoadBall) _sound.PlayOneShot(SLoadBall);
        Instantiate(Ball, pos, Quaternion.identity);
        _numberOfBallOnBoard++;
    }

    public void NewValueForUi(int value)
    {
        if (_uiManager != null)
            _uiManager.NewValueForUi(value);
    }


    /////////////////////////////////	SECTION : Put the Game on PAUSE MODE : START /////////////
    public void Pause_Game()
    {
        if (!_bPause) _bPause = true; // Pause ManagerGame 
        else _bPause = false;

        GameObject[] gos;


        gos = GameObject.FindGameObjectsWithTag("MainCamera"); // Find all game objects with tag MainCamera
        foreach (var go in gos)
        {
            if (_bPause)
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
            if (_bPause)
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
            if (_bPause)
                go.GetComponent<Flippers>().F_Pause_Start(); // Desactivate Flippers
            else
                go.GetComponent<Flippers>().F_Pause_Stop(); // Activate Flippers
        }

        gos = GameObject.FindGameObjectsWithTag("Plunger"); // Find all game objects with tag Plunger
        foreach (var go in gos)
        {
            if (_bPause)
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
            if (_bPause)
                go.GetComponent<Spinner_Rotation>().F_Pause_Start(); // Pause_Mode for these objects
            else
                go.GetComponent<Spinner_Rotation>().F_Pause_Stop(); // Activate spinner
        }

        if (_bPause && _sound.isPlaying)
            _sound.Pause();
        else
            _sound.UnPause();
    }


    public void PlayMultiLeds(int seqNum)
    {
        // --> Use play a global leds animation. this function is called by the mission. 			
        _animInProgress = seqNum;
        //Debug.Log("ici");
        for (var i = 0; i < LedsMulti[seqNum].Obj.Length; i++) LedsMulti[seqNum].ManagerLedAnimation[i].Play_New_Pattern(LedsMulti[seqNum].NumPattern[i]);
    }

    public void Save_Best_Score()
    {
        // Check if the player have beaten the best score

        PlayerPrefs.SetInt("CurrentScore", _playerScore + _tmpBonusScore); // Use by the script LeaderBoardSystem to know the score when game end


        if (PlayerPrefs.HasKey(BestScoreName))
        {
            // Check if PlayerPrefs(BestScoreName) exist
            if (PlayerPrefs.GetInt(BestScoreName) < _playerScore + _tmpBonusScore)
                // Check if the player beat has beaten the best score
                PlayerPrefs.SetInt(BestScoreName, _playerScore + _tmpBonusScore); // if true save the player_Score on PlayerPrefs(BestScoreName)
        }
        else
        {
            // Check if PlayerPrefs(BestScoreName) doesn't exist
            PlayerPrefs.SetInt(BestScoreName, _playerScore + _tmpBonusScore); // Save the player_Score on PlayerPrefs(BestScoreName)
        }

        // Update UI with final score
        if (_uiManager != null)
        {
            _uiManager.UpdateCanvasScore(_playerScore + _tmpBonusScore);
            _uiManager.UpdateCanvasBestScore(PlayerPrefs.GetInt(BestScoreName));
        }
    }

    public void SelectLastButton()
    {
        // Delegate to UiManager
        if (_uiManager != null)
            _uiManager.SelectLastButton();
    }

    public void Shake_AddForce_ToBall(Vector3 direction)
    {
        // --> Add force to ball when the the player shake the flipper
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Ball"); // Find all game objects with tag Ball
        foreach (var go in gos) go.GetComponent<Ball>().Ball_Shake(direction); // Add force to the ball
    }


    public void Start_Pause_Mode(int keepAlive)
    {
        // --> Pause all the mission unless the mission with Index = KeepAlive. this function is called by the mission. 		
        for (var i = 0; i < _objManagers.Length; i++)
        {
            if (keepAlive != _missionsIndex[i])
                _pauseMission[i].Start_Pause_Mission();
        }
    }

    public void Stop_Pause_Mode()
    {
        // --> Stop Pause all the mission. this function is called by the mission. 
        for (var i = 0; i < _objManagers.Length; i++) _pauseMission[i].Stop_Pause_Mission();
    }


    public void Total_Ball_Score()
    {
        _playerScore += _tmpBonusScore;
    }

    private IEnumerator InitGameWaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        if (_cameraMovement) _cameraMovement.PlayIdle();
        LoopAnimDemoPlayfield = true; // Leds animation : start the loop animation
        PlayMultiLeds(AnimDemoPlayfield);
    }

    private IEnumerator WaitToInit()
    {
        yield return new WaitForEndOfFrame();
        if (LoopAnimDemoPlayfield) PlayMultiLeds(AnimDemoPlayfield); // Play a loop animation until the game Start. 
    }

    #endregion
}