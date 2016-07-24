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

        private GameField _gameField;
        private BlockComponent _touchedBlock;
        private float _startAnimationTime;
        private AudioSource _audioSource;
        private float _timer;
        private long _levelScore;
        private long _totalScore;
        private long _thresholdCurrent;
        private long _thresholdNext;
        private int _currentLevel;
        private bool _isPause;
        private bool _isGameOver;

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (GameManager)FindObjectOfType(typeof(GameManager));
                return _instance;
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = soundClickEvent;

            _gameField = new GameField(fieldWidth, fieldHeight);
            CameraAdjust.FitCamera(cam, _gameField, BlockFactory.Instance.sprites[0].blockSprite);

            // Перестроение поля.
            while (!_gameField.ValidateField())
            {
                _gameField = BlockDestroyer.DestroyField(_gameField);
                _gameField = new GameField(fieldWidth, fieldHeight);
            }

            _currentLevel = 0;
            InitNextLevel();
        }

        private void InitNextLevel()
        {
            _timer = timeLimit;
            _levelScore = 0;
            _thresholdNext = _thresholdNext + thresholdDelta;
            _thresholdCurrent = _thresholdNext;
            _currentLevel++;

            UIManager.Instance.SetLevel(_currentLevel);
            UIManager.Instance.SetLevelScore(_levelScore);
            UIManager.Instance.SetThresholdValue(_thresholdCurrent);
            UIManager.Instance.SetTimer(_timer);
        }

        // Таймеры.
        private void Update()
        {
            if (!_isGameOver)
            {
                // Уровень пройден. Перейти на следующий.
                if (_levelScore >= _thresholdCurrent && _timer > 0)
                {
                    InitNextLevel();
                }
                // Игра проиграна.
                else if (_timer <= 0 && _levelScore < _thresholdCurrent)
                {
                    _gameField.DisableBlockTouch();
                    UIManager.Instance.ShowDefeatScreen(_totalScore, _currentLevel);
                    _isGameOver = true;
                }
                else
                {
                    // Нет паузы.   
                    if (!_isPause)
                    {
                        // Обратный отсчет таймера.
                        _timer -= Time.deltaTime;
                        UIManager.Instance.SetTimer(_timer);
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
            if (_gameField.IsRebuildRowsOn)
            {
                if (_gameField.RebuildRows(blockAnimationSpeed, _startAnimationTime))
                {
                    _gameField.IsRebuildRowsOn = false;
                    _gameField.UpdateCols();

                    // Начать анимацию сдвига влево.
                    _gameField.IsRebuildColsOn = true;
                    _startAnimationTime = Time.time;
                }
            }

            // Анимация схлопывания колонок.
            if (_gameField.IsRebuildColsOn)
            {
                if (_gameField.RebuildCols(blockAnimationSpeed, _startAnimationTime))
                {
                    _gameField.IsRebuildColsOn = false;
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
            if (!_gameField.ValidateField())
            {
                // Ходов нет, а поле целое -> уничтожить поле.
                if (_gameField.IsFieldFull)
                    _gameField = BlockDestroyer.DestroyField(_gameField);

                // Начисление очков за полностью уничтоженное поле.
                if (_gameField.IsFieldEmpty)
                {
                    _totalScore += (long)(fieldDestroyCoefficient * _thresholdCurrent);
                    _levelScore += (long)(fieldDestroyCoefficient * _thresholdCurrent);
                    UIManager.Instance.SetLevelScore(_levelScore);
                }

                // Обновить ИП.
                _gameField.GenerateNewBlocks(
                    UIManager.Instance.redVessel.BonusCount,
                    UIManager.Instance.yellowVessel.BonusCount,
                    UIManager.Instance.blueVessel.BonusCount
                );

                UIManager.Instance.EmptyVessels();

                // Запустить анимацию.
                _startAnimationTime = Time.time;
                _gameField.IsRebuildRowsOn = true;
            }
        }

        /// <summary>
        /// Функция, срабатываемая в момент нажатия на блок пользователем.
        /// </summary>
        /// <param name="touched"></param>
        public void TouchBlockCallback(BlockComponent touched)
        {
            // Уничтожение блоков разрешено, только тогда, когда не запущена анимация.
            if (!_gameField.IsRebuildColsOn && !_gameField.IsRebuildRowsOn)
            {
                _touchedBlock = touched;

                Dictionary<BlockType, long> points = new Dictionary<BlockType, long>();
                int multiplier; // Общее значение бонуса умножения.

                // Уничтожение блоков ип.
                DestroyBlocks(touched, ref points, out multiplier);

                // Некоторые блоки были уничтожены.
                if (points.Any())
                {
                    UIManager.Instance.UpdateVessels(points);

                    long pts = GetTotalPoints(points) * multiplier;
                    _levelScore += pts;
                    _totalScore += pts;

                    UIManager.Instance.SetLevelScore(_levelScore);
                }
            }
        }

        /// <summary>
        /// Уничтожение блоков и обновление "мензурок".
        /// </summary>
        private void DestroyBlocks(BlockComponent touched, ref Dictionary<BlockType, long> points, out int multiplier)
        {
            multiplier = 1;
            List<BlockComponent> destroyList = BlockDestroyer.GetMatchedElements(_gameField, touched);

            if (destroyList.Count >= matchCount)
            {
                BlockDestroyer.CommitBonuses(ref destroyList, _gameField);
                points = BlockDestroyer.CalculatePoints(destroyList, matchCount, startBlockCost, deltaBlockCost);
                multiplier = BlockDestroyer.CalculateBonusMultiplier(destroyList);
                _audioSource.Play();
                destroyList.ForEach(i =>
                {
                    i.StartDestroy();
                });
            }
        }

        public void SetFieldItemNull(int x, int y)
        {
            _gameField.Field[x, y] = null;

            // Обрушить строки.
            _gameField.UpdateRows();

            // Начать анимацию "обрушения" колонок.
            _gameField.IsRebuildRowsOn = true;
            _startAnimationTime = Time.time;
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
            _isPause = true;
            _gameField.DisableBlockTouch();
        }

        /// <summary>
        /// Функция, срабатываемая при выключении паузы.
        /// </summary>
        public void PauseDisabledCallback()
        {
            _isPause = false;
            _gameField.EnableBlockTouch();
        }
        #endregion
    }
}