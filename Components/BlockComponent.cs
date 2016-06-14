using UnityEngine;
using System.Collections;

namespace Match3.Components
{
    public enum BlockType { Block, Bomb }

    /// <summary>
    /// Компонент для каждого из блоков игрового поля.
    /// Позволяет получать какой элемент был выбран пользователем.
    /// </summary>
    public class BlockComponent : MonoBehaviour
    {
        public int x;
        public int y;
        public string spriteName;
        public BlockType blockType;
        public GameManager gameManager;

        public void OnMouseDown()
        {
            gameManager.TouchBlockCallback(this);
        }
    }
}