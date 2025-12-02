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
        public GameObject[] Obj = new GameObject[1];

        [FormerlySerializedAs("num_pattern")]
        public int[] NumPattern = new int[1];

        [FormerlySerializedAs("manager_Led_Animation")]
        public Manager_Led_Animation[] ManagerLedAnimation = new Manager_Led_Animation[1];

        #endregion
    }

    #endregion

    #region --- Statics ---

    public static GameManager Instance { get; private set; }

    #endregion

    #region --- Exposed Fields ---

    [Header("Audio : Sfx")]
    [FormerlySerializedAs("a_BallSave")]
    public AudioClip ABallSave;

    [FormerlySerializedAs("a_Bonus_Screen")]
    public AudioClip ABonusScreen;

    [FormerlySerializedAs("a_LoseBall")]
    public AudioClip ALoseBall;

    [FormerlySerializedAs("s_Load_Ball")]
    public AudioClip SLoadBall;

    [FormerlySerializedAs("s_Tilt")]
    public AudioClip STilt;

    [FormerlySerializedAs("s_Warning")]
    public AudioClip SWarning;

    [FormerlySerializedAs("b_Ball_Saver")]
    public bool BBallSaver;

    [FormerlySerializedAs("b_ExtraBall")]
    [Header("Bonus Extra Ball")]
    public bool BExtraBall;

    [FormerlySerializedAs("b_InsertCoin_GameStart")]
    public bool BInsertCoinGameStart;

    [FormerlySerializedAs("b_Respawn_Timer_Ball_Saver")]
    public bool BRespawnTimerBallSaver;

    [FormerlySerializedAs("Loop_AnimDemoPlayfield")]
    public bool LoopAnimDemoPlayfield = true;

    [Header("Bonus Ball Saver")]
    public bool StartGameWithBallSaver;

    [Header("Tilt Mode")]
    public float MinTimeTilt = 1;

    [FormerlySerializedAs("Respawn_Timer_BallSaver")]
    public float RespawnTimerBallSaver = 2;

    [FormerlySerializedAs("Time_Ballout_Part_1_BallOut")]
    [Header("Ball Lost")]
    public float TimeBalloutPart1BallOut = 2;

    [FormerlySerializedAs("Time_Ballout_Part_2_Bonus")]
    public float TimeBalloutPart2Bonus = 2;

    [FormerlySerializedAs("Time_Ballout_Part_3_TotalScore")]
    public float TimeBalloutPart3TotalScore = 2;

    public float TimeToWaitMulti = 3;

    [FormerlySerializedAs("obj_Launcher_MultiBall")]
    [Header("Mode Multi Ball")]
    public GameObject ObjLauncherMultiBall;

    [FormerlySerializedAs("obj_Led_Ball_Saver")]
    public GameObject ObjLedBallSaver;

    [FormerlySerializedAs("obj_Led_ExtraBall")]
    public GameObject ObjLedExtraBall;

    [FormerlySerializedAs("Deactivate_Obj")]
    public GameObject[] DeactivateObj;

    [FormerlySerializedAs("Deactivate_Obj_WhenMultIsFinished")]
    public GameObject[] DeactivateObjWhenMultIsFinished = { };

    [FormerlySerializedAs("obj_Multiplier_Leds")]
    public GameObject[] ObjMultiplierLeds;

    public int AnimDemoPlayfield;
    public int BallSaverLedAnimation;

    [FormerlySerializedAs("Bonus_Base")]
    public int BonusBase = 100;

    [FormerlySerializedAs("BONUS_Global_Hit_Counter")]
    public int BonusGlobalHitCounter;

    public int GameOverLedAnimation;

    [Header("Player Life and Score")]
    public int Life = 3;

    [FormerlySerializedAs("Mulitplier_SuperBonus")]
    public int MulitplierSuperBonus = 1000000;

    [FormerlySerializedAs("multiplier")]
    [Header("Bonus Multiplier")]
    public int Multiplier = 1;

    public int NewBallLedAnimation;
    public int StartDuration = -1;

    [FormerlySerializedAs("leds_Multi")]
    [Header("Global Leds pattern manager")]
    public LedsPatternMulti[] LedsMulti = new LedsPatternMulti[1];

    [FormerlySerializedAs("multiBall")]
    public MultiBall MultiBall;

    [FormerlySerializedAs("spring_Launcher")]
    public SpringLauncher[] SpringLauncher;


    [FormerlySerializedAs("ball")]
    [Header("Ball")]
    public Transform Ball;

    #endregion

    #region --- Private Fields ---

    private AudioSource _sound;

    private bool _bBalloutPart1 = true;
    private bool _bBalloutPart2 = true;
    private bool _bBalloutPart3 = true;
    private bool _bGame;
    private bool _bTimerBallSaver;
    private bool _debugGame;
    private bool _gamePaused;
    private bool _lcdWaitStartGame = true;
    private bool _missionMultiBallEnded;
    private bool _multiBall;
    private bool _timeToWaitBeforeMultiBallStart;

    private Camera_Movement _cameraMovement;
    private CameraSmoothFollow _pivotCam;
    private ChangeSpriteRenderer _ledBallSaverRenderer;
    private ChangeSpriteRenderer _ledExtraBall;
    private ChangeSpriteRenderer[] _ledMultiplierRenderer;

    private float _tiltTimer;
    private float _timerBallSaver = 2;
    private float _timerMulti = 1;
    private float _timerMultiBall;
    private float _tmpBalloutTime;
    private float _tmpBalloutTime2;
    private float _tmpBalloutTime3;
    private float _tmpBallSaver;

    private GameObject _objSkillshotMission;
    private GameObject _spawnBall;
    private GameObject[] _objManagers;
    private GameObject[] _objTmpLedsGroups;
    private GameObject[] _objTmpMission;

    private int _animInProgress;
    private int _ballNum;
    private int _bTilt;
    private int _globAnimCount;
    private int _numberOfBallOnBoard;
    private int _playerScore;
    private int _reloadNumber = 3;
    private int _tmpBonusGlobalHitCounter;
    private int _tmpBonusScore;
    private int _tmpCount;
    private int _tmpIndexInfo = -1;
    private int _tmpLife;
    private int _tmpMultiplier;
    private int _tmpReloadNumber = 3;
    private int[] _missionsIndex;

    private ManagerInputSetting _managerInputSetting;
    private Pause_Mission[] _pauseMission;
    private PinballInputManager _inputManager;
    private UiManager _uiManager;

    #endregion

    #region --- Static and Constant Fields ---

    private const string HIGHSCORE_NAME = "BestScore"; // Score is saved with this name

    #endregion

    #region --- Unity Methods ---

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeManagers();
        InitializeCamera();
        InitializeComponents();
        InitializeLeds();
        InitializeLedsPatterns();

        _tmpReloadNumber = _reloadNumber;

        var pivotCamObj = GameObject.Find("Pivot_Cam");
        if (pivotCamObj) _pivotCam = pivotCamObj.GetComponent<CameraSmoothFollow>();

        _debugGame = _managerInputSetting.F_Debug_Game();
        StartCoroutine(WaitToInit());
    }

    private void Update()
    {
        HandleUINavigation();

        if (!_gamePaused)
        {
            HandleGameStart();
            HandleTiltTimer();
            HandleShakeInput();
            UpdateDefaultDisplay();
            HandleMultiBall();
            HandleBallSaver();
            HandleBallOutSequence();
        }

        if (_debugGame) Debug_Input();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion

    #region --- Methods ---

    public void Add_Info_To_Array(string inf, float timer)
    {
        if (_uiManager != null)
            _uiManager.Add_Info_To_Array(inf, timer);
    }

    public void Add_Info_To_Timer(string inf)
    {
        if (_uiManager != null)
            _uiManager.Add_Info_To_Timer(inf);
    }

    public void Add_Score(int addScore)
    {
        _playerScore += addScore;

        if (_playerScore > 999999999)
            _playerScore = 999999999;

        if (_uiManager != null)
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
        _globAnimCount++;
        if (_globAnimCount == LedsMulti[_animInProgress].Obj.Length - 1)
        {
            _globAnimCount = 0;
            if (LoopAnimDemoPlayfield) PlayMultiLeds(AnimDemoPlayfield);
        }
    }

    public void Debug_Input()
    {
        // Debug input placeholder - enable debug functionality as needed
    }

    public bool ExtraBall_State()
    {
        return BExtraBall;
    }

    public void F_Init_Skillshot_Mission(GameObject obj)
    {
        _objSkillshotMission = obj;
    }

    public void F_InsertCoin_GameStart()
    {
        if (!_gamePaused) InsertCoin_GameStart();
        if (_uiManager != null)
            _uiManager.ShowGameStartUI();
    }

    public void F_Mission_MultiBall(int indexInfo, int nbrOfBall)
    {
        _reloadNumber = nbrOfBall;
        _tmpReloadNumber = _reloadNumber;
        _tmpIndexInfo = indexInfo;
        if (_cameraMovement) _cameraMovement.Camera_MultiBall_Start();
        _timeToWaitBeforeMultiBallStart = true;

        ActivateDeactivateObjects(DeactivateObj, true);
        ActivateDeactivateObjects(DeactivateObjWhenMultIsFinished, true);
    }

    public void F_Mode_Ball_Saver_Off()
    {
        _timerBallSaver = 0;
        _tmpBallSaver = 0;
        _bTimerBallSaver = false;
        BBallSaver = false;
        if (ObjLedBallSaver) _ledBallSaverRenderer.F_ChangeSprite_Off();
    }

    public void F_Mode_Ball_Saver_On(int value)
    {
        _timerBallSaver = value;
        _tmpBallSaver = 0;
        if (value != -1)
            _bTimerBallSaver = true;

        BBallSaver = true;
        if (ObjLedBallSaver) _ledBallSaverRenderer.F_ChangeSprite_On();
    }

    public void F_Mode_BONUS_Counter()
    {
        BonusGlobalHitCounter++;
    }

    public void F_Mode_ExtraBall()
    {
        BExtraBall = true;
        if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_On();
    }

    public void F_Mode_MultiBall()
    {
        if (_multiBall)
        {
            _tmpReloadNumber = _reloadNumber;
            _multiBall = false;
            MultiBall.KickBack_MultiOnOff();
            _missionMultiBallEnded = true;
            ActivateDeactivateObjects(DeactivateObj, false);
        }
        else
        {
            _multiBall = true;
            MultiBall.KickBack_MultiOnOff();
        }
    }

    public void F_Multiplier()
    {
        if (Multiplier == 1)
        {
            Multiplier = 2;
            if (ObjMultiplierLeds.Length > 0) _ledMultiplierRenderer[0].F_ChangeSprite_On();
        }
        else if (Multiplier < 10)
        {
            var valueTmp = (int)(Multiplier * 0.5f);
            if (ObjMultiplierLeds.Length > 0) _ledMultiplierRenderer[valueTmp].F_ChangeSprite_On();
            Multiplier += 2;
        }
        else if (Multiplier >= 10)
        {
            Add_Score(MulitplierSuperBonus);
            if (Multiplier == 10) Multiplier += 2;
        }
    }

    // Helper for debug/UI functions
    public void F_NewBall()
    {
        if (_spawnBall != null)
            NewBall(_spawnBall.transform.position);
    }

    public void F_Pause_Game()
    {
        if (_uiManager != null && _bGame)
        {
            Pause_Game();
            _uiManager.TogglePauseUI(_gamePaused, _bGame);
        }
        else
        {
            Pause_Game();
        }
    }

    public void F_Quit_No()
    {
        Pause_Game();
        if (_uiManager != null)
            _uiManager.ShowQuitNoUI();
    }

    public int F_return_Mulitplier_SuperBonus()
    {
        return MulitplierSuperBonus;
    }

    public int F_return_multiplier()
    {
        return Multiplier;
    }

    public void Flippers_Plunger_State_Tilt_Mode(string state)
    {
        var activate = state == "Activate";

        var flippers = GameObject.FindGameObjectsWithTag("Flipper");
        foreach (var flipper in flippers)
        {
            var component = flipper.GetComponent<Flippers>();
            if (activate)
                component.F_Activate();
            else
                component.F_Desactivate();
        }

        var plungers = GameObject.FindGameObjectsWithTag("Plunger");
        foreach (var plunger in plungers)
        {
            var component = plunger.GetComponent<SpringLauncher>();
            if (activate)
                component.F_Activate_After_Tilt();
            else
                component.Tilt_Mode();
        }
    }

    public void GamePlay(GameObject other)
    {
        Destroy(other);
        if (_pivotCam) _pivotCam.ChangeSmoothTimeWhenBallIsLost();
        _numberOfBallOnBoard--;

        if (_missionMultiBallEnded && _numberOfBallOnBoard == 1)
        {
            if (_cameraMovement) _cameraMovement.Camera_MultiBall_Stop();
            NotifyMissionMultiBallEnded();
            _missionMultiBallEnded = false;
        }

        ProcessBallLost();
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGHSCORE_NAME);
    }

    public int HowManyAnimation()
    {
        return LedsMulti.Length;
    }

    public void Init_All_Mission()
    {
        var missions = GameObject.FindGameObjectsWithTag("Missions");
        foreach (var mission in missions)
        {
            mission.SendMessage("Mission_Intialisation_StartGame");
            if (_tmpLife == 0 || BInsertCoinGameStart)
            {
                mission.SendMessage("InitLedMission");
                init_Param_After_Game_Over();
            }
        }

        Stop_Pause_Mode();
    }

    public void init_Param_After_Ball_Lost()
    {
        if (Multiplier > 10) Multiplier = 10;
        _tmpBonusScore = BonusGlobalHitCounter * BonusBase * Multiplier;
        _tmpMultiplier = Multiplier;
        _tmpBonusGlobalHitCounter = BonusGlobalHitCounter;
        BonusGlobalHitCounter = 0;
        Multiplier = 1;

        if (ObjMultiplierLeds.Length > 0)
            for (var i = 0; i < _ledMultiplierRenderer.Length; i++)
                _ledMultiplierRenderer[i].F_ChangeSprite_Off();

        Flippers_Plunger_State_Tilt_Mode("Activate");
    }

    public void init_Param_After_Game_Over()
    {
        _multiBall = false;
        _missionMultiBallEnded = false;
        BExtraBall = false;
        BBallSaver = false;
        BRespawnTimerBallSaver = false;
        _tmpBallSaver = 0;
        _bTimerBallSaver = false;

        ActivateDeactivateObjects(DeactivateObj, false);
        ActivateDeactivateObjects(DeactivateObjWhenMultIsFinished, false);

        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var ball in balls) Destroy(ball);

        if (StartGameWithBallSaver)
            F_Mode_Ball_Saver_On(StartDuration);

        Flippers_Plunger_State_Tilt_Mode("Activate");
    }

    public void InitGame_GoToMainMenu()
    {
        _bGame = false;
        BInsertCoinGameStart = true;
        if (_gamePaused) Pause_Game();

        ResetGameState();

        if (_objSkillshotMission)
            _objSkillshotMission.SendMessage("Disable_Skillshot_Mission");

        ResetMultiBallState();

        BInsertCoinGameStart = false;
        if (_uiManager != null)
            Add_Info_To_Array(_uiManager.Txt_InsertCoin.GetLocalizedString(), 3);
        _lcdWaitStartGame = true;

        StartCoroutine(InitGameWaitForFrame());
    }

    public void InitObjAfterMultiball()
    {
        ActivateDeactivateObjects(DeactivateObjWhenMultIsFinished, false);
    }

    public void NewBall(Vector3 pos)
    {
        if (SLoadBall) _sound.PlayOneShot(SLoadBall);
        Instantiate(Ball, pos, Quaternion.identity);
        _numberOfBallOnBoard++;
    }


    public void PlayMultiLeds(int seqNum)
    {
        _animInProgress = seqNum;
        for (var i = 0; i < LedsMulti[seqNum].Obj.Length; i++)
            LedsMulti[seqNum].ManagerLedAnimation[i].Play_New_Pattern(LedsMulti[seqNum].NumPattern[i]);
    }

    public void QuitGame()
    {
        InitGame_GoToMainMenu();

        if (_uiManager != null)
            _uiManager.ShowQuitYesUI();

        Application.Quit();
    }

    public void Save_Best_Score()
    {
        var finalScore = _playerScore + _tmpBonusScore;
        PlayerPrefs.SetInt("CurrentScore", finalScore);

        if (!PlayerPrefs.HasKey(HIGHSCORE_NAME) || PlayerPrefs.GetInt(HIGHSCORE_NAME) < finalScore) PlayerPrefs.SetInt(HIGHSCORE_NAME, finalScore);

        if (_uiManager != null)
        {
            _uiManager.UpdateCanvasScore(finalScore);
            _uiManager.UpdateCanvasBestScore(GetHighScore());
        }
    }

    public void Shake_AddForce_ToBall(Vector3 direction)
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var ball in balls)
            ball.GetComponent<Ball>().Ball_Shake(direction);
    }

    public void Start_Pause_Mode(int keepAlive)
    {
        for (var i = 0; i < _objManagers.Length; i++)
        {
            if (keepAlive != _missionsIndex[i])
                _pauseMission[i].Start_Pause_Mission();
        }
    }

    public void Stop_Pause_Mode()
    {
        for (var i = 0; i < _objManagers.Length; i++)
            _pauseMission[i].Stop_Pause_Mission();
    }

    public void Total_Ball_Score()
    {
        _playerScore += _tmpBonusScore;
    }

    private void ActivateDeactivateObjects(GameObject[] objects, bool activate)
    {
        if (objects == null || objects.Length == 0) return;

        var message = activate ? "Activate_Object" : "Desactivate_Object";
        for (var i = 0; i < objects.Length; i++)
            objects[i].SendMessage(message);
    }

    private void ActivateTiltMode(int cameraShakeType)
    {
        Start_Pause_Mode(-1);
        if (_cameraMovement) _cameraMovement.Shake_Cam(cameraShakeType);
        if (STilt) _sound.PlayOneShot(STilt);
        if (_uiManager != null)
            _uiManager.Add_Info_To_Array(_uiManager.Txt_Tilt.GetLocalizedString(), 3);

        _bTilt = 2;
        _tiltTimer = 0;
        F_Mode_Ball_Saver_Off();
        BExtraBall = false;
        if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_Off();
        Flippers_Plunger_State_Tilt_Mode("Desactivate");
    }

    private void HandleBallOutSequence()
    {
        if (_bBalloutPart1 && _bBalloutPart2 && _bBalloutPart3) return;

        _bTilt = 2; // Disable tilt mode when player loses a ball

        for (var i = 0; i < SpringLauncher.Length; i++)
            SpringLauncher[i].F_Desactivate();

        if (_objSkillshotMission)
            _objSkillshotMission.SendMessage("Disable_Skillshot_Mission");

        // Part 1: Ball Lost
        if (!_bBalloutPart1)
        {
            _tmpBalloutTime = Mathf.MoveTowards(_tmpBalloutTime, TimeBalloutPart1BallOut, Time.deltaTime);
            if (_tmpBalloutTime == TimeBalloutPart1BallOut)
            {
                if (ABonusScreen)
                {
                    _sound.clip = ABonusScreen;
                    _sound.Play();
                }

                _tmpBalloutTime = 0;
                _bBalloutPart1 = true;
                _bBalloutPart2 = false;
                if (_uiManager != null)
                {
                    var bonusText = _uiManager.Txt_BonusHits.GetLocalizedString() + "\n"      + _tmpBonusGlobalHitCounter +
                                    _uiManager.Txt_BonusBase.GetLocalizedString() + BonusBase + " x "                     + "\n" +
                                    _tmpMultiplier                                + _uiManager.Txt_BonusMultiplier.GetLocalizedString();
                    Add_Info_To_Array(bonusText, TimeBalloutPart2Bonus);
                }

                Total_Ball_Score();
            }
        }

        // Part 2: Bonus Calculation
        if (!_bBalloutPart2)
        {
            _tmpBalloutTime2 = Mathf.MoveTowards(_tmpBalloutTime2, TimeBalloutPart2Bonus, Time.deltaTime);
            if (_tmpBalloutTime2 == TimeBalloutPart2Bonus)
            {
                _tmpBalloutTime2 = 0;
                _bBalloutPart2 = true;
                _bBalloutPart3 = false;
                if (_uiManager != null)
                    Add_Info_To_Array(_uiManager.Txt_TotalScore.GetLocalizedString() + "\n" + _playerScore, TimeBalloutPart3TotalScore);
            }
        }

        // Part 3: Next Ball or Game Over
        if (!_bBalloutPart3)
        {
            _tmpBalloutTime3 = Mathf.MoveTowards(_tmpBalloutTime3, TimeBalloutPart3TotalScore, Time.deltaTime);
            if (_tmpBalloutTime3 == TimeBalloutPart3TotalScore)
            {
                _tmpBalloutTime3 = 0;
                _bBalloutPart3 = true;
                ProcessBallOutResult();
            }
        }
    }

    private void HandleBallSaver()
    {
        // Respawn timer for ball saver
        if (BRespawnTimerBallSaver)
        {
            RespawnTimerBallSaver = Mathf.MoveTowards(RespawnTimerBallSaver, 1, Time.deltaTime);
            if (RespawnTimerBallSaver == 1)
            {
                BRespawnTimerBallSaver = false;
                MultiBall.KickBack_MultiOnOff();
            }
        }

        // Ball saver duration timer
        if (_bTimerBallSaver)
        {
            _tmpBallSaver = Mathf.MoveTowards(_tmpBallSaver, _timerBallSaver, Time.deltaTime);
            if (_timerBallSaver == _tmpBallSaver)
            {
                _bTimerBallSaver = false;
                _tmpBallSaver = 0;
                F_Mode_Ball_Saver_Off();
            }
        }

        // Delay before multi-ball start
        if (_timeToWaitBeforeMultiBallStart)
        {
            _timerMultiBall = Mathf.MoveTowards(_timerMultiBall, TimeToWaitMulti, Time.deltaTime);
            if (_timerMultiBall == TimeToWaitMulti)
            {
                _timerMultiBall = 0;
                _timeToWaitBeforeMultiBallStart = false;
                F_Mode_MultiBall();
            }
        }
    }

    private void HandleGameStart()
    {
        if (_uiManager != null && _uiManager.IsUIActive()) return;

        if (_inputManager.WasPlungerPressed() && !_bGame)
            F_InsertCoin_GameStart();
    }

    private void HandleMultiBall()
    {
        if (!_multiBall) return;

        if (_numberOfBallOnBoard < 3 && _timerMulti > 1 && _tmpReloadNumber > 0)
        {
            NewBall(_spawnBall.transform.position);
            _timerMulti = 0;
            _tmpReloadNumber--;
        }

        _timerMulti += Time.deltaTime;

        if (_tmpReloadNumber == 0 && _timerMulti > 1)
            F_Mode_MultiBall();
    }

    private void HandleShakeInput()
    {
        if (_inputManager == null || !_bGame) return;

        if (_inputManager.WasShakeRightPressed())
            ProcessShake(new Vector3(-1, 0, 0), 1);
        else if (_inputManager.WasShakeLeftPressed())
            ProcessShake(new Vector3(1, 0, 0), 2);
        else if (_inputManager.WasShakeUpPressed())
            ProcessShake(new Vector3(0, 0, 1), 3);
    }

    private void HandleTiltTimer()
    {
        if (_bTilt != 1) return;

        _tiltTimer = Mathf.MoveTowards(_tiltTimer, MinTimeTilt, Time.deltaTime);
        if (_tiltTimer == MinTimeTilt)
        {
            _bTilt = 0;
            _tiltTimer = 0;
        }
    }

    private void HandleUINavigation()
    {
        if (_inputManager.WasPausePressed())
            F_Pause_Game();

        if (_inputManager.WasCameraChangePressed() && _cameraMovement)
            _cameraMovement.Selected_Cam();
    }

    private IEnumerator InitGameWaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        if (_cameraMovement) _cameraMovement.PlayIdle();
        LoopAnimDemoPlayfield = true;
        PlayMultiLeds(AnimDemoPlayfield);
    }

    private void InitializeCamera()
    {
        var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (var cam in cameras)
        {
            var camMovement = cam.GetComponent<Camera_Movement>();
            if (camMovement) _cameraMovement = camMovement;
        }
    }

    private void InitializeComponents()
    {
        if (ObjLauncherMultiBall)
            MultiBall = ObjLauncherMultiBall.GetComponent<MultiBall>();

        _sound = GetComponent<AudioSource>();

        if (_spawnBall == null)
            _spawnBall = GameObject.Find("Plunger_Spawn");

        // Initialize plungers
        var plungers = GameObject.FindGameObjectsWithTag("Plunger");
        SpringLauncher = new SpringLauncher[plungers.Length];
        for (var i = 0; i < plungers.Length; i++)
            SpringLauncher[i] = plungers[i].GetComponent<SpringLauncher>();

        // Initialize missions
        var missions = GameObject.FindGameObjectsWithTag("Missions");
        _objManagers = new GameObject[missions.Length];
        _missionsIndex = new int[missions.Length];

        for (var i = 0; i < missions.Length; i++)
        {
            _objManagers[i] = missions[i];
            _missionsIndex[i] = missions[i].GetComponent<MissionIndex>().F_index();
        }

        _pauseMission = new Pause_Mission[_objManagers.Length];
        for (var i = 0; i < _objManagers.Length; i++)
            _pauseMission[i] = _objManagers[i].GetComponent<Pause_Mission>();
    }

    private void InitializeLeds()
    {
        if (ObjLedExtraBall)
            _ledExtraBall = ObjLedExtraBall.GetComponent<ChangeSpriteRenderer>();
        if (ObjLedBallSaver)
            _ledBallSaverRenderer = ObjLedBallSaver.GetComponent<ChangeSpriteRenderer>();

        if (ObjMultiplierLeds.Length > 0)
        {
            _ledMultiplierRenderer = new ChangeSpriteRenderer[ObjMultiplierLeds.Length];
            for (var i = 0; i < ObjMultiplierLeds.Length; i++)
                _ledMultiplierRenderer[i] = ObjMultiplierLeds[i].GetComponent<ChangeSpriteRenderer>();
        }
    }

    private void InitializeLedsPatterns()
    {
        if (LedsMulti[0].Obj[0] == null)
        {
            // Auto-create LED pattern from tagged objects
            var missionObjs = GameObject.FindGameObjectsWithTag("Missions");
            _objTmpMission = new GameObject[missionObjs.Length];
            for (var i = 0; i < missionObjs.Length; i++)
                _objTmpMission[i] = missionObjs[i];

            var ledGroupObjs = GameObject.FindGameObjectsWithTag("Leds_Groups");
            _objTmpLedsGroups = new GameObject[ledGroupObjs.Length];
            for (var i = 0; i < ledGroupObjs.Length; i++)
                _objTmpLedsGroups[i] = ledGroupObjs[i];

            var totalCount = _objTmpMission.Length + _objTmpLedsGroups.Length;
            LedsMulti[0].Obj = new GameObject[totalCount];
            LedsMulti[0].NumPattern = new int[totalCount];

            var idx = 0;
            for (var i = 0; i < _objTmpMission.Length; i++)
            {
                LedsMulti[0].Obj[i] = _objTmpMission[i];
                LedsMulti[0].NumPattern[idx] = 0;
                idx++;
            }

            for (var i = 0; i < _objTmpLedsGroups.Length; i++)
            {
                LedsMulti[0].Obj[idx] = _objTmpLedsGroups[i];
                LedsMulti[0].NumPattern[idx] = 0;
                idx++;
            }
        }

        for (var j = 0; j < LedsMulti.Length; j++)
        {
            LedsMulti[j].ManagerLedAnimation = new Manager_Led_Animation[LedsMulti[j].Obj.Length];
            for (var k = 0; k < LedsMulti[j].Obj.Length; k++)
                LedsMulti[j].ManagerLedAnimation[k] = LedsMulti[j].Obj[k].GetComponent<Manager_Led_Animation>();
        }
    }

    private void InitializeManagers()
    {
        _managerInputSetting = GetComponent<ManagerInputSetting>();
        _inputManager = PinballInputManager.Instance;
        _uiManager = UiManager.Instance;

        if (_inputManager == null) Debug.LogWarning("ManagerGame: PinballInputManager not found.");
        if (_uiManager    == null) Debug.LogWarning("ManagerGame: UiManager not found.");
    }

    private void InsertCoin_GameStart()
    {
        _lcdWaitStartGame = false;
        LoopAnimDemoPlayfield = false;
        if (_uiManager != null)
            Add_Info_To_Array(_uiManager.Txt_GameStart.GetLocalizedString(), 3);
        _bGame = true;
        BInsertCoinGameStart = true;
        if (_gamePaused) Pause_Game();

        ResetGameState();

        if (_objSkillshotMission)
            _objSkillshotMission.SendMessage("Enable_Skillshot_Mission");

        if (_spawnBall != null) NewBall(_spawnBall.transform.position);
        PlayMultiLeds(NewBallLedAnimation);
        BInsertCoinGameStart = false;
    }

    private void NotifyMissionMultiBallEnded()
    {
        for (var i = 0; i < _missionsIndex.Length; i++)
        {
            if (_missionsIndex[i] == _tmpIndexInfo && _tmpIndexInfo != -1)
                _objManagers[i].SendMessage("Mode_MultiBall_Ended");
        }
    }

    private void NotifyMissionsAfterBallLost()
    {
        var missions = GameObject.FindGameObjectsWithTag("Missions");
        foreach (var mission in missions)
            mission.SendMessage("Mission_Intialisation_AfterBallLost");
    }

    private void Pause_Game()
    {
        _gamePaused = !_gamePaused;

        PauseCameras();
        PauseGameObjects();
        PauseMechanics();

        GetComponent<Blink>().Pause_Blinking();

        if (_gamePaused && _sound.isPlaying)
            _sound.Pause();
        else
            _sound.UnPause();
    }

    private void PauseCameras()
    {
        var mainCameras = GameObject.FindGameObjectsWithTag("MainCamera");
        foreach (var cam in mainCameras)
        {
            var camMovement = cam.GetComponent<Camera_Movement>();
            if (camMovement)
            {
                if (_gamePaused)
                    camMovement.StartPauseMode();
                else
                    camMovement.StopPauseMode();
            }
        }

        var pivotCams = GameObject.FindGameObjectsWithTag("PivotCam");
        foreach (var cam in pivotCams)
        {
            if (_gamePaused)
                cam.GetComponent<CameraSmoothFollow>().StartPauseMode();
            else
                cam.GetComponent<CameraSmoothFollow>().StopPauseMode();
        }
    }

    private void PauseGameObjects()
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var ball in balls)
            ball.GetComponent<Ball>().Ball_Pause();

        var ledGroups = GameObject.FindGameObjectsWithTag("Leds_Groups");
        foreach (var led in ledGroups)
            led.GetComponent<Manager_Led_Animation>().Pause_Anim();

        var missions = GameObject.FindGameObjectsWithTag("Missions");
        foreach (var mission in missions)
            mission.GetComponent<Pause_Mission>().Pause_Game();

        var ledAnims = GameObject.FindGameObjectsWithTag("Led_animation");
        foreach (var led in ledAnims)
            led.GetComponent<Anim_On_LCD>().Pause_Anim();

        var animatedObjects = GameObject.FindGameObjectsWithTag("AnimatedObject");
        foreach (var obj in animatedObjects)
        {
            var toys = obj.GetComponent<Toys>();
            if (toys) toys.Pause_Anim();
            var moving = obj.GetComponent<movingObject>();
            if (moving) moving.Pause_Anim();
        }
    }

    private void PauseMechanics()
    {
        var flippers = GameObject.FindGameObjectsWithTag("Flipper");
        foreach (var flipper in flippers)
        {
            var component = flipper.GetComponent<Flippers>();
            if (_gamePaused)
                component.F_Pause_Start();
            else
                component.F_Pause_Stop();
        }

        var plungers = GameObject.FindGameObjectsWithTag("Plunger");
        foreach (var plunger in plungers)
        {
            var component = plunger.GetComponent<SpringLauncher>();
            if (_gamePaused)
                component.F_Desactivate();
            else
                component.F_Activate();
        }

        var holes = GameObject.FindGameObjectsWithTag("Hole_Multi");
        foreach (var hole in holes)
            hole.GetComponent<MultiBall>().F_Pause();

        var singleHoles = GameObject.FindGameObjectsWithTag("Hole");
        foreach (var hole in singleHoles)
            hole.GetComponent<Hole>().F_Pause();

        var spinners = GameObject.FindGameObjectsWithTag("spinner");
        foreach (var spinner in spinners)
        {
            var component = spinner.GetComponent<Spinner_Rotation>();
            if (_gamePaused)
                component.F_Pause_Start();
            else
                component.F_Pause_Stop();
        }
    }

    private void PlayLoseBallSound()
    {
        if (ALoseBall)
        {
            _sound.clip = ALoseBall;
            _sound.Play();
        }
    }

    private void ProcessBallLost()
    {
        if (BBallSaver && _numberOfBallOnBoard == 0)
        {
            // Ball Saver activated
            PlayMultiLeds(BallSaverLedAnimation);
            MultiBall.KickBack_MultiOnOff();
            NewBall(_spawnBall.transform.position);
            if (_uiManager != null)
                Add_Info_To_Array(_uiManager.Txt_BallSaver.GetLocalizedString(), 3);
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
            // Extra ball activated
            if (_uiManager != null)
                Add_Info_To_Array(_uiManager.Txt_ExtraBall.GetLocalizedString(), 3);
            NewBall(_spawnBall.transform.position);
            BExtraBall = false;
            if (ObjLedExtraBall) _ledExtraBall.F_ChangeSprite_Off();
        }
        else if (_tmpLife > 1 && _numberOfBallOnBoard == 0)
        {
            // New ball (player has lives remaining)
            if (_uiManager != null)
                Add_Info_To_Array(_uiManager.Txt_NewBall.GetLocalizedString(), 3);
            _bBalloutPart1 = false;
            PlayLoseBallSound();
            init_Param_After_Ball_Lost();
            _tmpLife--;
            NotifyMissionsAfterBallLost();
            PlayMultiLeds(GameOverLedAnimation);
            _ballNum++;
        }
        else if (_numberOfBallOnBoard <= 0)
        {
            // Game Over
            if (_uiManager != null)
            {
                _uiManager.ClearBallInfo();
                Add_Info_To_Array(_uiManager.Txt_GameOver.GetLocalizedString(), 3);
            }

            _bBalloutPart1 = false;
            PlayLoseBallSound();
            init_Param_After_Ball_Lost();
            _tmpLife--;
            Init_All_Mission();
            PlayMultiLeds(GameOverLedAnimation);
            Save_Best_Score();
        }
    }

    private void ProcessBallOutResult()
    {
        if (_tmpLife >= 1)
        {
            // Player has remaining lives - spawn new ball
            PlayMultiLeds(NewBallLedAnimation);
            NewBall(_spawnBall.transform.position);
            if (StartGameWithBallSaver)
                F_Mode_Ball_Saver_On(StartDuration);
            if (_uiManager != null)
                Add_Info_To_Array(_uiManager.Txt_NextBall.GetLocalizedString(), 2);
            _bTilt = 0;
            if (_objSkillshotMission)
                _objSkillshotMission.SendMessage("Enable_Skillshot_Mission");
            Stop_Pause_Mode();
        }
        else
        {
            // Game Over
            PlayMultiLeds(AnimDemoPlayfield);
            LoopAnimDemoPlayfield = true;
            _bGame = false;
            _bTilt = 0;
            Stop_Pause_Mode();
            if (_uiManager != null)
                _uiManager.ShowGameOverUI();
        }
    }

    private void ProcessShake(Vector3 forceDirection, int cameraShakeType)
    {
        if (_bTilt == 1)
            // Activate tilt mode
            ActivateTiltMode(cameraShakeType);
        else if (_bTilt == 0)
            // First warning
            ShowTiltWarning(forceDirection, cameraShakeType);
    }

    private void ResetGameState()
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var ball in balls) Destroy(ball);

        var ledAnims = GameObject.FindGameObjectsWithTag("Led_animation");
        foreach (var led in ledAnims)
            led.GetComponent<Anim_On_LCD>().DestoyAnimGameobject();

        Init_All_Mission();

        var flippers = GameObject.FindGameObjectsWithTag("Flipper");
        foreach (var flipper in flippers)
            flipper.GetComponent<Flippers>().F_Activate();

        var plungers = GameObject.FindGameObjectsWithTag("Plunger");
        foreach (var plunger in plungers)
            plunger.GetComponent<SpringLauncher>().F_Activate();

        _tmpLife = BInsertCoinGameStart && !_bGame ? 0 : Life;
        _playerScore = 0;
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
    }

    private void ResetMultiBallState()
    {
        _multiBall = false;
        _tmpReloadNumber = 0;
        _timerMulti = 2;
        if (_cameraMovement) _cameraMovement.Camera_MultiBall_Stop();
        NotifyMissionMultiBallEnded();
        _missionMultiBallEnded = false;

        var holes = GameObject.FindGameObjectsWithTag("Hole");
        foreach (var hole in holes)
            hole.GetComponent<Hole>().initHole();

        var multiHoles = GameObject.FindGameObjectsWithTag("Hole_Multi");
        foreach (var hole in multiHoles)
            hole.GetComponent<MultiBall>().initHole();
    }

    private void ShowTiltWarning(Vector3 forceDirection, int cameraShakeType)
    {
        if (_uiManager != null)
            _uiManager.Add_Info_To_Array(_uiManager.Txt_TiltWarning.GetLocalizedString(), 1);
        if (SWarning) _sound.PlayOneShot(SWarning);
        if (_cameraMovement) _cameraMovement.Shake_Cam(cameraShakeType);
        Shake_AddForce_ToBall(forceDirection);
        _bTilt = 1;
    }

    private void UpdateDefaultDisplay()
    {
        if (_uiManager != null)
            _uiManager.UpdateDefaultDisplay(_playerScore, _ballNum, _lcdWaitStartGame, _tmpLife, GetHighScore());
    }

    private IEnumerator WaitToInit()
    {
        yield return new WaitForEndOfFrame();
        if (LoopAnimDemoPlayfield) PlayMultiLeds(AnimDemoPlayfield);
    }

    #endregion
}