// UI_Debug_Led : Description : use to test leds patterns

using UnityEngine;
using UnityEngine.UI;

public class UI_Debug_Led : MonoBehaviour
{
    #region --- Exposed Fields ---

    [Header("-> Global Pattern (Manage by ManagerGame)")]
    public bool Global_Led; // true if you want to test global pattern using more than one group of leds

    [Header("-> Leds Group (Mission, group of leds)")]
    public bool Group_Led; // true if you want to test a group leds.

    public GameObject Obj_Grp; // Connect the group of leds

    public Text Gui_Txt_Timer; // Connect a UI.Text

    #endregion

    #region --- Private Fields ---

    private GameManager gameManager; // Used to Access ManagerGame component (You find ManagerGame.js on ManagerGame object on the hierachy)

    private int anim;

    private int cmpt;
    private int HowManyAnim;
    private Manager_Led_Animation obj_Grp;

    private MissionIndex missionIndex; // Used to Access Mission_Index component (You find Mission_Index.js on each Mission)

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        if (Global_Led)
        {
            gameManager = GameManager.Instance; // --> Connect the Mission to <ManagerGame>() component. 
            HowManyAnim = gameManager.HowManyAnimation();
        }

        if (Group_Led)
        {
            obj_Grp = Obj_Grp.GetComponent<Manager_Led_Animation>();
            HowManyAnim = obj_Grp.HowManyAnimation();
        }

        Gui_Txt_Timer.text = cmpt.ToString();
    }

    #endregion

    #region --- Methods ---

    public void _PressButton()
    {
        cmpt++;
        cmpt = cmpt % HowManyAnim;
        Gui_Txt_Timer.text = cmpt.ToString();
    }

    public void PlayLedAnim()
    {
        if (Global_Led)
            gameManager.PlayMultiLeds(cmpt);
        if (Group_Led)
            obj_Grp.Play_New_Pattern(cmpt);
    }

    #endregion
}