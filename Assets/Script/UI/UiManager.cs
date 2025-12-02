// UiManager : Manages all UI elements including LCD screen and canvas-based UI

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    #region --- Statics ---

    public static UiManager Instance { get; private set; }

    #endregion

    #region --- Exposed Fields ---

    public GameObject BlackScreen; // Black screen overlay for scene transitions

    [Header("Score Popup")]
    [Tooltip("Prefab with TextMeshPro text object to instantiate when score is awarded")]
    public GameObject scoreTextPrefab; // Prefab holding a TextMeshPro text object

    [FormerlySerializedAs("Game_UI")]
    [Header("UI GameObjects")]
    public GameObject UI_GameOverScreen; // Connect the parent UI

    public GameObject UI_PauseScreen; // Connect the pause screen UI

    [FormerlySerializedAs("Game_UI2")]
    public GameObject UI_StartScreen; // Connect the parent UI Part2 Contain button Start and quit

    public LocalizedString Txt_BallPrefix; // "Ball: " prefix for ball number
    public LocalizedString Txt_BallSaver; // Ball saver text (TxtGame[10])
    public LocalizedString Txt_BestScorePrefix; // "Best Score: " prefix for best score
    public LocalizedString Txt_BonusBase; // Bonus base part (TxtGame[6])
    public LocalizedString Txt_BonusHits; // Bonus hits part (TxtGame[5])
    public LocalizedString Txt_BonusMultiplier; // Bonus multiplier part (TxtGame[7])
    public LocalizedString Txt_ExtraBall; // Extra ball text (TxtGame[11])
    public LocalizedString Txt_GameOver; // "Game Over" text
    public LocalizedString Txt_GameStart; // Game start text (TxtGame[14])
    public LocalizedString Txt_InitialWelcome; // Initial welcome/start text
    public LocalizedString Txt_InsertCoin; // "Insert Coin" text
    public LocalizedString Txt_NewBall; // New ball text (TxtGame[12])
    public LocalizedString Txt_NextBall; // Next ball text (TxtGame[9])

    [Header("Localized Strings")]
    public LocalizedString Txt_ScorePrefix; // "Score: " prefix for player score

    // GameManager localized strings
    public LocalizedString Txt_Tilt; // Tilt text (TxtGame[0])
    public LocalizedString Txt_TiltWarning; // Tilt warning text (TxtGame[1])
    public LocalizedString Txt_TotalScore; // Total score text (TxtGame[8])

    #endregion

    #region --- Private Fields ---

    // UI State Management
    private bool b_Txt_Info = true; // Use to display text on LCD screen
    private bool LCD_Wait_Start_Game = true; // use to switch between best score and insert coin

    // Camera reference
    private Camera_Movement cameraMovement;

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
        InitializeScreenStates();
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

    public void ChangeCamera()
    {
        if (cameraMovement != null)
            cameraMovement.Selected_Cam();
    }

    public void ClearBallInfo()
    {
        if (Gui_Txt_Info_Ball) Gui_Txt_Info_Ball.text = "";
    }

    // Scene Navigation Methods
    public void ExitGame()
    {
        StartCoroutine(ExitGameCoroutine());
    }

    public void GoToMainMenu()
    {
        if (BlackScreen) BlackScreen.SetActive(true);
        StartCoroutine(GoToMainMenuCoroutine());
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
        UI_GameOverScreen?.SetActive(true);

        // Hide other screens
        UI_StartScreen?.SetActive(false);
        UI_PauseScreen?.SetActive(false);

        var tmpLeadSaveName_ButtonNext = GameObject.Find("Button_Next_Lead");
        if (tmpLeadSaveName_ButtonNext && eventSystem != null)
            eventSystem.SetSelectedGameObject(tmpLeadSaveName_ButtonNext); // Select the button Next letter on Leaderboard save name Panel.
        else if (btn_RestartYes && eventSystem != null)
            eventSystem.SetSelectedGameObject(btn_RestartYes); // Select the button No.
    }

    public void ShowGameStartUI()
    {
        // Hide StartScreen when game starts
        UI_StartScreen?.SetActive(false);
        UI_GameOverScreen?.SetActive(false);
        UI_PauseScreen?.SetActive(false);

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
        // Show StartScreen and hide others when quitting
        UI_StartScreen?.SetActive(true);
        UI_GameOverScreen?.SetActive(false);
        UI_PauseScreen?.SetActive(false);

        if (btn_InsertCoin && eventSystem != null)
            eventSystem.SetSelectedGameObject(btn_InsertCoin); // Change selected button
    }

    public void ShowScoreText(int score, Vector3 worldPosition)
    {
        // Instantiate score text prefab at world position
        if (scoreTextPrefab == null) return;

        var scoreTextInstance = Instantiate(scoreTextPrefab, worldPosition, Quaternion.identity);

        // Try to find TextMeshPro component (world space) in the prefab or its children
        var tmpText = scoreTextInstance.GetComponent<TextMeshPro>();
        if (tmpText == null)
            tmpText = scoreTextInstance.GetComponentInChildren<TextMeshPro>();

        // If not found, try TextMeshProUGUI (UI space) - needs to be on Canvas
        if (tmpText == null)
        {
            var tmpTextUI = scoreTextInstance.GetComponent<TextMeshProUGUI>();
            if (tmpTextUI == null)
                tmpTextUI = scoreTextInstance.GetComponentInChildren<TextMeshProUGUI>();

            if (tmpTextUI != null)
            {
                // If it's a UI text, find or create a Canvas parent
                var canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    scoreTextInstance.transform.SetParent(canvas.transform, false);
                    // Convert world position to screen position for UI
                    var mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        var screenPos = mainCamera.WorldToScreenPoint(worldPosition);
                        var rectTransform = scoreTextInstance.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                canvas.GetComponent<RectTransform>(),
                                screenPos,
                                canvas.worldCamera,
                                out var localPoint);
                            rectTransform.localPosition = localPoint;
                        }
                    }
                }

                tmpTextUI.text = score.ToString();
                return;
            }
        }

        if (tmpText != null) tmpText.text = score.ToString();
    }

    public void TogglePauseUI(bool isPaused, bool isGameActive)
    {
        if (isGameActive)
        {
            if (isPaused)
            {
                // Show pause screen
                UI_PauseScreen?.SetActive(true);

                // Select resume button when pausing
                if (btn_ResumeGame && eventSystem != null)
                    eventSystem.SetSelectedGameObject(btn_ResumeGame);
            }
            else
            {
                // Hide pause screen
                UI_PauseScreen?.SetActive(false);
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

    private IEnumerator ExitGameCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Application.Quit();
    }

    private IEnumerator GoToMainMenuCoroutine()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(0); // Load the Main Menu
    }

    private void InitializeScreenStates()
    {
        // On game start, show StartScreen and hide others
        UI_StartScreen?.SetActive(true);
        UI_GameOverScreen?.SetActive(false);
        UI_PauseScreen?.SetActive(false);
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

        // Find Camera_Movement
        var mainCam = GameObject.Find("Main Camera");
        if (mainCam != null)
            cameraMovement = mainCam.GetComponent<Camera_Movement>();

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
        //var _tmp = GameObject.Find("UI_Game_Interface_v2_Lightweight_LCD");
        //if (_tmp == null) _tmp = GameObject.Find("UI_Game_Interface_v2");
        //if (_tmp != null)
        //{
        //    var children = _tmp.GetComponentsInChildren<Transform>(true);
        //    foreach (var child in children)
        //    {
        //        if (child.name == "Text_Camera")
        //            Mobile_Cam_Txt = child.gameObject;
        //    }
        //}

        //_tmp = GameObject.Find("G_UI_Game_Interface_Mobile");
        //if (_tmp != null) UI_GameOverScreen = _tmp;

        //_tmp = GameObject.Find("G_UI_Game_Interface_Mobile_Part2");
        //if (_tmp != null) UI_StartScreen = _tmp;

        //_tmp = GameObject.Find("PauseAndView");
        //if (_tmp)
        //{
        //    var children = _tmp.GetComponentsInChildren<Transform>(true);
        //    foreach (var child in children)
        //    {
        //        if (child.name == "btn_Mobile_Pause")
        //            Mobile_PauseAndCam = child.gameObject;
        //    }
        //}

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
            if (Obj_UI_BestScore) Obj_UI_BestScore.text = GameManager.Instance.GetHighScore().ToString(); // Display Best score
        }

        // Subscribe to button events programmatically
        SubscribeToButtons();
    }

    private void SubscribeButton(string buttonName, Action action)
    {
        var buttonObj = GameObject.Find(buttonName);
        if (buttonObj == null) return;

        var button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action());
        }
    }

    private void SubscribeToButtons()
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null) return;

        // Find and subscribe to common UI buttons
        SubscribeButton("btn_InsertCoin", () => gameManager.F_InsertCoin_GameStart());
        SubscribeButton("btn_Resume_Game", () => gameManager.F_Pause_Game());
        SubscribeButton("btn_Restart_Yes", () => gameManager.F_InsertCoin_GameStart());
        SubscribeButton("btn_Restart_No", ExitGame);
        SubscribeButton("btn_Quit_Yes", () => gameManager.QuitGame());
        SubscribeButton("btn_Quit_No", () => gameManager.F_Quit_No());
        SubscribeButton("btn_Exit_Game", ExitGame);
        SubscribeButton("btn_GoToMainMenu", GoToMainMenu);
        SubscribeButton("btn_Back_Main_Menu", ExitGame);
        SubscribeButton("btn_Cam", ChangeCamera);
        SubscribeButton("btn_ChangeCam", ChangeCamera);

        // Debug buttons (only subscribe if needed - these might not exist in production builds)
        SubscribeButton("btn_Debug_Ball_Saver_Off", () => gameManager.F_Mode_Ball_Saver_Off());
        SubscribeButton("btn_Debug_Ball_Saver_On", () => gameManager.F_Mode_Ball_Saver_On(-1));
        SubscribeButton("btn_Debug_ExtraBall", () => gameManager.F_Mode_ExtraBall());
        SubscribeButton("btn_Debug_Init_All_Mission", () => gameManager.Init_All_Mission());
        SubscribeButton("btn_Debug_MultiBall", () => gameManager.F_Mode_MultiBall());
        SubscribeButton("btn_Debug_NewBall", () => gameManager.F_NewBall());
        SubscribeButton("btn_Debug_Pause_Game", () => gameManager.F_Pause_Game());
        SubscribeButton("btn_Debug_PlayMultiLeds", () => gameManager.PlayMultiLeds(0));
        SubscribeButton("btn_Debug_Start_Pause_Mode", () => gameManager.Start_Pause_Mode(-1));
        SubscribeButton("btn_Debug_Stop_Pause_Mode", () => gameManager.Stop_Pause_Mode());
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