// Description : 
#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


public class w_Version : EditorWindow
{
    private Vector2 scrollPosAll;
  



    [MenuItem("Pinball Creator/Other/w_Version")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(w_Version));
    }

    #region Init Inspector Color
    private Texture2D MakeTex(int width, int height, Color col)
    {                       // use to change the GUIStyle
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private List<Texture2D> listTex = new List<Texture2D>();
    public List<GUIStyle> listGUIStyle = new List<GUIStyle>();
    private List<Color> listColor = new List<Color>();
    #endregion



    void OnEnable()
    {
        #region

        #endregion
    }

    void OnGUI()
    {
        #region
        //--> Scrollview
        scrollPosAll = EditorGUILayout.BeginScrollView(scrollPosAll);
        //--> Window description

        Version();

      

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndScrollView();
        #endregion
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

   void Version()
    {
        EditorGUILayout.LabelField("Current", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Pinball Creator Unity 2022.3.14f1 LTS.");

        EditorGUILayout.LabelField("-[Correction] Bonus Score for last ball. manager_Game.cs.");
        EditorGUILayout.LabelField("-[Correction] UI_Game_Interface_v2_Lightweight_LCD_WithSaveName_ prefab.");
        EditorGUILayout.LabelField("-Remove Obsolete files.");


        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Old", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Pinball Creator Unity 2022.1.0f1.");

        EditorGUILayout.LabelField("-[Change] velocity (script) Unity 2022.");
        EditorGUILayout.LabelField("   Modified scripts: Multiball.cs | Debug_Test_Ball.cs | Hole.cs | Slingshot.cs | Bumper.cs.");
        EditorGUILayout.LabelField("-[Correction] CameraSmoothFollow.cs (deltaTime).");
      


        EditorGUILayout.LabelField("");
    }

}
#endif