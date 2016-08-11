using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Alchemy
{
    public class UILevelPreviewComponent : MonoBehaviour
    {
        public Text levelTitle;
        public Image levelIcon;
        public GameObject levelChoice;
        public GameObject bonusChoice;
        public Button buttonReturn;
        public Button buttonStartLevel;

        public void Visualize(LevelType lvlType, DifficultyType dflcType)
        {
            var viewModel = GameManager.gameManager.GetLevelPreviewViewModel(lvlType, dflcType);

            levelTitle.text = viewModel.levelName;
            levelIcon.sprite = viewModel.levelImage;

            levelChoice.GetComponent<UILevelChoiceComponent>().Visualize(viewModel.difficultyLvls, lvlType);

            bonusChoice.GetComponent<UIPotionChoiceComponent>().Visualize(viewModel.potionBonuses);

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public void UpdateVisualization(LevelType lvlType, DifficultyType dflcType)
        {
            var viewModel = GameManager.gameManager.GetLevelPreviewViewModel(lvlType, dflcType);

            levelTitle.text = viewModel.levelName;
            levelIcon.sprite = viewModel.levelImage;

            var bonusComp = bonusChoice.GetComponent<UIPotionChoiceComponent>();
            bonusComp.Devisualize();
            bonusComp.Visualize(viewModel.potionBonuses);
        }

        public void Devisualize()
        {
            levelChoice.GetComponent<UILevelChoiceComponent>().Devisualize();
            bonusChoice.GetComponent<UIPotionChoiceComponent>().Devisualize();

            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }
}
