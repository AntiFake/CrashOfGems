using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Alchemy.Model;

namespace Alchemy.UI
{
    public class UIStartNewLevelComponent : MonoBehaviour
    {
        public GameObject bonusChoicePanel;
        public GameObject levelChoicePanel;

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => ButtonOnClickEvent());
        }

        /// <summary>
        /// Событие нажатия на кнопку "Начать уровень". 
        /// </summary>
        private void ButtonOnClickEvent()
        {
            LevelType lvlType;
            DifficultyType dflcType;
            levelChoicePanel.GetComponent<UILevelChoiceComponent>().GetSelectedDifficultyAndLevelTypes(out lvlType, out dflcType);
            GameManager.Instance.SaveNewLevelSettings(lvlType, dflcType, bonusChoicePanel.GetComponent<UIPotionChoiceComponent>().GetSelectedBonusPotions());

            SceneManager.LoadScene("Level");
        }
    }
}
