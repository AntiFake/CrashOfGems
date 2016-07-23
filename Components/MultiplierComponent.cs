using UnityEngine;
using System.Collections;

namespace CrashOfGems.Components
{
    public class MultiplierComponent : MonoBehaviour, IBlock
    {
        public int multiplierValue = 2;

        private void Awake()
        {

        }

        public void Destroy()
        {
            //Debug.Log("Multiplier destroy animation...");
        }
    }
}
