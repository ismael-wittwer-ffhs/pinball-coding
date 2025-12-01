// Manager_Led_Animation : Description : 

using System;
using UnityEngine;

public class Manager_Led_Animation : MonoBehaviour
{
    #region --- Nested Types ---

    [Serializable]
    public class List_Led_Pattern
    {
        #region --- Exposed Fields ---

        public string[] pattern;

        #endregion
    }

    #endregion

    #region --- Exposed Fields ---

    [Header("Led Animation associated only with the Group_Leds_ExtraBall_BallSaver")]
    public bool b_extraBall_or_BallSaver; // Special condition for extraBall and BallSaver Leds. ExtraBall and ballSaver are manage by the ManagerGame.js script

    [Header("Led Animation associated with a Group_Led")]
    public bool b_Leds_Grp; // If it is a animation = true // 

    [Header("Led Animation associated with a mission")]
    public bool b_Mission = true; // If it is a mission = true	//

    [Header("Led Animation associated only with the Group_Leds_Missions")]
    public bool b_Mission_Leds_Grp; // Use only for the led animation that indicate if missions are complete or not

    [Header("Led Animation associated only with Mission Led_Mission_InProgress and Led_Part1_InProgress")]
    public bool b_Mission_Leds_Mission_Part;

    [Header("Led Animation associated only with the Group_Leds_Multiplier")]
    public bool b_Multiplier; // Special condition for Multiplier. Multipiler is manage by the ManagerGame.js script

    public float timeBetweenTwoLight = .1f;

    public GameObject[] obj_Led;
    public List_Led_Pattern[] list_Led_Pattern = new List_Led_Pattern[1];

    #endregion

    #region --- Private Fields ---

    private bool b_Pause;

    private bool Led_Anim_isPlaying;
    private ChangeSpriteRenderer[] Led_Renderer;
    private float target;
    private float Timer;
    private float TimeScale;

    private GameManager gameManager;
    private int count;
    private int isPlaying_Pattern;
    private string tmp_Last_State;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        gameManager = GameManager.Instance; // Connection with the gameObject : "ManagerGame"

