using UnityEngine;
using CrashOfGems.Management;
using CrashOfGems.Enums;

namespace CrashOfGems.Components
{
    /// <summary>
    /// Компонент для каждого из блоков игрового поля.
    /// Позволяет получать какой элемент был выбран пользователем.
    /// </summary>
    public class BlockComponent : MonoBehaviour, IBlock
    {
        public int x;
        public int y;
        public BlockType type;
        public BonusType bonusType;

        public void OnMouseDown()
        {
            GameManager.Instance.TouchBlockCallback(this);
        }

        public void Destroy()
        {

        }
    }
}