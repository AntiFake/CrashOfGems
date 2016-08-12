using UnityEngine;

namespace Alchemy.Level
{
    public class BombComponent : MonoBehaviour, IBlock
    {
        public int explosionRadius = 1;

        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void StartDestroy()
        {
            EndDestroy();
        }

        public void EndDestroy()
        {
            Destroy(gameObject);
        }
    }
}
