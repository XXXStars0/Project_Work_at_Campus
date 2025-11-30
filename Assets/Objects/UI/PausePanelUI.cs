using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PausePanelUI : MonoBehaviour
{

    public Button retryButton;
    public Button menuButton;

    private string _mainMenuScene;

    void Awake()
    {
        //Time.timeScale = 0f;
        retryButton.onClick.AddListener(OnRetry);
        menuButton.onClick.AddListener(OnMenu);
    }

    public void OnRetry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMenu()
    {
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void SetMainMenu(string mainMenuScene)
    {
        _mainMenuScene = mainMenuScene;
    }
}
