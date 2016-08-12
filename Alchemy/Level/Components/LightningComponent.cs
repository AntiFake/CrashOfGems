using UnityEngine;

namespace Alchemy.Level
{
    public class LightningComponent : MonoBehaviour, IBlock
    {
        public int hitLength = 4;

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
