using UnityEngine;
using CrashOfGems.Enums;

namespace Alchemy.Level
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

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnMouseDown()
        {
            LevelManager.Instance.TouchBlockCallback(this);
        }

        public void StartDestroy()
        {
            switch (bonusType)
            {
                case BonusType.None:
                    _animator.SetTrigger("Destroyed");
                    break;
                case BonusType.Bomb:
                    gameObject.GetComponent<BombComponent>().StartDestroy();
                    break;
                case BonusType.Lightning:
                    gameObject.GetComponent<LightningComponent>().StartDestroy();
                    break;
                case BonusType.Multiplication:
                    gameObject.GetComponent<MultiplierComponent>().StartDestroy();
                    break;
            }
        }

        public void EndDestroy()
        {
            Destroy(gameObject);
            LevelManager.Instance.SetFieldItemNull(x, y);
        }
    }
}