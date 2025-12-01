// UiManager : Manages all UI elements including LCD screen and canvas-based UI

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    #region --- Singleton ---

    public static UiManager Instance { get; private set; }

    #endregion

    #region --- Exposed Fields ---

    [Header("UI GameObjects")]
    public GameObject Game_UI; // Connect the parent UI
    public GameObject Game_UI2; // Connect the parent UI Part2 Contain button Start and quit
    public GameObject Mobile_Cam_Txt; // Use to deactivate Mobile Change Camera text if you use the Mobile System of pause and change camera
    public GameObject Mobile_PauseAndCam; // Use to deactivate Mobile pause and Mobile Change Camera button if you use the Mobile System of pause and change camera
    public GameObject[] btn_UI; // Connect the UI button
    public float[] PosGameUI; // Choose the different UI position

    [Header("Text Arrays")]
    public string[] Txt_Game; // Array : All the text use by the game Manager
    public string BestScoreName = "BestScore"; // Score is saved with this name

    #endregion

    #region --- Private Fields ---

    private EventSystem eventSystem;

    // LCD Screen Text Components
    private Text Gui_Txt_Info_Ball; // Connect a UI.Text
    private Text Gui_Txt_Score; // Connect a UI.Text
    private Text Gui_Txt_Timer; // Connect a UI.Text

    // Canvas UI Text Components
    private Text Obj_UI_BestScore; // Display the best score
    private Text Obj_UI_Score; // Display game score

    // UI State Management
    private bool b_Txt_Info = true; // Use to display text on LCD screen
    private bool LCD_Wait_Start_Game = true; // use to switch between best score and insert coin
    private float TimeBetweenTwoInfo; // Use to display text on LCD screen
    private float tmp_Time; // Use to display text on LCD screen
    private int count_LCD;

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
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion

    #region --- Initialization ---

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
        if (Gui_Txt_Score && Txt_Game != null && Txt_Game.Length > 15)
            Gui_Txt_Score.text = Txt_Game[15];

        // Find UI GameObjects
        var _tmp = GameObject.Find("UI_Game_Interface_v2_Lightweight_LCD");
        if (_tmp == null) _tmp = GameObject.Find("UI_Game_Interface_v2");
        if (_tmp != null)
        {
            var children = _tmp.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
                if (child.name == "Text_Camera")
                    Mobile_Cam_Txt = child.gameObject;
        }

        _tmp = GameObject.Find("G_UI_Game_Interface_Mobile");
        if (_tmp != null) Game_UI = _tmp;

        _tmp = GameObject.Find("G_UI_Game_Interface_Mobile_Part2");
        if (_tmp != null) Game_UI2 = _tmp;

        _tmp = GameObject.Find("PauseAndView");
        if (_tmp)
        {
            var children = _tmp.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
                if (child.name == "btn_Mobile_Pause")
                    Mobile_PauseAndCam = child.gameObject;
        }

        // Initialize Canvas UI
        if (Game_UI)
        {
            tmp_Gui = GameObject.Find("btn_InsertCoin");
            if (btn_UI != null && btn_UI.Length > 0) btn_UI[0] = tmp_Gui;
            tmp_Gui = GameObject.Find("btn_Resume_Game");
            if (btn_UI != null && btn_UI.Length > 1) btn_UI[1] = tmp_Gui;
            tmp_Gui = GameObject.Find("btn_Restart_Yes");
            if (btn_UI != null && btn_UI.Length > 2) btn_UI[2] = tmp_Gui;
            if (eventSystem != null && btn_UI != null && btn_UI.Length > 0 && btn_UI[0] != null)
                eventSystem.SetSelectedGameObject(btn_UI[0]); // Select the Insert coin button

            tmp_Gui = GameObject.Find("Txt_Best_Score_1");
            if (tmp_Gui) Obj_UI_BestScore = tmp_Gui.GetComponent<Text>();
            tmp_Gui = GameObject.Find("Txt_Game_Score_1");
            if (tmp_Gui) Obj_UI_Score = tmp_Gui.GetComponent<Text>();
            if (Obj_UI_BestScore) Obj_UI_BestScore.text = PlayerPrefs.GetInt(BestScoreName).ToString(); // Display Best score
        }
    }

    #endregion

    #region --- LCD Screen Methods ---

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

    public void UpdateScore(int playerScore, int ballNum, bool waitStartGame, int tmpLife)
    {
        // Update score and ball number on LCD screen when nothing else is displayed
        if (b_Txt_Info && Txt_Game != null && Txt_Game.Length > 3)
        {
            if (Gui_Txt_Score) Gui_Txt_Score.text = Txt_Game[2] + playerScore; // display player score
            if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = Txt_Game[3] + (ballNum + 1); // display the ball number
        }
    }

    public void UpdateDefaultDisplay(int playerScore, int ballNum, bool waitStartGame, int tmpLife, int bestScore)
    {
        // Update default display when there's nothing else to show
        // This should be called from ManagerGame's Update when b_Txt_Info would be true
        if (!b_Txt_Info) return;

        if (!waitStartGame)
        {
            if (tmpLife > 0 && Txt_Game != null && Txt_Game.Length > 3)
            {
                if (Gui_Txt_Score) Gui_Txt_Score.text = Txt_Game[2] + playerScore; // display player score
                if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = Txt_Game[3] + (ballNum + 1); // display the ball number
            }
            else if (Txt_Game != null && Txt_Game.Length > 4)
            {
                // if GameOver
                if (Gui_Txt_Score) Gui_Txt_Score.text = Txt_Game[4]; // display player score
            }
        }
        else
        {
            // Switch between best score and insert coin
            if (Txt_Game != null && Txt_Game.Length > 16)
            {
                if (count_LCD == 0)
                    Add_Info_To_Array(Txt_Game[16] + "\n" + bestScore, 3);
                else if (Txt_Game.Length > 4)
                    Add_Info_To_Array(Txt_Game[4], 3);
                count_LCD++;
                count_LCD = count_LCD % 2;
            }
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

    public void SetLCDWaitStartGame(bool value)
    {
        LCD_Wait_Start_Game = value;
    }

    public void ClearBallInfo()
    {
        if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = "";
    }

    #endregion

    #region --- Canvas UI Methods ---

    public void UpdateCanvasScore(int score)
    {
        if (Obj_UI_Score) Obj_UI_Score.text = score.ToString();
    }

    public void UpdateCanvasBestScore(int bestScore)
    {
        if (Obj_UI_BestScore) Obj_UI_BestScore.text = bestScore.ToString();
    }

    public void ShowGameOverUI()
    {
        if (Game_UI)
        {
            if (!Game_UI.activeInHierarchy) Game_UI.SetActive(true);

            if (PosGameUI != null && PosGameUI.Length > 0)
            {
                Game_UI.GetComponent<RectTransform>().pivot = new Vector2(
                    Game_UI.GetComponent<RectTransform>().pivot.x,
                    PosGameUI[0]
                );
            }

            GameObject tmpLeadSaveName_ButtonNext = GameObject.Find("Button_Next_Lead");
            if (tmpLeadSaveName_ButtonNext && eventSystem != null)
                eventSystem.SetSelectedGameObject(tmpLeadSaveName_ButtonNext); // Select the button Next letter on Leaderboard save name Panel.
            else if (btn_UI != null && btn_UI.Length > 2 && btn_UI[2] != null && eventSystem != null)
                eventSystem.SetSelectedGameObject(btn_UI[2]); // Select the button No.

            if (Mobile_PauseAndCam) Mobile_PauseAndCam.SetActive(false);
            if (Mobile_Cam_Txt) Mobile_Cam_Txt.SetActive(false);
        }
    }

    public void ShowGameStartUI()
    {
        if (Game_UI && PosGameUI != null && PosGameUI.Length > 0)
        {
            Game_UI.GetComponent<RectTransform>().pivot = new Vector2(
                Game_UI.GetComponent<RectTransform>().pivot.x,
                PosGameUI[0]
            );
            if (eventSystem != null) eventSystem.SetSelectedGameObject(null); // Change selected button
        }
    }

    public void TogglePauseUI(bool isPaused, bool isGameActive)
    {
        if (Game_UI && isGameActive)
        {
            if (Mobile_PauseAndCam && !Mobile_PauseAndCam.activeSelf)
            {
                if (Mobile_PauseAndCam) Mobile_PauseAndCam.SetActive(true);
                if (Mobile_Cam_Txt) Mobile_Cam_Txt.SetActive(true);
                if (Game_UI) Game_UI.SetActive(false);
            }
            else
            {
                if (Mobile_PauseAndCam) Mobile_PauseAndCam.SetActive(false);
                if (Mobile_Cam_Txt) Mobile_Cam_Txt.SetActive(false);
                if (Game_UI) Game_UI.SetActive(true);
            }

            if (PosGameUI != null && PosGameUI.Length > 1)
            {
                if (Game_UI.GetComponent<RectTransform>().pivot.y == PosGameUI[0])
                {
                    // Pause Start
                    Game_UI.GetComponent<RectTransform>().pivot = new Vector2(
                        Game_UI.GetComponent<RectTransform>().pivot.x,
                        PosGameUI[1]
                    );
                    if (Game_UI.activeInHierarchy && btn_UI != null && btn_UI.Length > 1 && btn_UI[1] != null && eventSystem != null)
                        eventSystem.SetSelectedGameObject(btn_UI[1]); // Change selected button
                }
                else
                {
                    // Pause Stop
                    Game_UI.GetComponent<RectTransform>().pivot = new Vector2(
                        Game_UI.GetComponent<RectTransform>().pivot.x,
                        PosGameUI[0]
                    );
                    if (Game_UI.activeInHierarchy && eventSystem != null)
                        eventSystem.SetSelectedGameObject(null); // Change selected button
                }
            }
        }
    }

    public void ShowQuitNoUI()
    {
        if (Game_UI && PosGameUI != null && PosGameUI.Length > 0)
        {
            Game_UI.GetComponent<RectTransform>().pivot = new Vector2(
                Game_UI.GetComponent<RectTransform>().pivot.x,
                PosGameUI[0]
            );
            if (eventSystem != null) eventSystem.SetSelectedGameObject(null); // Change selected button
        }
    }

    public void ShowQuitYesUI()
    {
        if (Game_UI && PosGameUI != null && PosGameUI.Length > 2)
        {
            Game_UI.GetComponent<RectTransform>().pivot = new Vector2(
                Game_UI.GetComponent<RectTransform>().pivot.x,
                PosGameUI[2]
            );
            if (Game_UI2) Game_UI2.SetActive(true);
            if (btn_UI != null && btn_UI.Length > 0 && btn_UI[0] != null && eventSystem != null)
                eventSystem.SetSelectedGameObject(btn_UI[0]); // Change selected button
        }
    }

    public void SelectLastButton()
    {
        if (Game_UI && PosGameUI != null && btn_UI != null && eventSystem != null)
        {
            if (PosGameUI.Length > 2 && Game_UI.GetComponent<RectTransform>().pivot.y == PosGameUI[2])
            {
                if (btn_UI.Length > 0 && btn_UI[0] != null)
                    eventSystem.SetSelectedGameObject(btn_UI[0]);
            }

            if (PosGameUI.Length > 1 && Game_UI.GetComponent<RectTransform>().pivot.y == PosGameUI[1])
            {
                if (btn_UI.Length > 1 && btn_UI[1] != null)
                    eventSystem.SetSelectedGameObject(btn_UI[1]);
            }
        }
    }

    public bool IsUIActive()
    {
        return Game_UI != null && Game_UI.activeInHierarchy;
    }

    public void NewValueForUi(int value)
    {
        if (PosGameUI == null || PosGameUI.Length < 3) return;

        if (value == 0)
        {
            PosGameUI[0] = 14;
            PosGameUI[1] = 22;
            PosGameUI[2] = 0;
            if (PosGameUI.Length > 3) PosGameUI[3] = 34;
        }
        else
        {
            PosGameUI[0] = -6.04f;
            PosGameUI[1] = -3.05f;
            PosGameUI[2] = .05f;
        }
    }

    #endregion
}

