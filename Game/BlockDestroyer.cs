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
		/// Запуск алгоритмов бонусов. Функция вызывается после GetMatchedElements.
		/// </summary>
		public static void CommitBonuses(ref List<BlockComponent> matchedBlocks, GameField field)
		{
			BlockComponent[] destroyArray = new BlockComponent[matchedBlocks.Count];
			matchedBlocks.CopyTo(destroyArray);
			List<BlockComponent> destroyList = destroyArray.ToList();

			foreach (BlockComponent bc in matchedBlocks)
			{
				// Бомба.
				if (bc.bonusType == BonusType.Bomb)
					ExplodeBomb(ref destroyList, field, bc);
				// Молния.
				if (bc.bonusType == BonusType.Lightning)
					UnleashLightning(ref destroyList, field, bc);
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
		/// Сохранение элементов в destroy-список и запуск ChainReaction.
		/// </summary>
		private static void CommitBombedBlock(ref List<BlockComponent> destroyList, GameField field, int x, int y)
		{
			var bc = field.Field[x, y].GetComponent<BlockComponent>();

			if (bc != null)
			{
				if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
				{
					destroyList.Add(bc);

					if (bc.bonusType != BonusType.None)
						ChainDestruction(ref destroyList, field, bc);
				}
			}
		}

		/// <summary>
		/// Алгоритм срабатывания молнии. Молния бьет во все четыре стороны на указанное число элементов.
		/// </summary>
		private static void UnleashLightning(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
		{
			LightningComponent lightning = bc.gameObject.GetComponent<LightningComponent>();

			int counter = 0, x, y;
			bool isNextNull = false;
			x = bc.x;
			y = bc.y;

			// Cлева.
			while (counter < lightning.hitLength && !isNextNull)
			{
				if (y - 1 >= 0 && field.Field[x, y - 1] != null)
				{
					CommitLightning(ref destroyList, field, x, y - 1);                  
					counter++;
					y--;
				}
				else
					isNextNull = true;
			}

			// Сверху.
			counter = 0; isNextNull = false; x = bc.x; y = bc.y;
			while (counter < lightning.hitLength && !isNextNull)
			{

				if (x + 1 < field.FieldHeight && field.Field[x + 1, y] != null)
				{
					CommitLightning(ref destroyList, field, x + 1, y);
					counter++;
					x++;
				}
				else
					isNextNull = true;
			}

			// Справа.
			counter = 0; isNextNull = false; x = bc.x; y = bc.y;
			while (counter < lightning.hitLength && !isNextNull)
			{

				if (y + 1 < field.FieldWidth && field.Field[x, y + 1] != null)
				{
					CommitLightning(ref destroyList, field, x, y + 1);
					counter++;
					y++;
				}
				else
					isNextNull = true;
			}

			// Вниз.
			counter = 0; isNextNull = false; x = bc.x; y = bc.y;
			while (counter < lightning.hitLength && !isNextNull)
			{

				if (x - 1 >= 0 && field.Field[x - 1, y] != null)
				{
					CommitLightning(ref destroyList, field, x - 1, y);
					counter++;
					x--;
				}
				else
					isNextNull = true;
			}
		}

		/// <summary>
		/// Сохранение элементов в destroy-список и запуск ChainReaction.
		/// </summary>

		private static void CommitLightning(ref List<BlockComponent> destroyList, GameField field, int x, int y)
		{
			BlockComponent bc = field.Field[x, y].GetComponent<BlockComponent>();

			if (bc != null)
			{
				if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
				{
					destroyList.Add(bc);

					//Цепная реакция.
					if (bc.bonusType != BonusType.None)
						ChainDestruction(ref destroyList, field, bc);
				}
			}
		}

		/// <summary>
		/// Цепная реакция при уничтожении блоков с помощью бонуса.
		/// </summary>
		private static void ChainDestruction(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
		{
			// Молния.
			if (bc.bonusType == BonusType.Lightning)
				UnleashLightning(ref destroyList, field, bc);
			// Бомба.
			if (bc.bonusType == BonusType.Bomb)
				ExplodeBomb(ref destroyList, field, bc);
			// ... + бонус.
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
			var multipliers = destroyedList.Where(i => i.bonusType == BonusType.Multiplication);

			if (!multipliers.Any())
				return 1;

			int total = 1;

			foreach (var m in multipliers)
			{
				total *= m.GetComponent<MultiplierComponent>().multiplierValue;
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