using UnityEngine;
using UnityEngine.UI;

namespace Alchemy
{
    public class UILevelChoiceItemComponent : MonoBehaviour
    {
        public bool isSelected;
        public Image icon;
        public Color activeColor;
        public Color nonActiveColor;

        public GameObject levelPreviewPanel;
        public GameObject levelChoicePanel;

        public DifficultyType DifficultyType { get; set; }
        public LevelType LevelType { get; set; }

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => ButtonOnClickEvent());
        }

        private void ButtonOnClickEvent()
        {
            var thisLvlItem = GetComponent<UILevelChoiceItemComponent>();

            // Если уже выделен данный уровень.
            if (thisLvlItem.isSelected)
                return;

            // Сбрасываем все уровни и устанавливаем новый.
            for (int i = 0; i < levelChoicePanel.transform.childCount; i++)
            {
                var lvlItem = levelChoicePanel.transform.GetChild(i);

                if (!lvlItem.gameObject.activeSelf)
                    continue;

                var comp = lvlItem.GetComponent<UILevelChoiceItemComponent>();
                comp.isSelected = false;
                comp.icon.color = nonActiveColor;
            }
            
            thisLvlItem.isSelected = true;
            thisLvlItem.icon.color = activeColor;

            // Загрузка зелий, preview-картинки и названия уровня.
            levelPreviewPanel.GetComponent<UILevelPreviewComponent>().UpdateVisualization(LevelType, DifficultyType);
        }

        public void Visualize(Sprite icon, DifficultyType dflcType, LevelType lvlType)
        {
            this.icon.sprite = icon;
            DifficultyType = dflcType;
            LevelType = lvlType;

            if (dflcType == DifficultyType.I)
            {
                isSelected = true;
                this.icon.color = activeColor;
            }
            else
            {
                isSelected = false;
                this.icon.color = nonActiveColor;
            }

            gameObject.SetActive(true);
        }
    }
}
