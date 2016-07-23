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
            switch (bonusType)
            {
                case BonusType.None:
                    //Debug.Log("Block destroy animation...");
                    break;
                case BonusType.Bomb:
                    gameObject.GetComponent<BombComponent>().Destroy();
                    break;
                case BonusType.Lightning:
                    gameObject.GetComponent<LightningComponent>().Destroy();
                    break;
                case BonusType.Multiplication:
                    gameObject.GetComponent<MultiplierComponent>().Destroy();
                    break;
            }
            Destroy(gameObject);
        }
    }
}