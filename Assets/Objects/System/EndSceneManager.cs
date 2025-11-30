using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainSceneName = "Scene_MainMenu";

    public void OnMainMenuButtonPressed()
    {
        SceneManager.LoadScene(mainSceneName);
    }
    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
