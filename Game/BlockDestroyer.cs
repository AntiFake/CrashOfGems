using UnityEngine;
using System.Collections.Generic;
using CrashOfGems.Components;
using System.Linq;
using CrashOfGems.Enums;

namespace CrashOfGems.Game
{
    /// <summary>
    /// Статический класс, реализующий алгоритмы уничтожения блоков на поле и последовательность их вызова.
    /// </summary>
	public static class BlockDestroyer
	{
        /// <summary>
        /// Уничтожение игрового поля.
        /// </summary>
        /// <param name="field">Игровое поле.</param>
        /// <returns></returns>
		public static GameField DestroyField(GameField field)
		{
			for (int x = 0; x < field.FieldHeight; x++)
			{
				for (int y = 0; y < field.FieldWidth; y++)
				{
					GameObject.Destroy(field.Field[x,y]);
					field.Field[x, y] = null;
				}
			}
			return field;
		}

        public static List<BlockComponent> GetMatchedElements(GameField field, BlockComponent touchedBlock)
        {
            List<BlockComponent> destroyList = new List<BlockComponent>() { touchedBlock };
            MatchN(ref destroyList, field, touchedBlock);
            return destroyList;
        }

        #region Алгоритмы уничтожения

        /// <summary>
        /// Запуск алгоритмов бонусов. Функция вызывается после GetMatchedElements.
        /// </summary>
        public static void CommitBonuses(ref List<BlockComponent> matchedBlocks, GameField field)
        {
            BonusComponent bonus;
            BlockComponent[] destroyArray = new BlockComponent[matchedBlocks.Count];
            matchedBlocks.CopyTo(destroyArray);
            List<BlockComponent> destroyList = destroyArray.ToList();

            foreach (BlockComponent bc in matchedBlocks)
            {
                bonus = bc.GetComponent<BonusComponent>();
                if (bonus != null)
                {
                    if (bonus.type == BonusType.Bomb)
                        ExplodeBomb(ref destroyList, field, bc);
                }
            }

            matchedBlocks = destroyList;
        }

