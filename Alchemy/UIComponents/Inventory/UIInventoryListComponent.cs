using UnityEngine;

namespace Alchemy.UI
{
    public class UIInventoryListComponent : MonoBehaviour
    {
        public GameObject inventoryItemInstance;
        public GameObject scrollContent;

        #region События
        private void OnEnable()
        {
            VisualizeList();
        }

        private void OnDisable()
        {
            DevisualizeList();
        }
        #endregion

        #region Визуализация
        private void VisualizeList()
        {
            foreach (var inventoryItem in GameManager.Instance.GetPlayerInventory())
            {
                var listItem = Instantiate(inventoryItemInstance);
                listItem.transform.SetParent(scrollContent.transform, false);

                var inventoryItemComponent = listItem.GetComponent<UIInventoryItemComponent>();
                inventoryItemComponent.Visualize(inventoryItem.sprite, inventoryItem.count.ToString());
            }

            gameObject.SetActive(true);
        }

        private void DevisualizeList()
        {
            for (int i = 0; i < scrollContent.transform.childCount; i++)
            {
                var inventoryItem = scrollContent.transform.GetChild(i).gameObject;
                if (inventoryItem.activeSelf)
                    Destroy(inventoryItem);
            }
        }
        #endregion
    }
}
