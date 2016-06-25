using UnityEngine;
using CrashOfGems.Components;
using System.Collections.Generic;
using System.Linq;
using CrashOfGems.Classes;
using CrashOfGems.Management;
using CrashOfGems.Enums;

namespace CrashOfGems.Game
{
    public class GameField
    {
        private BlockSprite[] sprites;
        private float spriteWidth;
        private float spriteHeight;
        private GameManager gameManager;
        private GameObject[,] field;
        private int width;
        private int height;
        private bool isRebuildRowsOn;
        private bool isRebuildColsOn;

        // Переменные
        private System.Random rnd;

        #region Свойства
        public bool IsRebuildRowsOn { get { return isRebuildRowsOn; } set { isRebuildRowsOn = value; } }
        public bool IsRebuildColsOn { get { return isRebuildColsOn; } set { isRebuildColsOn = value; } }
        public int FieldWidth { get { return width; } }
        public GameObject[,] Field { get { return field; } set { field = value; } }
        public int FieldHeight { get { return height; } }
        public bool IsFull
        {
            get
            {
                int notNullCount = 0;
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        if (field[x, y] != null)
                            notNullCount++;
                    }
                }

                return notNullCount == width * height;
            }
        }
        public bool IsEmpty
        {
            get
            {
                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        if (field[x, y] != null)
                            return false;
                    }
                }

                return true;
            }
        }
        
        #endregion

        public GameField(GameManager gm, int width, int height, BlockSprite[] sprites)
        {
            this.sprites = sprites;
            this.width = width;
            this.height = height;
            this.gameManager = gm;

            rnd = new System.Random();
            spriteWidth = sprites[0].blockSprite.bounds.size.x;
            spriteHeight = sprites[0].blockSprite.bounds.size.y;
            GenerateField();
        }

        #region Генерация поля

        /// <summary>
        /// Генерация игрового поля.
        /// </summary>
        private void GenerateField()
        {
            field = new GameObject[height, width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    field[x, y] = CreateNewFieldItem(x, y, new Vector2(y * spriteWidth, x * spriteHeight), gameManager.blockPrefab, BonusType.None);
                }
            }
        }

        /// <summary>
        /// Создание нового block-а.
        /// </summary>
        private GameObject CreateNewFieldItem(int x, int y, Vector2 pos, GameObject prefab, BonusType bonusType)
        {
            var block = (GameObject)GameObject.Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);

            block.name = string.Format("{0};{1}", x, y);
            int spriteNumber = GetRandomSpriteNumber();
            BlockType blockType = sprites[spriteNumber].blockType;

            block = UpdateBlockComponent(block, x, y, blockType);
            block = UpdateSpriteRendererComponent(block, x, y, spriteNumber, bonusType);

            if (bonusType != BonusType.None)
                AddBonusComponent(block, bonusType);

            return block;
        }

        private GameObject UpdateBlockComponent(GameObject block, int x, int y, BlockType blockType)
        {
            BlockComponent blockComponent = block.GetComponent<BlockComponent>();
            blockComponent.x = x;
            blockComponent.y = y;
            blockComponent.type = blockType;
            blockComponent.gameManager = gameManager;

            return block;
        }

        private GameObject UpdateSpriteRendererComponent(GameObject block, int x, int y, int spriteNumber, BonusType bonusType)
        {
            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();

            switch (bonusType)
            {
                case BonusType.None:
                    sr.sprite = sprites[spriteNumber].blockSprite;
                    break;
                case BonusType.Bomb:
                    sr.sprite = sprites[spriteNumber].bombSprite;
                    break;
                case BonusType.Multiplication:
                    sr.sprite = sprites[spriteNumber].multiplicationSprite;
                    break;
                case BonusType.Lightning:
                    sr.sprite = sprites[spriteNumber].lightningSprite;
                    break;
            }

            return block;
        }

        private GameObject AddBonusComponent(GameObject bonusBlock, BonusType bonusType)
        {
            BonusComponent bonus = bonusBlock.AddComponent<BonusComponent>();
            bonus.type = bonusType;
            bonusBlock.GetComponent<BlockComponent>().bonus = bonus;

            if (bonusType == BonusType.Multiplication)
                bonus.value = 2;

            if (bonusType == BonusType.Lightning)
                bonus.value = 3;

            return bonusBlock;
        }

        /// <summary>
        /// Получение случайного спрайта из коллекции.
        /// </summary>
        /// <returns></returns>
        private int GetRandomSpriteNumber()
        {
            return rnd.Next(0, sprites.Length);
        }
        #endregion

        #region Перестроение
        /// <summary>
        /// Пересобрать строки. Все элементы перемещаются вниз.
        /// </summary>
        public void UpdateRows()
        {
            // Новая позиция по x.
            GameObject[,] gf = new GameObject[height, width];
            int newX;
            BlockComponent bc;

            for (int y = 0; y < width; y++)
            {
                newX = 0;
                for (int x = 0; x < height; x++)
                {
                    // Если блок не удален.
                    if (field[x, y] != null)
                    {
                        gf[newX, y] = field[x, y];
                        bc = gf[newX, y].GetComponent<BlockComponent>();
                        bc.x = newX;
                        gf[newX, y].name = string.Format("{0};{1}", newX, y);
                        newX++;
                    }
                }
            }
            field = gf;
        }

        /// <summary>
        /// Пересобрать колонки. Реализован сдвиг влево.
        /// </summary>
        public void UpdateCols()
        {
            GameObject[,] gf = new GameObject[height, width];
            BlockComponent bc;
            int newY = 0;

            for (int y = 0; y < width; y++)
            {
                if (field[0, y] != null)
                {
                    for (int x = 0; x < height; x++)
                    {
                        gf[x, newY] = field[x, y];
                        if (field[x, y] != null)
                        {
                            bc = gf[x, newY].GetComponent<BlockComponent>();
                            bc.y = newY;
                            gf[x, newY].name = string.Format("{0};{1}", x, newY);
                        }
                    }
                    newY++;
                }
            }
            
            field = gf;
            isRebuildColsOn = true;
        }

        /// <summary>
        /// Получение пустых позиций на ИП.
        /// </summary>
        private List<KeyValuePair<int, int>> GetEmptyPositions()
        {
            List<KeyValuePair<int, int>> emptyPositions = new List<KeyValuePair<int, int>>();

            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    if (field[x, y] == null)
                        emptyPositions.Add(new KeyValuePair<int, int>(x, y));
                }
            }

            return emptyPositions;
        }

        /// <summary>
        /// Функция для получения случайных для определенного бонуса.
        /// </summary>
        private List<KeyValuePair<int, int>> GetRandomBonusPositions(ref List<KeyValuePair<int, int>> emptyPositions, int bonusCount)
        {
            if (bonusCount == 0)
                return null;

            if (emptyPositions.Count < bonusCount)
                bonusCount = emptyPositions.Count;

            List<KeyValuePair<int, int>> bonusPositions = new List<KeyValuePair<int, int>>();
            int count = 0, pos;
            while (count < bonusCount)
            {
                pos = rnd.Next(0, emptyPositions.Count);
                bonusPositions.Add(new KeyValuePair<int, int>(emptyPositions[pos].Key, emptyPositions[pos].Value));
                emptyPositions.RemoveAt(pos);
                count++;
            }

            return bonusPositions;
        }

        /// <summary>
        /// Достраивает на место удаленных элементов новые. 
        /// При этом определяет позиции этих элементов за пределами экрана для последующей анимации.
        /// </summary>
        public void GenerateNewBlocks(int bombCount, int multiplicationCount, int lightningCount /*+ другие бонусы как появятся*/)
        {
            int sortNullX;
            Vector2 pos;
            List<KeyValuePair<int, int>> emptyPositions = GetEmptyPositions();
            List<KeyValuePair<int, int>> bombPositions = GetRandomBonusPositions(ref emptyPositions, bombCount);
            List<KeyValuePair<int, int>> multiplicationPositions = GetRandomBonusPositions(ref emptyPositions, multiplicationCount);
            List<KeyValuePair<int, int>> lightningPositions = GetRandomBonusPositions(ref emptyPositions, lightningCount);
            //... другие бонусы.

            for (int y = 0; y < width; y++)
            {
                sortNullX = 0;
                for (int x = 0; x < height; x++)
                {
                    if (field[x, y] == null)
                    {
                        pos = new Vector2(y * spriteWidth, (spriteHeight * height) + sortNullX * spriteHeight);

                        // Проверка: нужно ли поставить бонус на это место.
                        if (bombPositions != null && bombPositions.Contains(new KeyValuePair<int, int>(x, y)))
                            field[x, y] = CreateNewFieldItem(x, y, pos, gameManager.bombPrefab, BonusType.Bomb);
                        else if (multiplicationPositions != null && multiplicationPositions.Contains(new KeyValuePair<int, int>(x, y)))
                            field[x, y] = CreateNewFieldItem(x, y, pos, gameManager.multiplicationPrefab, BonusType.Multiplication);
                        else if (lightningPositions != null && lightningPositions.Contains(new KeyValuePair<int, int>(x, y)))
                            field[x, y] = CreateNewFieldItem(x, y, pos, gameManager.lightningPrefab, BonusType.Lightning);
                        else
                            field[x, y] = CreateNewFieldItem(x, y, pos, gameManager.blockPrefab, BonusType.None);
                        sortNullX++;
                    }
                }
            }
        }
        #endregion

        #region Анимации
        /// <summary>
        /// Анимация обсыпания строк.
        /// </summary>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="startAnimationTime">Время начала анимации.</param>
        /// <returns></returns>
        public bool RebuildRows(float speed, float startAnimationTime)
        {
            return RebuildField(speed, startAnimationTime);
        }

        /// <summary>
        /// Анимация сдвига колонок.
        /// </summary>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="startAnimationTime">Время начала анимации.</param>
        /// <returns></returns>
        public bool RebuildCols(float speed, float startAnimationTime)
        {
            return RebuildField(speed, startAnimationTime);
        }

        /// <summary>
        /// Анимация перестроения поля (сдвигает блоки на их позиции).
        /// </summary>
        /// <param name="speed">Скорость анимации.</param>
        /// <param name="startAnimationTime">Время начала анимации.</param>
        /// <returns></returns>
        private bool RebuildField(float speed, float startAnimationTime)
        {
            Vector2 newPos, currentPos;
            float step;
            bool isFinished = true;


            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    if (field[x, y] != null)
                    {
                        newPos = new Vector2(y * spriteWidth, x * spriteHeight);
                        currentPos = new Vector2(field[x, y].transform.position.x, field[x, y].transform.position.y);

                        if (newPos != currentPos)
                        {
                            step = (Time.time - startAnimationTime) * speed / (Vector2.Distance(newPos, currentPos));
                            field[x, y].transform.position = Vector3.Lerp(field[x, y].transform.position, new Vector3(y * spriteWidth, x * spriteHeight, 0f), step);
                            isFinished = false;
                        }
                    }
                }
            }

            return isFinished;
        }
        #endregion

        #region Валидация игрового поля
        /// <summary>
        /// Проверяет, остались ли ходы на поле.
        /// </summary>
        public bool ValidateField()
        {
            List<string> matchList = new List<string>();

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    matchList.Clear();
                    if (field[x, y] != null)
                    {
                        ValidateBlock(ref matchList, field[x, y].GetComponent<BlockComponent>());
                        if (matchList.Count >= gameManager.matchCount)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Рекурсивная проверка каждого блока на существование возможности хода.
        /// </summary>
        private void ValidateBlock(ref List<string> matchList, BlockComponent bc)
        {
            if (matchList.Count >= gameManager.matchCount)
                return;

            // Проверка "cнизу".
            if (bc.x - 1 >= 0 && field[bc.x - 1, bc.y] != null)
                CommitBlockValidation(ref matchList, bc.x - 1, bc.y, bc.type);

            // Проверка "слева".
            if (bc.y - 1 >= 0 && field[bc.x, bc.y - 1] != null)
                CommitBlockValidation(ref matchList, bc.x, bc.y - 1, bc.type);

            // Проверка "сверху".
            if (bc.x + 1 < height && field[bc.x + 1, bc.y] != null)
                CommitBlockValidation(ref matchList, bc.x + 1, bc.y, bc.type);

            // Проверка "справа".
            if (bc.y + 1 < width && field[bc.x, bc.y + 1] != null)
                CommitBlockValidation(ref matchList, bc.x, bc.y + 1, bc.type);

            return;
        }

        /// <summary>
        /// Запускает валидацию рекурсивно дальше.
        /// </summary>
        private void CommitBlockValidation(ref List<string> matchList, int x, int y, BlockType blockType)
        {
            var bc = field[x, y].GetComponent<BlockComponent>();
            if (bc != null && bc.type == blockType)
            {
                string blockName = string.Format("{0};{1}", x, y);
                if (matchList.FirstOrDefault(i => i == blockName) == null)
                {
                    matchList.Add(blockName);
                    ValidateBlock(ref matchList, bc);
                }
            }
        }
        #endregion

        #region Управление полем

        /// <summary>
        /// Включение touch-а блоков игрового поля.
        /// </summary>
        public void EnableBlockTouch()
        {
            EnableCollider2D(true);
        }

        /// <summary>
        /// Выключение touch-а блоков игрового поля.
        /// </summary>
        public void DisableBlockTouch()
        {
            EnableCollider2D(false);
        }

        /// <summary>
        /// Активация/деактивация collider2D блоков.
        /// </summary>
        /// <param name="flag"></param>
        private void EnableCollider2D(bool flag)
        {
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    if (field[x, y] != null)
                        field[x, y].GetComponent<Collider2D>().enabled = flag;
                }
            }
        }

        #endregion

        #region Лог

        public void PrintField()
        {
            Debug.Log("============================");
            string matrix = string.Empty;

            for (int y = width - 1; y >= 0; y--)
            {
                string row = string.Empty;
                for (int x = 0; x < height; x++)
                {
                    row += (field[x, y] == null ? 0 : 1);
                }
                matrix += (row + "\n");
            }
            Debug.Log(matrix);
        }

        #endregion
    }
}
