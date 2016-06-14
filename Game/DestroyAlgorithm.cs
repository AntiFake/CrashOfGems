using UnityEngine;
using System.Collections.Generic;
using Match3.Components;
using System.Linq;

namespace Match3.Main
{
    public class DestroyAlgorithm
    {
        public virtual List<BlockComponent> GetDestroyedElements(GameField field, BlockComponent touchedBlock) { return null; }
    }

    /// <summary>
    /// Алгоритм поиска сопряженных элементов.
    /// </summary>
    public class MatchDestroyAlgorithm : DestroyAlgorithm
    {
        public override List<BlockComponent> GetDestroyedElements(GameField field, BlockComponent touchedBlock)
        {
            List<BlockComponent> destroyList = new List<BlockComponent>() { touchedBlock };
            MatchN(ref destroyList, field, touchedBlock);
            return destroyList;
        }

        /// <summary>
        /// Алгоритм поиска сопряженных блоков одного цвета.
        /// </summary>
        private void MatchN(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
        {
            string spriteName = bc.spriteName;

            // Проверка "снизу".
            if (bc.x - 1 >= 0 && field.Field[bc.x - 1, bc.y] != null)
            {
                CommitMatch(ref destroyList, field, bc.x - 1, bc.y, spriteName);
                //var bc_left = field.Field[bc.x - 1, bc.y].GetComponent<BlockComponent>();
                //if (bc_left != null && bc_left.spriteName == spriteName)
                //{
                //    if (destroyList.FirstOrDefault(i => i.x == bc.x - 1 && i.y == bc.y) == null)
                //    {
                //        destroyList.Add(bc_left);
                //        MatchN(ref destroyList, field, bc_left);
                //    }
                //}
            }

            // Проверка "слева".
            if (bc.y - 1 >= 0 && field.Field[bc.x, bc.y - 1] != null)
            {
                CommitMatch(ref destroyList, field, bc.x, bc.y - 1, spriteName);

                //var bc_top = field.Field[bc.x, bc.y - 1].GetComponent<BlockComponent>();
                //if (bc_top != null && bc_top.spriteName == spriteName)
                //{
                //    if (destroyList.FirstOrDefault(i => i.x == bc.x && i.y == bc.y - 1) == null)
                //    {
                //        destroyList.Add(bc_top);
                //        MatchN(ref destroyList, field, bc_top);
                //    }
                //}
            }

            // Проверка "сверху".
            if (bc.x + 1 < field.FieldHeight && field.Field[bc.x + 1, bc.y] != null)
            {
                CommitMatch(ref destroyList, field, bc.x + 1, bc.y, spriteName);

                //var bc_right = field.Field[bc.x + 1, bc.y].GetComponent<BlockComponent>();
                //if (bc_right != null && bc_right.spriteName == spriteName)
                //{
                //    if (destroyList.FirstOrDefault(i => i.x == bc.x + 1 && i.y == bc.y) == null)
                //    {
                //        destroyList.Add(bc_right);
                //        MatchN(ref destroyList, field, bc_right);
                //    }
                //}
            }

            // Проверка "справа".
            if (bc.y + 1 < field.FieldWidth && field.Field[bc.x, bc.y + 1] != null)
            {
                CommitMatch(ref destroyList, field, bc.x, bc.y + 1, spriteName);

                //var bc_bottom = field.Field[bc.x, bc.y + 1].GetComponent<BlockComponent>();
                //if (bc_bottom != null && bc_bottom.spriteName == spriteName)
                //{
                //    if (destroyList.FirstOrDefault(i => i.x == bc.x && i.y == bc.y + 1) == null)
                //    {
                //        destroyList.Add(bc_bottom);
                //        MatchN(ref destroyList, field, bc_bottom);
                //    }
                //}
            }
        }

        private void CommitMatch(ref List<BlockComponent> destroyList, GameField field, int x, int y, string spriteName)
        {
            var bc = field.Field[x, y].GetComponent<BlockComponent>();
            if (bc != null && bc.spriteName == spriteName)
            {
                if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
                {
                    destroyList.Add(bc);
                    MatchN(ref destroyList, field, bc);
                }
            }
        }
    }

    /// <summary>
    /// Алгоритм уничтожения элементов бомбой.
    /// </summary>
    public class BombDestroyAlgorithm : DestroyAlgorithm
    {
        public override List<BlockComponent> GetDestroyedElements(GameField field, BlockComponent touchedBlock)
        {
            return null;
        }

        /// <summary>
        /// Алгоритм взрыва бомбы.
        /// Взрыв происходит вокруг бомбы по всем направлениям.
        /// </summary>
        private void ExplodeBomb(ref List<BlockComponent> destroyList, GameField field, BlockComponent bc)
        {
            int x, y;

            // Проверка "слева".
            if (bc.x - 1 >= 0 && field.Field[bc.x - 1, bc.y] != null)
            {
                x = bc.x - 1;
                y = bc.y;
                var bc_left = field.Field[x, y].GetComponent<BlockComponent>();

                if (bc_left != null)
                {
                    if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
                    {
                        destroyList.Add(bc_left);
                        // Если рядом также есть бомба --> цепная реакция.
                        if (bc_left.blockType == BlockType.Bomb)
                            ExplodeBomb(ref destroyList, field, bc_left);
                    }
                }
            }

            // Проверка "слева-сверху".
            if (bc.x - 1 >= 0 && bc.y + 1 >= 0 && field.Field[bc.x - 1, bc.y + 1] != null)
            {
                x = bc.x - 1;
                y = bc.y + 1;
                var bc_left_top = field.Field[x, y].GetComponent<BlockComponent>();

                if (bc_left_top != null)
                {
                    if (destroyList.FirstOrDefault(i => i.x == x && i.y == y) == null)
                    {
                        destroyList.Add(bc_left_top);
                        if (bc_left_top.blockType == BlockType.Bomb)
                            ExplodeBomb(ref destroyList, field, bc_left_top);
                    }
                }
            }

            // Проверка "сверху".
            if (bc.y - 1 >= 0 && field.Field[bc.x, bc.y - 1] != null)
            {
                y = bc.y - 1;
                x = bc.x;

                var bc_top = field.Field[x, y].GetComponent<BlockComponent>();
                if (bc_top != null)
                {
                    if (destroyList.FirstOrDefault(i => i.x == bc.x && i.y == bc.y - 1) == null)
                    {
                        destroyList.Add(bc_top);
                        if (bc_top.blockType == BlockType.Bomb)
                            ExplodeBomb(ref destroyList, field, bc_top);
                    }
                }
            }

            // Проверка "сверху-справа".
            if (bc.x + 1 < field.FieldHeight && bc.y - 1 >= 0 && field.Field[bc.x, bc.y - 1] != null)
            {
                y = bc.y - 1;
                x = bc.x;

                var bc_top_right = field.Field[x, y].GetComponent<BlockComponent>();
                if (bc_top_right != null)
                {
                    if (destroyList.FirstOrDefault(i => i.x == bc.x && i.y == bc.y - 1) == null)
                    {
                        destroyList.Add(bc_top_right);
                        if (bc_top_right.blockType == BlockType.Bomb)
                            ExplodeBomb(ref destroyList, field, bc_top_right);
                    }
                }
            }

            // Проверка "справа".

        }
    }
}