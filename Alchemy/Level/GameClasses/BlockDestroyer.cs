using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Alchemy.Level
{
	/// <summary>
	/// Статический класс, реализующий алгоритмы уничтожения блоков на поле и последовательность их вызова.
	/// </summary>
	public static class BlockDestroyer
	{
		/// <summary>
		/// Уничтожение игрового поля.
		/// </summary>
		public static GameField DestroyField(GameField field)
		{
			for (int x = 0; x < field.FieldHeight; x++)
			{
				for (int y = 0; y < field.FieldWidth; y++)
				{
					GameObject.Destroy(field.Field[x, y]);
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