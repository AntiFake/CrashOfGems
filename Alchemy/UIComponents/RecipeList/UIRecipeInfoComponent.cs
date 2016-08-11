using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Alchemy
{
    public class UIRecipeInfoComponent : MonoBehaviour
    {
        public Text header;
        public Text description;
        public Image icon;

        public Button btnClose;

        public void Start()
        {
            btnClose.onClick.AddListener(() => BtnCloseOnClickEvent());
        }

        public void Visualize(string title, string description, Sprite sprite)
        {
            header.text = title;
            this.description.text = description;
            icon.sprite = sprite;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// При нажатии на кнопку "Закрыть" удаляем окно.
        /// </summary>
        private void BtnCloseOnClickEvent()
        {
            gameObject.SetActive(false);
        }
    }
}