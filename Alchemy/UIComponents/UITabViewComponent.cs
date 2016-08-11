using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace Alchemy
{
    [Serializable]
    public class TabButtonContent
    {
        public Button tabButton;
        public GameObject tabContent;
    }

    public class UITabViewComponent : MonoBehaviour
    {
        public List<TabButtonContent> tabButtonContents;
        public Sprite activeTabButtonSprite;
        public Sprite nonActiveTabButtonSprite;

        private void Start()
        {
            tabButtonContents.ForEach(i => i.tabButton.onClick.AddListener(() => TabButtonOnClickEvent(i)));
        }

        private void TabButtonOnClickEvent(TabButtonContent tbc)
        {
            tabButtonContents.ForEach(i => i.tabContent.SetActive(false));
            ResetTabButtonsStyle();

            SetOpenedStyle(tbc);
            tbc.tabContent.SetActive(true);
        }

        private void ResetTabButtonsStyle()
        {
            tabButtonContents.ForEach(i => i.tabButton.gameObject.GetComponent<Image>().sprite = nonActiveTabButtonSprite);
        }

        private void SetOpenedStyle(TabButtonContent tbc)
        {
            tbc.tabButton.GetComponent<Image>().sprite = activeTabButtonSprite;
        }
    }
}
