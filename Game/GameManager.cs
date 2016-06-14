using UnityEngine;
using Match3.Main;
using Match3.Cam;
using Match3.Components;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    #region Задаваемые параметры игрового поля
    [Header("Ширина поля")]
	public int fieldWidth = 5;

	[Header("Высота поля")]
	public int fieldHeight = 5;

	[Header("Скорость анимации")]
	public float blockAnimationSpeed = 5f;

	[Header("Спрайты плиток")]
	public Sprite[] sprites;

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

    [Header("UI контроллер")]
    public GameUIManager UIManager;
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
    private MatchDestroyAlgorithm matchDestroyAlgorithm;

	private void Awake ()
	{
        matchDestroyAlgorithm = new MatchDestroyAlgorithm();

		audioSource = GetComponent<AudioSource>();
		audioSource.clip = soundClickEvent;

		gameField = new GameField(this, fieldWidth, fieldHeight, sprites);
		CameraAdjust.FitCamera(cam, gameField);

        // Перестроение поля.
		while (!gameField.ValidateField())
		{
            gameField = BlockDestroyer.DestroyField(gameField);
            gameField = new GameField(this, fieldWidth, fieldHeight, sprites);
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

        UIManager.SetLevel(currentLevel);
        UIManager.SetLevelScore(levelScore);
        UIManager.SetThresholdValue(thresholdCurrent);
        UIManager.SetTimer(timer);
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
                UIManager.ShowDefeatScreen(totalScore, currentLevel);
                isGameOver = true;
            }
            else
            {
                // Нет паузы.   
                if (!isPause)
                {
                    // Обратный отсчет таймера.
                    timer -= Time.deltaTime;
                    UIManager.SetTimer(timer);
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

    /// <summary>
    /// Callback-функция, вызываемая после проигрывания всех анимаций перестроения поля.
    /// </summary>
    private void RebuildAnimationEndCallback()
    {
        // Проверка существования вариантов ходов.
        if (!gameField.ValidateField())
        {
            // Ходов нет, а поле целое -> -уничтожить поле.
            if (gameField.IsFieldFull)
                gameField = BlockDestroyer.DestroyField(gameField);

            // Обновить ИП.
            gameField.GenerateNewBlocks();

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

            // Уничтожение блоков ип.
            long points;
            gameField = BlockDestroyer.DestroyBlocks(matchDestroyAlgorithm, gameField, touchedBlock, matchCount, audioSource, out points);
            levelScore += points;
            totalScore += points;

            UIManager.SetLevelScore(levelScore);

            // Обрушить строки.
            gameField.UpdateRows();

            // Начать анимацию "обрушения" колонок.
            gameField.IsRebuildRowsOn = true;
            startAnimationTime = Time.time;
        }
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
}
