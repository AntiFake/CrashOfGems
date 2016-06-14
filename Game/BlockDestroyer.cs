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
		public static GameField DestroyBlocks(DestroyAlgorithm da, GameField field, BlockComponent bc, int matchCount, AudioSource audioSource, out long points)
		{
            points = 0;
            List<BlockComponent> destroyList = da.GetDestroyedElements(field, bc);

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
	}
}
