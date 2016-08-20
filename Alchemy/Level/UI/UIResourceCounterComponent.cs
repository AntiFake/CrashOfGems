using UnityEngine;
using UnityEngine.UI;
using Alchemy.Model;

namespace Alchemy.Level
{
    public class UIResourceCounterComponent : MonoBehaviour
    {
        public Image fillImage;
        public Image resourceIcon;
        public Text remainCount;

        public ResourceType resourceType { get; set; }
        public int ingredientCost { get; set; }

        private int currentValue;

        public void Visualize(Sprite icon, ResourceType type, int cost)
        {
            resourceIcon.sprite = icon;
            resourceType = type;
            ingredientCost = cost;
            currentValue = 0;

            remainCount.text = string.Format("{0}/{1}", currentValue, ingredientCost);

            fillImage.fillAmount = (float) currentValue / ingredientCost;
        }

        public void UpdateCounter(int destroyedCount)
        {
            if (destroyedCount > ingredientCost)
            {
                while (destroyedCount > ingredientCost)
                {
                    destroyedCount -= ingredientCost;
                }
                currentValue = destroyedCount;
            }
            else
                currentValue = Mathf.Abs(currentValue + destroyedCount);

            if (currentValue >= ingredientCost)
                currentValue -= ingredientCost;

            fillImage.fillAmount =  (float) currentValue / ingredientCost;
            remainCount.text= string.Format("{0}/{1}", currentValue, ingredientCost);
        }

        private void PlayAddAnimation()
        {

        }
    }
}
