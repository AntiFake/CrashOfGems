using UnityEngine;
using CrashOfGems.Management;
using CrashOfGems.Enums;

namespace CrashOfGems.Components
{
    /// <summary>
    /// Компонент для каждого из блоков игрового поля.
    /// Позволяет получать какой элемент был выбран пользователем.
    /// </summary>
    public class BlockComponent : MonoBehaviour
    {
        public int x;
        public int y;
        public BlockType type;
        public GameManager gameManager;
        public BonusComponent bonus;

        public void OnMouseDown()
        {
            gameManager.TouchBlockCallback(this);
        }
    }
}