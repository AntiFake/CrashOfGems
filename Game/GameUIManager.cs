﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour {
    public GameManager gameManager;

    #region Экран "Игра"
    public Button btnPause;
    public Text timer;
    public Text totalScore;
    public Text threshold;
    public Text level;
    #endregion

    #region Экран "Пауза".
    public GameObject pauseScreen;

    public Button psBtnResume;
    public Button psBtnRestart;
    public Button psBtnExit;

    #endregion

    #region Экран "Игра окончена"

    public GameObject gameOverScreen;

    public Button goBtnRestart;
    public Text finalScore;
    public Text finalLevel;

    #endregion

    private void Awake()
    {
        // Экран "Игра".
        btnPause.onClick.AddListener(() => { OnPauseButtonClick(); });

        // Экран "Пауза".
        psBtnResume.onClick.AddListener(() => { OnResumeButtonClick(); });
        psBtnRestart.onClick.AddListener(() => { OnRestartButtonClick(); });
        psBtnExit.onClick.AddListener(() => { OnExitButtonClick(); });

        // Экран "Игра окончена".
        goBtnRestart.onClick.AddListener(() => { OnRestartButtonClick(); });
    }

    #region Функции управления ЭУ UI

    public void SetTimer(float timer)
    {
        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);

        // Определение цвета.
        if (timer <= 10)
            this.timer.color = Color.red;
        else
            this.timer.color = Color.white;

        this.timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void SetLevel(int lvl)
    {
        level.text = lvl.ToString();
    }

    public void SetLevelScore(long scoreValue)
    {
        totalScore.text = FormatScore(scoreValue);
    }

    public void SetThresholdValue(long thresholdValue)
    {
        threshold.text = FormatScore(thresholdValue);
    }

    public void ShowDefeatScreen(long score, int level)
    {
        gameOverScreen.gameObject.SetActive(true);
        SetFinalScore(score, level);
    }

    private void SetFinalScore(long score, int level)
    {
        finalScore.text = string.Format("Score: {0}", FormatScore(score));
        finalLevel.text = string.Format("Level: {0}", level);
    }

    #endregion

    #region Служебные функции
    /// <summary>
    /// Форматирование результата.
    /// </summary>
    /// <param name="value">Результат.</param>
    /// <returns></returns>
    private string FormatScore(long value)
    {
        string str = string.Empty;

        if (value > 9999 && value < 1000000)
            str = string.Format("{0}.{1}K", value / 1000, value % 1000 / 100);
        else if (value >= 1000000)
            str = string.Format("{0}.{1}M", value / 1000000, value % 1000000 / 100000);
        else
            str = value.ToString();

        return str;
    }
    #endregion

    #region События экрана "Игра"
    /// <summary>
    /// Нажатие на кнопку "Пауза".
    /// </summary>
    public void OnPauseButtonClick()
    {
        pauseScreen.SetActive(true);
        gameManager.PauseEnabledCallback();
    }
    #endregion

    #region События экрана "Пауза"

    public void OnResumeButtonClick()
    {
        pauseScreen.SetActive(false);
        gameManager.PauseDisabledCallback();
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    #endregion

    // Анимации.
    public void FixedUpdate()
    {

    }
}

