using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Alchemy
{
    public class UIRecipeListComponent : MonoBehaviour
    {
        public GameObject recipeInstance;
        public GameObject scrollContent;

        private void OnEnable()
        {
            VisualizeList();
        }

        private void OnDisable()
        {
            DevisualizeList();
        }

        /// <summary>
        /// Отображение списка рецептов.
        /// </summary>
        private void VisualizeList()
        {
            foreach (var recipe in GameManager.gameManager.RecipeList)
            {
                var listItem = Instantiate(recipeInstance);
                listItem.transform.SetParent(scrollContent.transform, false);

                var recipeComponent = listItem.GetComponent<UIRecipeComponent>();
                recipeComponent.Model = GameManager.gameManager.GetRecipeViewModel(recipe.potionType);
                recipeComponent.Visualize();
            }
        }

        private void DevisualizeList()
        {
            for (int i = 0; i < scrollContent.transform.childCount; i++)
            {
                var recipeItem = scrollContent.transform.GetChild(i).gameObject;
                // Удаляем только те рецепты, которые isActive = true. (isActive = false - базовый элемент).
                if (recipeItem.activeSelf)
                    Destroy(recipeItem);
            }
        }
    }
}
