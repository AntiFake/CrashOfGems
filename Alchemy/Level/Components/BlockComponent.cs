using UnityEngine;
using Alchemy.Model;

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
        public ResourceType type;

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
            _animator.SetTrigger("Destroyed");
        }

        public void EndDestroy()
        {
            Destroy(gameObject);
            LevelManager.Instance.SetFieldItemNull(x, y);
        }
    }
}