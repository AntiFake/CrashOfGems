using UnityEngine;
using CrashOfGems.Game;
using CrashOfGems.Cam;
using CrashOfGems.Components;
using CrashOfGems.Classes;
using System.Collections.Generic;
using CrashOfGems.Enums;
using System.Linq;

namespace CrashOfGems.Management
{
    public class GameManager : MonoBehaviour
    {

        #region Задаваемые параметры игрового поля
        [Header("Ширина поля")]
        public int fieldWidth = 5;

        [Header("Высота поля")]
        public int fieldHeight = 5;

        [Header("Скорость анимации")]
        public float blockAnimationSpeed = 5f;

        [Header("Игровая камера")]
        public Camera cam;

        [Header("Количество для совмещения")]
        public int matchCount;

        [Header("Клик по блоку")]
        public AudioClip soundClickEvent;

        [Header("Лимит по времени, с")]
        public float timeLimit;

        [Header("Начальный порог")]
        public long threshold;

        [Header("Сдвиг порога")]
        public long thresholdDelta;

        [Header("Начальная цена за блок")]
        public int startBlockCost = 10;

        [Header("Дельта цены за блок")]
        public int deltaBlockCost = 5;

        [Header("Доля от уровня за уничтожения всего поля")]
        public float fieldDestroyCoefficient = 0.25f;

        #endregion

        private GameField gameField;
        private BlockComponent touchedBlock;
        private float startAnimationTime;
        private AudioSource audioSource;
        private float timer;
        private long levelScore;
        private long totalScore;
        private long thresholdCurrent;
        private long thresholdNext;
        private int currentLevel;
        private bool isPause;
        private bool isGameOver;

        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = (GameManager)FindObjectOfType(typeof(GameManager));
                return instance;
            }
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = soundClickEvent;

            gameField = new GameField(fieldWidth, fieldHeight);
            CameraAdjust.FitCamera(cam, gameField, BlockFactory.Instance.sprites[0].blockSprite);

            // Перестроение поля.
            while (!gameField.ValidateField())
            {
                gameField = BlockDestroyer.DestroyField(gameField);
                gameField = new GameField(fieldWidth, fieldHeight);
            }

