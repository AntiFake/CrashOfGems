using UnityEngine;
using System.Collections.Generic;
using Match3.Components;
using System.Linq;

namespace Match3.Main
{
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

        /// <summary>
        /// Уничтожение блоков по выбранному алгоритму.
        /// </summary>
		public static GameField DestroyBlocks(GameField field, BlockComponent bc, int matchCount, AudioSource audioSource, out long points)
		{
            points = 0;
            List<BlockComponent> destroyList = GetDestroyedElements(field, bc);

            if (destroyList.Count >= matchCount)
			{
                // Подсчет очков. Какой-то алгоритм....
                points = destroyList.Count * 10;
                audioSource.Play();
				destroyList.ForEach(i =>
				{
					GameObject.Destroy(i.gameObject);
					field.Field[i.x, i.y] = null;
				});
			}

			return field;
		}

        private static List<BlockComponent> GetDestroyedElements(GameField field, BlockComponent touchedBlock)
        {
            List<BlockComponent> destroyList = new List<BlockComponent>() { touchedBlock };
            MatchN(ref destroyList, field, touchedBlock);
            return destroyList;
        }

        /// <summary>
        /// Алгоритм поиска сопряженных блоков одного цвета.
        /// </summary>
        private static void MatchN(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
        {
            int typeId = bc.typeId;

            // Проверка на бонусы.
            var bonusComponent = bc.GetComponent<BonusComponent>();
            if (bonusComponent != null)
            {
                if (bonusComponent.type == BonusType.Bomb)
                    ExplodeBomb(ref destroyList, field, bc);
                // ... + еще алгоритмы бонусов.
            }

            // Проверка "снизу".
            if (bc.x - 1 >= 0 && field.Field[bc.x - 1, bc.y] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x - 1, bc.y, typeId);

            // Проверка "слева".
            if (bc.y - 1 >= 0 && field.Field[bc.x, bc.y - 1] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x, bc.y - 1, typeId);

            // Проверка "сверху".
            if (bc.x + 1 < field.FieldHeight && field.Field[bc.x + 1, bc.y] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x + 1, bc.y, typeId);

            // Проверка "справа".
            if (bc.y + 1 < field.FieldWidth && field.Field[bc.x, bc.y + 1] != null)
                CommitMatchedBlock(ref destroyList, field, bc.x, bc.y + 1, typeId);
        }

        /// <summary>
        /// Сохранение элемента в destroy-список и рекурсивный запуск функции MatchN.
        /// </summary>
        private static void CommitMatchedBlock(ref List<BlockComponent> destroyList, GameField field, int x, int y, int typeId)
        {
            var bc = field.Field[x, y].GetComponent<BlockComponent>();
            if (bc != null && bc.typeId == typeId)
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
            if (bc.x + 1 < field.FieldHeight && field.Field[bc.x, bc.y] != null)
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
    }
}
