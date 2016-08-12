using UnityEngine;
using System.Collections.Generic;
using Alchemy.Model;

namespace Alchemy.UI
{
    public class UIPotionChoiceComponent : MonoBehaviour
    {
        public GameObject potionChoiceItemInstance;

        public void Visualize(List<Potion> potionList)
        {
            foreach (var item in potionList)
            {
                var potionBonus = Instantiate(potionChoiceItemInstance);
                var comp = potionBonus.GetComponent<UIPotionChoiceItemComponent>();
                comp.Visualize(item.sprite, item.type);
                potionBonus.transform.SetParent(gameObject.transform, false);
            }
        }

        public void Devisualize()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var childItem = gameObject.transform.GetChild(i);

                if (!childItem.gameObject.activeSelf)
                    continue;

                Destroy(childItem.gameObject);
            }
        }

        /// <summary>
        /// Получает выбранные пользователем бонусы.
        /// </summary>
        /// <returns></returns>
        public List<PotionType> GetSelectedBonusPotions()
        {
            List<PotionType> bonusPotions = new List<PotionType>();

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var childItem = gameObject.transform.GetChild(i);

                if (!childItem.gameObject.activeSelf)
                    continue;

                var comp = childItem.GetComponent<UIPotionChoiceItemComponent>();

                if (comp.isSelected)
                    bonusPotions.Add(comp.PotionType);
            }

            return bonusPotions;
        }
    }
}
