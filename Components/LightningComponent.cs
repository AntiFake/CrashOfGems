using UnityEngine;
using System.Collections;

namespace CrashOfGems.Components
{
    public class LightningComponent : MonoBehaviour, IBlock
    {
        public int hitLength = 4;

        private void Awake()
        {

        }

        public void Destroy()
        {
            //Debug.Log("Lightning destroy animation...");
        }
    }
}
