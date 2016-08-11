using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItemComponent : MonoBehaviour
{
    public Image inventoryItemIcon;
    public Text inventoryItemText;

    public void Visualize(Sprite sprite, string text)
    {
        inventoryItemIcon.sprite = sprite;
        inventoryItemText.text = text;
        gameObject.SetActive(true);
    }
}
