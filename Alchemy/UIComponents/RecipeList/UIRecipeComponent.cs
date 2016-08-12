using UnityEngine;
using UnityEngine.UI;
using Alchemy.Model;

namespace Alchemy.UI
{
    public class UIRecipeComponent : MonoBehaviour
    {
        public Image potionIcon;
        public Text potionTitle;
        public Button btnCook;
        public Button btnInfo;
        public GameObject ingredientInstance;
        public GameObject ingredientPanel;
        public GameObject helpWindowInstance;

        public RecipeViewModel Model { get; set; }

        private void Start()
        {
            btnCook.onClick.AddListener(() => BtnCookOnClickEvent());
            btnInfo.onClick.AddListener(() => BtnInfoOnClickEvent());
        }

        public void Visualize()
        {
            potionIcon.sprite = Model.potionSprite;
            potionTitle.text = Model.potionName;

            // Требуемые для приготовления ингредиенты.
            foreach (var ri in Model.requiredIngredients)
            {
                var ingredient = (GameObject)Instantiate(ingredientInstance);
                ingredient.transform.SetParent(ingredientPanel.transform, false);

                var ingredientComponent = ingredient.GetComponent<UIRecipeIngredientComponent>();
                ingredientComponent.Visualize(ri.sprite, ri.requiredCount.ToString());
            }

            // Требуемые для приготовления ызелья.
            foreach (var rp in Model.requiredPotions)
            {
                var ingredient = (GameObject)Instantiate(ingredientInstance);
                ingredient.transform.SetParent(ingredientPanel.transform, false);
                ingredient.SetActive(true);

                var ingredientComponent = ingredient.GetComponent<UIRecipeIngredientComponent>();
                ingredientComponent.Visualize(rp.sprite, rp.requiredCount.ToString());
            }

            gameObject.SetActive(true);
        }

        public void BtnInfoOnClickEvent()
        {
            helpWindowInstance.GetComponent<UIRecipeInfoComponent>().Visualize(Model.potionName, Model.potionDescription, Model.potionSprite);
        }

        public void BtnCookOnClickEvent()
        {
            if (GameManager.Instance.CheckCooking(Model.potionType))
                GameManager.Instance.Cook(Model.potionType);
            else
                Debug.Log("Не могу приготовить!");
        }
    }
}