            currentLevel = 0;
            InitNextLevel();
        }

        private void InitNextLevel()
        {
            timer = timeLimit;
            levelScore = 0;
            thresholdNext = thresholdNext + thresholdDelta;
            thresholdCurrent = thresholdNext;
            currentLevel++;

            UIManager.Instance.SetLevel(currentLevel);
            UIManager.Instance.SetLevelScore(levelScore);
            UIManager.Instance.SetThresholdValue(thresholdCurrent);
            UIManager.Instance.SetTimer(timer);
        }

        // Таймеры.
        private void Update()
        {
            if (!isGameOver)
            {
                // Уровень пройден. Перейти на следующий.
                if (levelScore >= thresholdCurrent && timer > 0)
                {
                    InitNextLevel();
                }
                // Игра проиграна.
                else if (timer <= 0 && levelScore < thresholdCurrent)
                {
                    gameField.DisableBlockTouch();
                    UIManager.Instance.ShowDefeatScreen(totalScore, currentLevel);
                    isGameOver = true;
                }
                else
                {
                    // Нет паузы.   
                    if (!isPause)
                    {
                        // Обратный отсчет таймера.
                        timer -= Time.deltaTime;
                        UIManager.Instance.SetTimer(timer);
                    }
                }
            }
        }

        /// <summary>
        /// Проигрывание анимаций.
        /// </summary>
        private void FixedUpdate()
        {
            // Анимация обрушения колонок.
            if (gameField.IsRebuildRowsOn)
            {
                if (gameField.RebuildRows(blockAnimationSpeed, startAnimationTime))
                {
                    gameField.IsRebuildRowsOn = false;
                    gameField.UpdateCols();

                    // Начать анимацию сдвига влево.
                    gameField.IsRebuildColsOn = true;
                    startAnimationTime = Time.time;
                }
            }

            // Анимация схлопывания колонок.
            if (gameField.IsRebuildColsOn)
            {
                if (gameField.RebuildCols(blockAnimationSpeed, startAnimationTime))
                {
                    gameField.IsRebuildColsOn = false;
                    RebuildAnimationEndCallback();
                }
            }
        }

        #region Callbacks
        /// <summary>
        /// Callback-функция, вызываемая после проигрывания всех анимаций перестроения поля.
        /// </summary>
        private void RebuildAnimationEndCallback()
        {
            // Проверка существования вариантов ходов.
            if (!gameField.ValidateField())
            {
                // Ходов нет, а поле целое -> уничтожить поле.
                if (gameField.IsFieldFull)
                    gameField = BlockDestroyer.DestroyField(gameField);

                // Начисление очков за полностью уничтоженное поле.
                if (gameField.IsFieldEmpty)
                {
                    totalScore += (long)(fieldDestroyCoefficient * thresholdCurrent);
                    levelScore += (long)(fieldDestroyCoefficient * thresholdCurrent);
                    UIManager.Instance.SetLevelScore(levelScore);
                }

                // Обновить ИП.
                gameField.GenerateNewBlocks(
                    UIManager.Instance.redVessel.BonusCount,
                    UIManager.Instance.yellowVessel.BonusCount,
                    UIManager.Instance.blueVessel.BonusCount
                );

                UIManager.Instance.EmptyVessels();

                // Запустить анимацию.
                startAnimationTime = Time.time;
                gameField.IsRebuildRowsOn = true;
            }
        }

        /// <summary>
        /// Функция, срабатываемая в момент нажатия на блок пользователем.
        /// </summary>
        /// <param name="touched"></param>
        public void TouchBlockCallback(BlockComponent touched)
        {
            // Уничтожение блоков разрешено, только тогда, когда не запущена анимация.
            if (!gameField.IsRebuildColsOn && !gameField.IsRebuildRowsOn)
            {
                touchedBlock = touched;

                Dictionary<BlockType, long> points = new Dictionary<BlockType, long>();
                int multiplier; // Общее значение бонуса умножения.

                // Уничтожение блоков ип.
                DestroyBlocks(touched, ref points, out multiplier);

                // Некоторые блоки были уничтожены.
                if (points.Any())
                {
                    UIManager.Instance.UpdateVessels(points);

                    long pts = GetTotalPoints(points) * multiplier;
                    levelScore += pts;
                    totalScore += pts;

                    UIManager.Instance.SetLevelScore(levelScore);
                    
                    // Обрушить строки.
                    gameField.UpdateRows();

                    // Начать анимацию "обрушения" колонок.
                    gameField.IsRebuildRowsOn = true;
                    startAnimationTime = Time.time;
                }
            }
        }

        /// <summary>
        /// Уничтожение блоков и обновление "мензурок".
        /// </summary>
        private void DestroyBlocks(BlockComponent touched, ref Dictionary<BlockType, long> points, out int multiplier)
        {
            multiplier = 1;
            List<BlockComponent> destroyList = BlockDestroyer.GetMatchedElements(gameField, touched);

            if (destroyList.Count >= matchCount)
            {
                BlockDestroyer.CommitBonuses(ref destroyList, gameField);
                points = BlockDestroyer.CalculatePoints(destroyList, matchCount, startBlockCost, deltaBlockCost);
                multiplier = BlockDestroyer.CalculateBonusMultiplier(destroyList);
                audioSource.Play();
                destroyList.ForEach(i =>
                {
                    i.Destroy();
                    gameField.Field[i.x, i.y] = null;
                });
            }
        }

        private long GetTotalPoints(Dictionary<BlockType, long> points)
        {
            return points.Sum(i => i.Value);
        }

        /// <summary>
        /// Функция, срабатываемая при установке паузы.
        /// </summary>
        public void PauseEnabledCallback()
        {
            isPause = true;
            gameField.DisableBlockTouch();
        }

        /// <summary>
        /// Функция, срабатываемая при выключении паузы.
        /// </summary>
        public void PauseDisabledCallback()
        {
            isPause = false;
            gameField.EnableBlockTouch();
        }
        #endregion
    }
}