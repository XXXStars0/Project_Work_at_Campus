using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string firstLevelSceneName = "Scene_Level_0";
    public string levelSelectSceneName = "Scene_LevelSelect";

    [Header("Demo Level Selector")]
    public string secondLevelSceneName = "Scene_Level_1";
    public string thirdLevelSceneName = "Scene_Level_2";


    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }
    public void OnSelectLevelButtonPressed()
    {
        SceneManager.LoadScene(levelSelectSceneName);
    }

    //DEMO & DEBUG USE LEVEL SELECTOR

    public void On2ndButtonPressed()
    {
        SceneManager.LoadScene(secondLevelSceneName);
    }

    public void On3rdButtonPressed()
    {
        SceneManager.LoadScene(thirdLevelSceneName);
    }

    //
    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}