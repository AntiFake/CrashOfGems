using UnityEngine;
using System.Collections;

namespace Alchemy
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
