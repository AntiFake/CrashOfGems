using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Model;

namespace Alchemy.Level
{
    public class GameField
    {
        // Игровое поле.
        private GameObject[,] _field;
        private GameObject[,] _substrate;

        private int _width;
        private int _height;

        // Спрайты.
        private float _spriteWidth;
        private float _spriteHeight;

        // Анимации.
        private bool _isRebuildRowsOn;
        private bool _isRebuildColsOn;

        // Переменные
        private System.Random _rnd;

        #region Свойства
        public bool IsRebuildRowsOn { get { return _isRebuildRowsOn; } set { _isRebuildRowsOn = value; } }
        public bool IsRebuildColsOn { get { return _isRebuildColsOn; } set { _isRebuildColsOn = value; } }
        public int FieldWidth { get { return _width; } }
        public GameObject[,] Field { get { return _field; } set { _field = value; } }
        public int FieldHeight { get { return _height; } }
        public bool IsFieldFull
        {
            get
            {
                int notNullCount = 0;
                for (int x = 0; x < _height; x++)
                {
                    for (int y = 0; y < _width; y++)
                    {
                        if (_field[x, y] != null)
                            notNullCount++;
                    }
                }

                return notNullCount == _width * _height;
            }
        }
        public bool IsFieldEmpty
        {
            get
            {
                for (int x = 0; x < _height; x++)
                {
                    for (int y = 0; y < _width; y++)
                    {
                        if (_field[x, y] != null)
                            return false;
                    }
                }

                return true;
            }
        }
        #endregion

        public GameField(int width, int height, bool createSubstrate, GameObject substrateParent, GameObject fieldParent)
        {
            _width = width;
            _height = height;

            _spriteWidth = GameManager.Instance.LevelModel.resources[0].sprite.bounds.size.x;
            _spriteHeight = GameManager.Instance.LevelModel.resources[0].sprite.bounds.size.y;

            if (createSubstrate)
                _substrate = BlockFactory.Instance.CreateFieldSubstrate(width, height, _spriteWidth, _spriteHeight, substrateParent);

            _field = BlockFactory.Instance.GenerateField(width, height, _spriteWidth, _spriteHeight, fieldParent);
            _rnd = new System.Random();
        }

        #region Перестроение
        /// <summary>
        /// Пересобрать строки. Все элементы перемещаются вниз.
        /// </summary>
        public void UpdateRows()
        {
            // Новая позиция по x.
            GameObject[,] gf = new GameObject[_height, _width];
            int newX;
            BlockComponent bc;

            for (int y = 0; y < _width; y++)
            {
                newX = 0;
                for (int x = 0; x < _height; x++)
                {
                    // Если блок не удален.
                    if (_field[x, y] != null)
                    {
                        gf[newX, y] = _field[x, y];
                        bc = gf[newX, y].GetComponent<BlockComponent>();
                        bc.x = newX;
                        gf[newX, y].name = string.Format("{0};{1}", newX, y);
                        newX++;
                    }
                }
            }
            _field = gf;
        }

        /// <summary>
        /// Пересобрать колонки. Реализован сдвиг влево.
        /// </summary>
        public void UpdateCols()
        {
            GameObject[,] gf = new GameObject[_height, _width];
            BlockComponent bc;
            int newY = 0;

            for (int y = 0; y < _width; y++)
            {
                if (_field[0, y] != null)
                {
                    for (int x = 0; x < _height; x++)
                    {
                        gf[x, newY] = _field[x, y];
                        if (_field[x, y] != null)
                        {
                            bc = gf[x, newY].GetComponent<BlockComponent>();
                            bc.y = newY;
                            gf[x, newY].name = string.Format("{0};{1}", x, newY);
                        }
                    }
                    newY++;
                }
            }
            
            _field = gf;
            _isRebuildColsOn = true;
        }

