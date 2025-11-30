using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for loading scenes

public class LevelManager : MonoBehaviour
{
    [Header("Level Config")]
    public int level;
    public float levelDurationInRealMinutes = 3f;
    public int startTimeInHours = 6;
    public float gameMinutesPerRealMinute = 60f;

    [Header("Scene Management")]
    public string nextLevelSceneName;
    public string mainMenuSceneName = "Scene_MainMenu";

    [Header("UI References")]
    public TimerUI timerUI;
    public GameObject UI_HUD;
    public GameObject UI_Bubbles;
    public CheckPointManager checkPointManager;
    public GameObject UI_result;
    public ResultsPanelUI resultsPanelUI;

    [Header("Pause References")]
    public GameObject UI_Pause;
    public Button Btn_Pause;
    public Image pauseButtonImage;
    public Sprite pauseIcon;
    public Sprite resumeIcon;
    public TextMeshProUGUI pauseText;

    [Header("Other References")]
    public PlayerController playerController;

    private float _elapsedRealSeconds = 0f;
    private bool _levelEnded = false;
    private bool _isPaused = false;

    void Start()
    {

        Time.timeScale = 1f;
        float totalGameMinutes = levelDurationInRealMinutes * gameMinutesPerRealMinute;
        int endHour = startTimeInHours + (int)(totalGameMinutes / 60);
        int endMinute = (int)(totalGameMinutes % 60);

        timerUI.SetStartTime(startTimeInHours, 0);
        timerUI.SetEndTime(endHour, endMinute);

        timerUI.SetDay(level);

        UI_HUD.SetActive(true);
        UI_Bubbles.SetActive(true);
        UI_result.SetActive(false);
        UI_Pause.SetActive(false);
        UI_Pause.GetComponent<PausePanelUI>().SetMainMenu(mainMenuSceneName);

        //Need Redo
        pauseText.text = $"[{playerController.keyPause}]";

        Btn_Pause.onClick.AddListener(OnPauseButtonPressed);
    }

    void Update()
    {

        if (!_levelEnded && Input.GetKeyDown(playerController.keyPause))
        {
            TogglePause();
        }

        if (_isPaused || _levelEnded) return;

        _elapsedRealSeconds += Time.deltaTime;

        float elapsedGameMinutes = (_elapsedRealSeconds / 60f) * gameMinutesPerRealMinute;
        int currentHour = startTimeInHours + (int)(elapsedGameMinutes / 60);
        int currentMinute = (int)(elapsedGameMinutes % 60);

        timerUI.SetTime(currentHour, currentMinute);

        bool allTasksComplete = checkPointManager.AreAllTasksComplete;

        if (allTasksComplete)
        {
            EndLevel(true);
        }
        else if (_elapsedRealSeconds >= levelDurationInRealMinutes * 60f)
        {
            EndLevel(false);
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        UI_Bubbles.SetActive(!_isPaused);
        UI_Pause.SetActive(_isPaused);

        Time.timeScale = _isPaused ? 0f : 1f;

        if (pauseButtonImage != null)
        {
            if (_isPaused)
            {
                pauseButtonImage.sprite = resumeIcon;
            }
            else
            {
                pauseButtonImage.sprite = pauseIcon;
            }
        }
    }

    public void OnPauseButtonPressed()
    {
        TogglePause();
    }

    void EndLevel(bool allTaskFinish)
    {
        _levelEnded = true;
        Time.timeScale = 0f;

        //Debug.Log("Level Ended!");
        UI_HUD.SetActive(false);
        UI_Bubbles.SetActive(false);

        UI_result.SetActive(true);

        float baseMaxScore = 100f;

        float taskCompletionWeight = 0.6f;
        float taskPercentage = (float)checkPointManager.CompletedTasks / checkPointManager.TotalTasks;
        float taskScore = baseMaxScore * taskCompletionWeight * taskPercentage;

        float timeBonusWeight = 0.4f;
        float timeBonusMaxScoreWeight = 0.5f;
        float timePercentage = (float)(levelDurationInRealMinutes * 60f - _elapsedRealSeconds) / (levelDurationInRealMinutes * 60f);
        float timeScore = timePercentage > timeBonusMaxScoreWeight ? baseMaxScore * timeBonusWeight : baseMaxScore * timeBonusWeight * (timePercentage + 1 - timeBonusMaxScoreWeight);

        float score = taskScore + timeScore;

        resultsPanelUI.ShowResults(Mathf.RoundToInt(score), nextLevelSceneName, mainMenuSceneName, !allTaskFinish);
    }
}