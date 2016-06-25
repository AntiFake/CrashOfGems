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
        public float fillSpeed = 0.3f;
        public BlockType blockType;

        /// <summary>
        /// Свойство, рассчитывающее число полученных бонусов.
        /// </summary>
        public int BonusCount
        {
            get
            {
                if (vessel.fillAmount == 1.0f)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Свойство заполненности "мензурки".
        /// </summary>
        public bool IsFullFilled
        {
            get
            {
                return vessel.fillAmount == 1.0f;
            }
        }

        private void Awake()
        {
            vessel.fillAmount = 0f;
        }

        private float CalculateNewAmount(long amount)
        {
            float currentAmount = vessel.fillAmount;
            float normalized = (float) amount / 1000;
            currentAmount += normalized;
            currentAmount = currentAmount > 1.0f ? 1.0f : currentAmount;

            return currentAmount;
        }

        public void PourIn(long amount)
        {
            StopAllCoroutines();
            StartCoroutine(FillVessel(CalculateNewAmount(amount)));
        }

        public void PourOut(long amount)
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
                vessel.fillAmount += ((Time.time - start) * fillSpeed);
                yield return null;
            }
        }

        private IEnumerator EmptyVessel(float amount)
        {
            float start = Time.time;

            while (vessel.fillAmount > amount)
            {
                vessel.fillAmount -= ((Time.time - start) * fillSpeed);
                yield return null;
            }
        }
    }
}