        TimeScale = Time.timeScale;
        target += timeBetweenTwoLight * TimeScale;
    }

    private void Update()
    {
        if (Led_Anim_isPlaying && !b_Pause)
        {
            Timer = Mathf.MoveTowards(Timer, target, Time.deltaTime);
            if (Timer == target && count < list_Led_Pattern[isPlaying_Pattern].pattern.Length)
            {
                // PLay anmtion Pattern : If pause game the animation could be paused
                Animation_Leds(isPlaying_Pattern);
                count++;
                target += timeBetweenTwoLight * TimeScale;
            }

            if (count == list_Led_Pattern[isPlaying_Pattern].pattern.Length)
            {
                // Play the last element of the pattern
                if (Timer == target) count++;
                if (count == list_Led_Pattern[isPlaying_Pattern].pattern.Length + 1)
                {
                    // Init Animation
                    //Debug.Log("End of the animation : " + this.name);
                    if (gameManager != null) gameManager.CheckGlobalAnimationEnded();
                    count = 0;
                    target = 0;
                    Timer = 0;
                    Led_Anim_isPlaying = false; // Init script
                    if (b_Mission)
                    {
                        SendMessage("Init_Leds_State"); // Init Led Object with the mission's scripts
                    }
                    else if (b_extraBall_or_BallSaver && gameManager != null)
                    {
                        // Special Condition to initialize the BallSaver and Extraball leds after a pattern. 	
                        if (gameManager.BExtraBall) Led_Renderer[0].F_ChangeSprite_On(); // We check BallSaver and ExtraBall states directly from ManagerGame.js script
                        else Led_Renderer[0].F_ChangeSprite_Off();

                        if (gameManager.BBallSaver && gameManager != null) Led_Renderer[1].F_ChangeSprite_On();
                        else Led_Renderer[1].F_ChangeSprite_Off();
                    }
                    else if (b_Multiplier)
                    {
                        init_Multiplier_Leds(); // Special Condition to initialize the Multiplier leds  
                    }
                    else if (b_Leds_Grp)
                    {
                        for (var j = 0; j < obj_Led.Length; j++)
                        {
                            //Led_Renderer[j].F_ChangeSprite_Off();											// Switch off the leds
                        }
                    }
                    else if (b_Mission_Leds_Grp)
                    {
                        for (var j = 0; j < obj_Led.Length; j++)
                        {
                            if (Led_Renderer[j].Led_Mission_State())
                                Led_Renderer[j].F_ChangeSprite_On(); // Switch on the leds
                            else
                                Led_Renderer[j].F_ChangeSprite_Off(); // Switch off the leds	
                        }
                    }
                    else if (b_Mission_Leds_Mission_Part)
                    {
                        for (var j = 0; j < obj_Led.Length; j++)
                        {
                            if (Led_Renderer[j].F_led_Part_InProgress_State() == 1)
                                Led_Renderer[j].F_ChangeSprite_On(); // Switch on the leds
                            else
                                Led_Renderer[j].F_ChangeSprite_Off(); // Switch off the leds	
                        }
                    }


                    for (var i = 0; i < obj_Led.Length; i++) Led_Renderer[i].F_On_Blink_Switch(); // Start Blinking the light
                }
            }
        }
    }

    #endregion

    #region --- Methods ---

    public void Animation_Leds(int num)
    {
        Led_Anim_isPlaying = false;
        isPlaying_Pattern = num;

        for (var j = 0; j < obj_Led.Length; j++)
        {
            var tmp = list_Led_Pattern[num].pattern[count][j];
            if (tmp.ToString() == "1")
                Led_Renderer[j].F_ChangeSprite_On();
            else
                Led_Renderer[j].F_ChangeSprite_Off();
        }

        Led_Anim_isPlaying = true;
    }

    public int HowManyAnimation()
    {
        return list_Led_Pattern.Length;
    }


    public void init_Multiplier_Leds()
    {
        // Special Condition to initialize the Multiplier leds  
        if (gameManager != null)
        {
            var tmp_multi = gameManager.Multiplier; // We check Multiplier states directly from ManagerGame.js script 
            tmp_multi = tmp_multi / 2;
            if (tmp_multi < 1) // multiplier = 1 so all the leds are switch off
                for (var i = 0; i < obj_Led.Length; i++)
                    Led_Renderer[i].F_ChangeSprite_Off();
            else
                for (var j = 0; j < obj_Led.Length; j++)
                {
                    // if multiplier is greater than 1 
                    if (j < tmp_multi) // if tmp_multi = 1, switch on the first led, tmp_multi = 2, switch on the first two leds ...,
                        Led_Renderer[j].F_ChangeSprite_On();
                    else
                        Led_Renderer[j].F_ChangeSprite_Off();
                }
        }
    }

    public void Init_Obj_Led_Animation(GameObject[] tmp_obj_Led)
    {
        if (obj_Led.Length == 0)
        {
            obj_Led = new GameObject[tmp_obj_Led.Length];
            obj_Led = tmp_obj_Led;
        }

        Led_Renderer = new ChangeSpriteRenderer[obj_Led.Length];
        for (var k = 0; k < obj_Led.Length; k++)
        {
            if (obj_Led[k] != null)
                Led_Renderer[k] = obj_Led[k].GetComponent<ChangeSpriteRenderer>();
        }


        if (list_Led_Pattern[0].pattern.Length == 0)
        {
            list_Led_Pattern[0].pattern = new string[obj_Led.Length * 2 + 1];

            for (var j = 0; j < obj_Led.Length * 2 + 1; j++)
            {
                var temp_string = "";
                for (var i = 0; i < obj_Led.Length; i++)
                {
                    if (j < obj_Led.Length * 2)
                    {
                        if (j % obj_Led.Length == i)
                            temp_string += "1";
                        else
                            temp_string += "0";
                    }
                    else
                    {
                        temp_string += "0";
                    }
                }

                list_Led_Pattern[0].pattern[j] = temp_string;
            }
        }
    }


    public void Pause_Anim()
    {
        if (!b_Pause) Start_Pause_Anim();
        else Stop_Pause_Anim();
    }


    public void Play_New_Pattern(int num)
    {
        // CALL THIS FUNCTION TO PLAY A NEW PATTERN
        if (!b_Pause)
        {
            for (var i = 0; i < obj_Led.Length; i++) Led_Renderer[i].F_Off_Blink_Switch(); // Stop Blinking each light

            count = 0;
            target = 0;
            Timer = 0;
            Led_Anim_isPlaying = false;
            Animation_Leds(num);
        }
    }

    public void Start_Pause_Anim() { b_Pause = true; }

    public void Stop_Pause_Anim() { b_Pause = false; }

    #endregion
}