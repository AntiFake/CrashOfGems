using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Alchemy
{
    public class UIOpenLevelPreviewComponent : MonoBehaviour
    {
        public GameObject levelPreviewPanel;
        public LevelType levelType;
        public DifficultyType difficultyType;

        private Button btnOpenLevelPreview;

        private void Start()
        {
            btnOpenLevelPreview = GetComponent<Button>();
            btnOpenLevelPreview.onClick.AddListener(() =>
            {
                levelPreviewPanel.GetComponent<UILevelPreviewComponent>().Visualize(levelType, difficultyType);
            });
        }
    }
}
