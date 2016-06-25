using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using CrashOfGems.Enums;

namespace CrashOfGems.UIComponents
{
    /// <summary>
    /// UI-компонент "мензурка".
    /// </summary>
    public class UIVesselComponent : MonoBehaviour
    {
        public Image vessel;
        public float fillRate;
        public BlockType blockType;

        private void Awake()
        {
            vessel.fillAmount = 0f;
        }

        private float CalculateNewAmount(int amount)
        {
            float currentAmount = vessel.fillAmount;
            float normalized = (float) amount / 1000;
            currentAmount += normalized;
            currentAmount = currentAmount > 1.0f ? 1.0f : currentAmount;

            return currentAmount;
        }

        public void PourIn(int amount)
        {
            StopAllCoroutines();
            StartCoroutine(FillVessel(CalculateNewAmount(amount)));
        }

        public void PourOut(int amount)
        {
            StopAllCoroutines();
            StartCoroutine(EmptyVessel(CalculateNewAmount(amount)));
        }

        public void Empty()
        {
            StopAllCoroutines();
            StartCoroutine(EmptyVessel(0.0f));
        }

        private IEnumerator FillVessel(float amount)
        {
            float start = Time.time;

            while (vessel.fillAmount < amount)
            {
                vessel.fillAmount += ((Time.time - start) * fillRate);
                yield return null;
            }
        }

        private IEnumerator EmptyVessel(float amount)
        {
            float start = Time.time;

            while (vessel.fillAmount > amount)
            {
                vessel.fillAmount -= ((Time.time - start) * fillRate);
                yield return null;
            }
        }
    }
}