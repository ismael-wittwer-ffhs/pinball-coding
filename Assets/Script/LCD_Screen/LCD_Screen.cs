// LCD_Screen : Description : LCD : Choose the LCD Screen position relative to object cam. Useful when screen resolution change.

using UnityEngine;

public class LcdScreen : MonoBehaviour
{
    #region --- Exposed Fields ---

    public Camera cam; // The reference Camera
    public CameraSmoothFollow MainCam;

    public float Screen_Position_X = 0.15f; // LCD Screen Position. 0 = left Corner. 	1 = Right Corner  
    public float Screen_Position_Y = .9f; // LCD Screen Position. 0 = bottom Corner. 	1 = Up Corner  
    public float Screen_Position_Z = 8.5f; // LCD Screen Position. 0 = bottom Corner. 	1 = Up Corner  
    public GameObject lCD_Screen;

    #endregion

    #region --- Private Fields ---

    private bool OneTime = true;

    #endregion

    #region --- Unity Methods ---

    private void Start()
    {
        if (Screen.width < Screen.height)
            OneTime = false;
        else
            OneTime = true;

        lCD_Screen.transform.position = cam.ViewportToWorldPoint( // --> Choose the LCD Screen position relative to object cam
            new Vector3(Screen_Position_X, Screen_Position_Y, Screen_Position_Z)); // Transforms position from viewport space into world space.

        var tmp = GameObject.Find("Pivot_Cam");

        MainCam = tmp.GetComponent<CameraSmoothFollow>();
    }


    private void Update()
    {
        if (Screen.width < Screen.height && (MainCam.F_ReturnLastCam() == 3 || MainCam.F_ReturnLastCam() == 4))
        {
            lCD_Screen.transform.position = cam.ViewportToWorldPoint( // --> Choose the LCD Screen position relative to object cam
                new Vector3(.5f, .94f, Screen_Position_Z)); // Transforms position from viewport space into world space.

            if (OneTime)
            {
                lCD_Screen.SetActive(true);
                OneTime = false;
            }
        }
        else if (Screen.width < Screen.height && MainCam.F_ReturnLastCam() != 3 && MainCam.F_ReturnLastCam() != 4)
        {
            lCD_Screen.transform.position = cam.ViewportToWorldPoint( // --> Choose the LCD Screen position relative to object cam
                new Vector3(.5f, Screen_Position_Y, Screen_Position_Z)); // Transforms position from viewport space into world space.

            if (!OneTime)
            {
                lCD_Screen.SetActive(false);
                OneTime = true;
            }
        }
        else if (Screen.width > Screen.height)
        {
            lCD_Screen.transform.position = cam.ViewportToWorldPoint( // --> Choose the LCD Screen position relative to object cam
                new Vector3(Screen_Position_X, Screen_Position_Y, Screen_Position_Z)); // Transforms position from viewport space into world space.
            if (OneTime)
            {
                lCD_Screen.SetActive(true);
                OneTime = false;
            }
        }
    }

    #endregion
}