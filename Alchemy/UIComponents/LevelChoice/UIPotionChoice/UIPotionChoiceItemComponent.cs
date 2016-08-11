using UnityEngine;
using UnityEngine.UI;

namespace Alchemy
{
    public class UIPotionChoiceItemComponent : MonoBehaviour
    {
        public bool isSelected = false;
        public Image icon;
        public Color activeColor;
        public Color nonactiveColor;

        private Button button;

        public PotionType PotionType { get; set; }

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => ButtonOnClickEvent());
        }

        public void Visualize(Sprite icon, PotionType type)
        {
            this.icon.sprite = icon;
            this.icon.color = nonactiveColor;
            PotionType = type;
            gameObject.SetActive(true);
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
