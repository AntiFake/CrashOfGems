using UnityEngine;
using UnityEngine.UI;
using Alchemy.Model;

namespace Alchemy.UI
{
    public class UIPotionChoiceItemComponent : MonoBehaviour
    {
        public bool isSelected = false;
        public Image icon;
        public Color activeColor;
        public Color nonactiveColor;

        private Button button;

        public PotionType PotionType { get; set; }

        public void Visualize(Sprite icon, PotionType type, bool enabled)
        {
            this.icon.sprite = icon;
            this.icon.color = nonactiveColor;
            PotionType = type;
            gameObject.SetActive(true);

            // Если данное зелье есть у игрока, то он модет взять его на уровень, иначе иконка не кликабельна.
            if (enabled)
            {
                button = GetComponent<Button>();
                button.onClick.AddListener(() => ButtonOnClickEvent());
            }
        }

        private void ButtonOnClickEvent()
        {
            if (isSelected)
            {
                isSelected = false;
                icon.color = nonactiveColor;
            }
            else
            {
                isSelected = true;
                icon.color = activeColor;
            }
        }
    }
}
