﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Model;

namespace Alchemy.Level
{
    public class LevelManager : MonoBehaviour
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

        [Header("Корневой объект для подложки")]
        public GameObject substrateParent;

        [Header("Корневой объект для поля")]
        public GameObject gameFieldParent;

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
        private bool _isTouched;

        private static LevelManager _instance;
        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = (LevelManager)FindObjectOfType(typeof(LevelManager));
                return _instance;
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = soundClickEvent;

            _gameField = new GameField(fieldWidth, fieldHeight, true, substrateParent, gameFieldParent);
            CameraAdjust.FitCamera(cam, _gameField, GameManager.Instance.LevelModel.resources[0].sprite);

            // Перестроение поля.
            while (!_gameField.ValidateField())
            {
                _gameField = BlockDestroyer.DestroyField(_gameField);
                _gameField = new GameField(fieldWidth, fieldHeight, false, substrateParent, gameFieldParent);
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

            UILevelManager.Instance.SetLevel(_currentLevel);
            UILevelManager.Instance.SetLevelScore(_levelScore);
            UILevelManager.Instance.SetThresholdValue(_thresholdCurrent);
            UILevelManager.Instance.UpdateTimer(_timer);
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
                    UILevelManager.Instance.ShowDefeatScreen(_totalScore, _currentLevel);
                    _isGameOver = true;
                }
                else
                {
                    // Нет паузы.   
                    if (!_isPause)
                    {
                        // Обратный отсчет таймера.
                        _timer -= Time.deltaTime;
                        UILevelManager.Instance.UpdateTimer(_timer);
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
                    UILevelManager.Instance.SetLevelScore(_levelScore);
                }

                // Обновить игровое поле.
                _gameField.GenerateNewBlocks(gameFieldParent);

                // Запустить анимацию.
                _startAnimationTime = Time.time;
                _gameField.IsRebuildRowsOn = true;
            }
            // Срабатывает после каждого touch, если есть убранные элементы.
            else if (_isTouched)
            {
                // Обновить игровое поле.
                _gameField.GenerateNewBlocks(gameFieldParent);

                // Запустить анимацию.
                _startAnimationTime = Time.time;
                _gameField.IsRebuildRowsOn = true;
                _isTouched = false;
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

                Dictionary<ResourceType, long> points = new Dictionary<ResourceType, long>();

                // Уничтожение блоков ип.
                DestroyBlocks(touched, ref points);

                // Некоторые блоки были уничтожены.
                if (points.Any())
                {
                    long pts = GetTotalPoints(points);
                    _levelScore += pts;
                    _totalScore += pts;

                    UILevelManager.Instance.SetLevelScore(_levelScore);

                    _isTouched = true;
                }
            }
        }

        /// <summary>
        /// Уничтожение блоков.
        /// </summary>
        private void DestroyBlocks(BlockComponent touched, ref Dictionary<ResourceType, long> points)
        {
            List<BlockComponent> destroyList = BlockDestroyer.GetMatchedElements(_gameField, touched);

            if (destroyList.Count >= matchCount)
            {
                points = BlockDestroyer.CalculatePoints(destroyList, matchCount, startBlockCost, deltaBlockCost);
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

        private long GetTotalPoints(Dictionary<ResourceType, long> points)
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
