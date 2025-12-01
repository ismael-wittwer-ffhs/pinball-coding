// UiManager : Manages all UI elements including LCD screen and canvas-based UI

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    #region --- Statics ---

    public static UiManager Instance { get; private set; }

    #endregion

    #region --- Exposed Fields ---

    public GameObject Mobile_Cam_Txt; // Use to deactivate Mobile Change Camera text if you use the Mobile System of pause and change camera
    public GameObject Mobile_PauseAndCam; // Use to deactivate Mobile pause and Mobile Change Camera button if you use the Mobile System of pause and change camera

    [FormerlySerializedAs("Game_UI")]
    [Header("UI GameObjects")]
    public GameObject UI_GameOverScreen; // Connect the parent UI

    [FormerlySerializedAs("Game_UI2")]
    public GameObject UI_StartScreen; // Connect the parent UI Part2 Contain button Start and quit

    public string BestScoreName = "BestScore"; // Score is saved with this name

    [Header("Localized Strings")]
    public LocalizedString Txt_ScorePrefix; // "Score: " prefix for player score
    public LocalizedString Txt_BallPrefix; // "Ball: " prefix for ball number
    public LocalizedString Txt_GameOver; // "Game Over" text
    public LocalizedString Txt_InsertCoin; // "Insert Coin" text
    public LocalizedString Txt_InitialWelcome; // Initial welcome/start text
    public LocalizedString Txt_BestScorePrefix; // "Best Score: " prefix for best score
    
    // GameManager localized strings
    public LocalizedString Txt_Tilt; // Tilt text (TxtGame[0])
    public LocalizedString Txt_TiltWarning; // Tilt warning text (TxtGame[1])
    public LocalizedString Txt_BonusHits; // Bonus hits part (TxtGame[5])
    public LocalizedString Txt_BonusBase; // Bonus base part (TxtGame[6])
    public LocalizedString Txt_BonusMultiplier; // Bonus multiplier part (TxtGame[7])
    public LocalizedString Txt_TotalScore; // Total score text (TxtGame[8])
    public LocalizedString Txt_NextBall; // Next ball text (TxtGame[9])
    public LocalizedString Txt_BallSaver; // Ball saver text (TxtGame[10])
    public LocalizedString Txt_ExtraBall; // Extra ball text (TxtGame[11])
    public LocalizedString Txt_NewBall; // New ball text (TxtGame[12])
    public LocalizedString Txt_GameStart; // Game start text (TxtGame[14])

    #endregion

    #region --- Private Fields ---

    // UI State Management
    private bool b_Txt_Info = true; // Use to display text on LCD screen
    private bool LCD_Wait_Start_Game = true; // use to switch between best score and insert coin

    private EventSystem eventSystem;
    private float TimeBetweenTwoInfo; // Use to display text on LCD screen
    private float tmp_Time; // Use to display text on LCD screen

    // Cached Button References
    private GameObject btn_InsertCoin;
    private GameObject btn_RestartYes;
    private GameObject btn_ResumeGame;
    private int count_LCD;

    // LCD Screen Text Components
    private Text Gui_Txt_Info_Ball; // Connect a UI.Text
    private Text Gui_Txt_Score; // Connect a UI.Text
    private Text Gui_Txt_Timer; // Connect a UI.Text

    // Canvas UI Text Components
    private Text Obj_UI_BestScore; // Display the best score
    private Text Obj_UI_Score; // Display game score

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
        InitializeUI();
    }

    private void Update()
    {
        UpdateLCDInfo();
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
        if (Gui_Txt_Score) Gui_Txt_Score.text = inf;
        if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = ""; // We don't want to display the ball number when there is player information on LCD Fake Screen
        tmp_Time = 0;
        TimeBetweenTwoInfo = Timer;
        b_Txt_Info = false;
    }

    public void Add_Info_To_Timer(string inf)
    {
        // --> Timer Text : Call This function to add text to Gui_Txt_Timer
        if (Gui_Txt_Timer) Gui_Txt_Timer.text = inf;
    }

    public void ClearBallInfo()
    {
        if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = "";
    }

    public bool IsUIActive()
    {
        return UI_GameOverScreen != null && UI_GameOverScreen.activeInHierarchy;
    }

    public void SetLCDWaitStartGame(bool value)
    {
        LCD_Wait_Start_Game = value;
    }

    public void ShowGameOverUI()
    {
        if (UI_GameOverScreen)
        {
            if (!UI_GameOverScreen.activeInHierarchy) UI_GameOverScreen.SetActive(true);

            var tmpLeadSaveName_ButtonNext = GameObject.Find("Button_Next_Lead");
            if (tmpLeadSaveName_ButtonNext && eventSystem != null)
                eventSystem.SetSelectedGameObject(tmpLeadSaveName_ButtonNext); // Select the button Next letter on Leaderboard save name Panel.
            else if (btn_RestartYes && eventSystem != null)
                eventSystem.SetSelectedGameObject(btn_RestartYes); // Select the button No.

            if (Mobile_PauseAndCam) Mobile_PauseAndCam.SetActive(false);
            if (Mobile_Cam_Txt) Mobile_Cam_Txt.SetActive(false);
        }
    }

    public void ShowGameStartUI()
    {
        if (UI_GameOverScreen)
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(null); // Change selected button
    }

    public void ShowQuitNoUI()
    {
        if (UI_GameOverScreen)
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(null); // Change selected button
    }

    public void ShowQuitYesUI()
    {
        if (UI_GameOverScreen)
        {
            if (UI_StartScreen) UI_StartScreen.SetActive(true);
            if (btn_InsertCoin && eventSystem != null)
                eventSystem.SetSelectedGameObject(btn_InsertCoin); // Change selected button
        }
    }

    public void TogglePauseUI(bool isPaused, bool isGameActive)
    {
        if (UI_GameOverScreen && isGameActive)
        {
            if (Mobile_PauseAndCam && !Mobile_PauseAndCam.activeSelf)
            {
                if (Mobile_PauseAndCam) Mobile_PauseAndCam.SetActive(true);
                if (Mobile_Cam_Txt) Mobile_Cam_Txt.SetActive(true);
                if (UI_GameOverScreen) UI_GameOverScreen.SetActive(false);
            }
            else
            {
                if (Mobile_PauseAndCam) Mobile_PauseAndCam.SetActive(false);
                if (Mobile_Cam_Txt) Mobile_Cam_Txt.SetActive(false);
                if (UI_GameOverScreen) UI_GameOverScreen.SetActive(true);

                // Select resume button when pausing
                if (btn_ResumeGame && eventSystem != null)
                    eventSystem.SetSelectedGameObject(btn_ResumeGame);
            }
        }
    }

    public void UpdateCanvasBestScore(int bestScore)
    {
        if (Obj_UI_BestScore) Obj_UI_BestScore.text = bestScore.ToString();
    }

    public void UpdateCanvasScore(int score)
    {
        if (Obj_UI_Score) Obj_UI_Score.text = score.ToString();
    }

    public void UpdateDefaultDisplay(int playerScore, int ballNum, bool waitStartGame, int tmpLife, int bestScore)
    {
        // Update default display when there's nothing else to show
        // This should be called from ManagerGame's Update when b_Txt_Info would be true
        if (!b_Txt_Info) return;

        if (!waitStartGame)
        {
            if (tmpLife > 0)
            {
                if (Gui_Txt_Score) Gui_Txt_Score.text = Txt_ScorePrefix.GetLocalizedString()        + playerScore; // display player score
                if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = Txt_BallPrefix.GetLocalizedString() + (ballNum + 1); // display the ball number
            }
            else
            {
                // if GameOver
                if (Gui_Txt_Score) Gui_Txt_Score.text = Txt_GameOver.GetLocalizedString(); // display player score
            }
        }
        else
        {
            // Switch between best score and insert coin
            if (count_LCD == 0)
                Add_Info_To_Array(Txt_BestScorePrefix.GetLocalizedString() + "\n" + bestScore, 3);
            else
                Add_Info_To_Array(Txt_InsertCoin.GetLocalizedString(), 3);
            count_LCD++;
            count_LCD = count_LCD % 2;
        }
    }

    public void UpdateScore(int playerScore, int ballNum, bool waitStartGame, int tmpLife)
    {
        // Update score and ball number on LCD screen when nothing else is displayed
        if (b_Txt_Info)
        {
            if (Gui_Txt_Score) Gui_Txt_Score.text = Txt_ScorePrefix.GetLocalizedString()        + playerScore; // display player score
            if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = Txt_BallPrefix.GetLocalizedString() + (ballNum + 1); // display the ball number
        }
    }

    private void InitializeUI()
    {
        // Find EventSystem
        if (eventSystem == null)
        {
            var tmpEvent = GameObject.Find("EventSystem");
            if (tmpEvent)
                eventSystem = tmpEvent.GetComponent<EventSystem>();
        }

        // Find LCD Screen Text Components
        var tmp_Gui = GameObject.Find("txt_Timer");
        if (tmp_Gui) Gui_Txt_Timer = tmp_Gui.GetComponent<Text>();
        tmp_Gui = GameObject.Find("txt_Ball");
        if (tmp_Gui) Gui_Txt_Info_Ball = tmp_Gui.GetComponent<Text>();
        tmp_Gui = GameObject.Find("txt_Score");
        if (tmp_Gui) Gui_Txt_Score = tmp_Gui.GetComponent<Text>();

        // Display initial text on LCD screen
        if (Gui_Txt_Score)
            Gui_Txt_Score.text = Txt_InitialWelcome.GetLocalizedString();

        // Find UI GameObjects
        var _tmp = GameObject.Find("UI_Game_Interface_v2_Lightweight_LCD");
        if (_tmp == null) _tmp = GameObject.Find("UI_Game_Interface_v2");
        if (_tmp != null)
        {
            var children = _tmp.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name == "Text_Camera")
                    Mobile_Cam_Txt = child.gameObject;
            }
        }

        //_tmp = GameObject.Find("G_UI_Game_Interface_Mobile");
        //if (_tmp != null) UI_GameOverScreen = _tmp;

        //_tmp = GameObject.Find("G_UI_Game_Interface_Mobile_Part2");
        //if (_tmp != null) UI_StartScreen = _tmp;

        _tmp = GameObject.Find("PauseAndView");
        if (_tmp)
        {
            var children = _tmp.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name == "btn_Mobile_Pause")
                    Mobile_PauseAndCam = child.gameObject;
            }
        }

        // Initialize Canvas UI
        if (UI_GameOverScreen)
        {
            btn_InsertCoin = GameObject.Find("btn_InsertCoin");
            btn_ResumeGame = GameObject.Find("btn_Resume_Game");
            btn_RestartYes = GameObject.Find("btn_Restart_Yes");

            if (eventSystem != null && btn_InsertCoin)
                eventSystem.SetSelectedGameObject(btn_InsertCoin); // Select the Insert coin button

            tmp_Gui = GameObject.Find("Txt_Best_Score_1");
            if (tmp_Gui) Obj_UI_BestScore = tmp_Gui.GetComponent<Text>();
            tmp_Gui = GameObject.Find("Txt_Game_Score_1");
            if (tmp_Gui) Obj_UI_Score = tmp_Gui.GetComponent<Text>();
            if (Obj_UI_BestScore) Obj_UI_BestScore.text = PlayerPrefs.GetInt(BestScoreName).ToString(); // Display Best score
        }
    }

    private void UpdateLCDInfo()
    {
        // Update LCD info display timing
        if (!b_Txt_Info)
        {
            tmp_Time = Mathf.MoveTowards(tmp_Time, TimeBetweenTwoInfo, Time.deltaTime);
            if (tmp_Time == TimeBetweenTwoInfo)
            {
                b_Txt_Info = true;
                tmp_Time = 0;
            }
        }
    }

    #endregion
}