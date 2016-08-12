using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            SceneManager.LoadScene("Level");
        }
    }
}
