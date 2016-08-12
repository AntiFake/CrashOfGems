using UnityEngine;

namespace Alchemy.Level
{
    public class MultiplierComponent : MonoBehaviour, IBlock
    {
        public int multiplierValue = 2;

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
