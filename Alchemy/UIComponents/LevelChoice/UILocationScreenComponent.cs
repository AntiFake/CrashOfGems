using UnityEngine;

namespace Alchemy.UI
{
    public class UILocationScreenComponent : MonoBehaviour
    {
        public GameObject levelPreviewPanel;

        private void OnDisable()
        {
            levelPreviewPanel.GetComponent<UILevelPreviewComponent>().Devisualize();
        }
    }
}
