using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Alchemy.Model;

namespace Alchemy.Level
{
    public class UIResourcesComponent : MonoBehaviour
    {
        public GameObject ingredientPrefab;

        public void Visualize(List<IngredientCostLevelModel> ingredientCosts)
        {
            foreach (var item in ingredientCosts)
            {
                var uiItem = Instantiate(ingredientPrefab);
                uiItem.transform.SetParent(transform, false);

                var comp = uiItem.GetComponent<UIResourceCounterComponent>();
                comp.Visualize(item.resourceSprite, item.resourceType, item.conversionCost);

                uiItem.gameObject.SetActive(true);
            }
        }

        public void UpdateCounters(ResourceType rt, int destroyedCount)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var comp = transform.GetChild(i).GetComponent<UIResourceCounterComponent>();
                if (comp.resourceType == rt)
                    comp.UpdateCounter(destroyedCount);
            }
        }
    }
}