        /// <summary>
        /// Алгоритм поиска сопряженных блоков одного цвета.
        /// </summary>
        private static void MatchN(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
        {
            BlockType blockType = bc.type;

            // Проверка "снизу".
            if (bc.x - 1 >= 0 && field.Field[bc.x - 1, bc.y] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x - 1, bc.y, blockType);

            // Проверка "слева".
            if (bc.y - 1 >= 0 && field.Field[bc.x, bc.y - 1] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x, bc.y - 1, blockType);

            // Проверка "сверху".
            if (bc.x + 1 < field.FieldHeight && field.Field[bc.x + 1, bc.y] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x + 1, bc.y, blockType);

            // Проверка "справа".
            if (bc.y + 1 < field.FieldWidth && field.Field[bc.x, bc.y + 1] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x, bc.y + 1, blockType);
        }

        /// <summary>
        /// Сохранение элемента в destroy-список и рекурсивный запуск функции MatchN.
        /// </summary>
        private static void CommitMatchedBlock(ref List<BlockComponent> destroyList, GameField field, int x, int y, BlockType blockType)
        {
            var bc = field.Field[x, y].GetComponent<BlockComponent>();
            if (bc != null && bc.type == blockType)
            {
                if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
                {
                    destroyList.Add(bc);
                    MatchN(ref destroyList, field, bc);
                }
            }
        }

        /// <summary>
        /// Алгоритм взрыва бомбы.
        /// Взрыв происходит вокруг бомбы по всем направлениям.
        /// </summary>
        private static void ExplodeBomb(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
        {
            // Проверка "снизу".
            if (bc.x - 1 >= 0 && field.Field[bc.x - 1, bc.y] != null)
                CommitBombedBlock(ref destroyList, field, bc.x - 1, bc.y);

            // Проверка "снизу-слева".
            if (bc.x - 1 >= 0 && bc.y - 1 >= 0 && field.Field[bc.x - 1, bc.y - 1] != null)
                CommitBombedBlock(ref destroyList, field, bc.x - 1, bc.y - 1);

            // Проверка "слева".
            if (bc.y - 1 >= 0 && field.Field[bc.x, bc.y - 1] != null)
                CommitBombedBlock(ref destroyList, field, bc.x, bc.y - 1);

            // Проверка "слева-сверху".
            if (bc.y - 1 >= 0 && bc.x + 1 < field.FieldHeight && field.Field[bc.x + 1, bc.y - 1] != null)
                CommitBombedBlock(ref destroyList, field, bc.x + 1, bc.y - 1);

            // Проверка "сверху".
            if (bc.x + 1 < field.FieldHeight && field.Field[bc.x + 1, bc.y] != null)
                CommitBombedBlock(ref destroyList, field, bc.x + 1, bc.y);

            // Проверка "сверху-справа".
            if (bc.x + 1 < field.FieldHeight && bc.y + 1 < field.FieldWidth && field.Field[bc.x + 1, bc.y + 1] != null)
                CommitBombedBlock(ref destroyList, field, bc.x + 1, bc.y + 1);

            // Проверка "справа".
            if (bc.y + 1 < field.FieldWidth && field.Field[bc.x, bc.y + 1] != null)
                CommitBombedBlock(ref destroyList, field, bc.x, bc.y + 1);

            // Проверка "справа-снизу".
            if (bc.x - 1 >= 0 && bc.y + 1 < field.FieldWidth && field.Field[bc.x - 1, bc.y + 1] != null)
                CommitBombedBlock(ref destroyList, field, bc.x - 1, bc.y + 1);
        }

        /// <summary>
        /// Сохранение элементов в destroy-список.
        /// Если элемент - бомба, то запускается "цепная реакция" в виде рекурсии.
        /// </summary>
        private static void CommitBombedBlock(ref List<BlockComponent> destroyList, GameField field, int x, int y)
        {
            var bc = field.Field[x, y].GetComponent<BlockComponent>();

            if (bc != null)
            {
                if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
                {
                    destroyList.Add(bc);
                    // Если рядом также есть бомба --> цепная реакция.
                    if (bc.bonus != null && bc.bonus.type == BonusType.Bomb)
                        ExplodeBomb(ref destroyList, field, bc);
                }
            }
        }
        #endregion

        #region Подсчет очков
        public static Dictionary<BlockType, long> CalculatePoints(List<BlockComponent> destroyList, int matchCount, int startBlockCost, int costDelta)
        {
            Dictionary<BlockType, int> destroyedBlocks = (
                from b in destroyList
                group b by b.GetComponent<BlockComponent>().type into grp
                select new { blockType = grp.Key, count = grp.Count() }
            ).ToDictionary(i => i.blockType, i => i.count);

            Dictionary<BlockType, long> points = new Dictionary<BlockType, long>();
            var keys = destroyedBlocks.Keys;
            
            // Подсчет очков для каждой категории.
            foreach (var key in keys)
            {
                points.Add(key, CalculateBlocksPoints(destroyedBlocks[key], matchCount, startBlockCost, costDelta));
            }

            return points;
        }

        /// <summary>
        /// Функция для получения общего множителя "умножающих" бонусов.
        /// </summary>
        public static int CalculateBonusMultiplier(List<BlockComponent> destroyedList)
        {
            var multipliers = destroyedList.Where(i => i.gameObject.GetComponent<BonusComponent>() != null && i.gameObject.GetComponent<BonusComponent>().type == BonusType.Multiplication);
            if (!multipliers.Any())
                return 1;

            int total = 1;

            foreach (var m in multipliers)
            {
                total *= m.GetComponent<BonusComponent>().value;
            }

            return total;
        }

        /// <summary>
        /// Расчет очков для блоков опред. группы.
        /// </summary>
        private static long CalculateBlocksPoints(int blockCount, int matchCount, int startBlockCost, int costDelta)
        {
            long points = startBlockCost * matchCount;
            if (blockCount > matchCount)
            {
                int sbc = startBlockCost, cd = costDelta;
                for (int i = matchCount; i < blockCount; i++)
                {
                    sbc += cd;
                    points += sbc;
                }
            }

            return points;
        }
        #endregion
    }
}
