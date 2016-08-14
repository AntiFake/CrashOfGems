﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Alchemy.Level
{
    public class UILevelManager : MonoBehaviour
    {
        #region Экран "Игра"
        public Button btnPause;
        public Text totalScore;
        public Text threshold;
        public Text level;
        public UITimerComponent timer;
        #endregion

        #region Экран "Пауза".
        public GameObject pauseScreen;

        public Button psBtnResume;
        public Button psBtnRestart;

        #endregion

        #region Экран "Игра окончена"

        public GameObject gameOverScreen;

        public Button goBtnRestart;
        public Text finalScore;
        public Text finalLevel;

        #endregion

        private static UILevelManager _instance;
        public static UILevelManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (UILevelManager)FindObjectOfType(typeof(UILevelManager));
                return _instance;
            }
        }

        private void Awake()
        {
            // Экран "Игра".
            btnPause.onClick.AddListener(() => { OnPauseButtonClick(); });

            // Экран "Пауза".
            psBtnResume.onClick.AddListener(() => { OnResumeButtonClick(); });
            psBtnRestart.onClick.AddListener(() => { OnRestartButtonClick(); });

            // Экран "Игра окончена".
            goBtnRestart.onClick.AddListener(() => { OnRestartButtonClick(); });
        }

        #region Функции управления ЭУ UI

        public void UpdateTimer(float timerValue)
        {
            timer.SetTimer(timerValue);
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
            LevelManager.Instance.PauseEnabledCallback();
        }
        #endregion

        #region События экрана "Пауза"

        public void OnResumeButtonClick()
        {
            pauseScreen.SetActive(false);
            LevelManager.Instance.PauseDisabledCallback();
        }

        public void OnRestartButtonClick()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        // Анимации.
        public void FixedUpdate()
        {

        }
    }
}
