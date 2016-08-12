using UnityEngine;
using UnityEngine.UI;

namespace Alchemy.UI
{
    public class UICloseLevelPreviewComponent : MonoBehaviour
    {
        public GameObject levelPreviewPanel;
        public GameObject levelChoicePanel;
        public GameObject bonusChoicePanel;

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => ButtonOnClickEvent());
        }

        public void ButtonOnClickEvent()
        {
            levelChoicePanel.GetComponent<UILevelChoiceComponent>().Devisualize();
            bonusChoicePanel.GetComponent<UIPotionChoiceComponent>().Devisualize();
            levelPreviewPanel.gameObject.SetActive(false);
        }
    }
}
