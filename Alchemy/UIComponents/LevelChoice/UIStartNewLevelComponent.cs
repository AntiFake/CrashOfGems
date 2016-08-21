using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Alchemy.Model;
using System.Collections.Generic;

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
            List<PotionType> selectedPotions = new List<PotionType>();

            levelChoicePanel.GetComponent<UILevelChoiceComponent>().GetSelectedDifficultyAndLevelTypes(out lvlType, out dflcType);
            selectedPotions = bonusChoicePanel.GetComponent<UIPotionChoiceComponent>().GetSelectedBonusPotions();

            // Удалить количество выбранных зелий.
            GameManager.Instance.ConsumeLevelPotions(selectedPotions);

            // Сохранить найстройки уровня.
            GameManager.Instance.SaveNewLevelSettings(lvlType, dflcType, selectedPotions);

            SceneManager.LoadScene("Level");
        }
    }
}