        /// <summary>
        /// Получение пустых позиций на ИП.
        /// </summary>
        private List<KeyValuePair<int, int>> GetEmptyPositions()
        {
            List<KeyValuePair<int, int>> emptyPositions = new List<KeyValuePair<int, int>>();

            for (int y = 0; y < _width; y++)
            {
                for (int x = 0; x < _height; x++)
                {
                    if (_field[x, y] == null)
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
                pos = _rnd.Next(0, emptyPositions.Count);
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
        public void GenerateNewBlocks(GameObject fieldParent)
        {
            int sortNullX;
            Vector2 pos;
            List<KeyValuePair<int, int>> emptyPositions = GetEmptyPositions();

            for (int y = 0; y < _width; y++)
            {
                sortNullX = 0;
                for (int x = 0; x < _height; x++)
                {
                    if (_field[x, y] == null)
                    {
                        pos = new Vector2(y * _spriteWidth, (_spriteHeight * _height) + sortNullX * _spriteHeight);
                        _field[x, y] = BlockFactory.Instance.CreateBlock(x, y, pos, fieldParent);
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
        public bool RebuildRows(float speed, float startAnimationTime)
        {
            return RebuildField(speed, startAnimationTime);
        }

        /// <summary>
        /// Анимация сдвига колонок.
        /// </summary>
        public bool RebuildCols(float speed, float startAnimationTime)
        {
            return RebuildField(speed, startAnimationTime);
        }

        /// <summary>
        /// Анимация перестроения поля (сдвигает блоки на их позиции).
        /// </summary>
        private bool RebuildField(float speed, float startAnimationTime)
        {
            Vector2 newPos, currentPos;
            float step;
            bool isFinished = true;


            for (int y = 0; y < _width; y++)
            {
                for (int x = 0; x < _height; x++)
                {
                    if (_field[x, y] != null)
                    {
                        newPos = new Vector2(y * _spriteWidth, x * _spriteHeight);
                        currentPos = new Vector2(_field[x, y].transform.position.x, _field[x, y].transform.position.y);

                        if (newPos != currentPos)
                        {
                            step = (Time.time - startAnimationTime) * speed / (Vector2.Distance(newPos, currentPos));
                            _field[x, y].transform.position = Vector3.Lerp(_field[x, y].transform.position, new Vector3(y * _spriteWidth, x * _spriteHeight, 0f), step);
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

            for (int x = 0; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {
                    matchList.Clear();
                    if (_field[x, y] != null)
                    {
                        ValidateBlock(ref matchList, _field[x, y].GetComponent<BlockComponent>());
                        if (matchList.Count >= LevelManager.Instance.matchCount)
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
            if (matchList.Count >= LevelManager.Instance.matchCount)
                return;

            // Проверка "cнизу".
            if (bc.x - 1 >= 0 && _field[bc.x - 1, bc.y] != null)
                CommitBlockValidation(ref matchList, bc.x - 1, bc.y, bc.type);

            // Проверка "слева".
            if (bc.y - 1 >= 0 && _field[bc.x, bc.y - 1] != null)
                CommitBlockValidation(ref matchList, bc.x, bc.y - 1, bc.type);

            // Проверка "сверху".
            if (bc.x + 1 < _height && _field[bc.x + 1, bc.y] != null)
                CommitBlockValidation(ref matchList, bc.x + 1, bc.y, bc.type);

            // Проверка "справа".
            if (bc.y + 1 < _width && _field[bc.x, bc.y + 1] != null)
                CommitBlockValidation(ref matchList, bc.x, bc.y + 1, bc.type);

            return;
        }

        /// <summary>
        /// Запускает валидацию рекурсивно дальше.
        /// </summary>
        private void CommitBlockValidation(ref List<string> matchList, int x, int y, ResourceType blockType)
        {
            var bc = _field[x, y].GetComponent<BlockComponent>();
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
            for (int x = 0; x < _height; x++)
            {
                for (int y = 0; y < _width; y++)
                {
                    if (_field[x, y] != null)
                        _field[x, y].GetComponent<Collider2D>().enabled = flag;
                }
            }
        }

        #endregion
    }
}
