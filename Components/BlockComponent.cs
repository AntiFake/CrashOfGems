using UnityEngine;
using System.Collections;

namespace Match3.Components
{
    /// <summary>
    /// Компонент для каждого из блоков игрового поля.
    /// Позволяет получать какой элемент был выбран пользователем.
    /// </summary>
    public class BlockComponent : MonoBehaviour
    {
        public int x;
        public int y;
        public int typeId;
        public GameManager gameManager;
        public BonusComponent bonus;

        public void OnMouseDown()
        {
            gameManager.TouchBlockCallback(this);
        }
    }
}