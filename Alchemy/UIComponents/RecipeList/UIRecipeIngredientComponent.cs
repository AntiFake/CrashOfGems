using UnityEngine;
using UnityEngine.UI;

namespace Alchemy.UI
{
    public class UIRecipeIngredientComponent : MonoBehaviour
    {
        public Image icon;
        public Text count;

        /// <summary>
        /// Отображение информации об ингредиенте.
        /// </summary>
        public void Visualize(Sprite sprite, string count)
        {
            icon.sprite = sprite;
            this.count.text = count;
            gameObject.SetActive(true);
        }
    }
}
