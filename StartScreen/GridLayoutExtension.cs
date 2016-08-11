using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GridLayoutExtension : MonoBehaviour {
    public int columnCount;
    public int rowCount;

    private RectTransform rectTransform;
    private GridLayoutGroup grid;

	private void Start ()
    {
        grid = gameObject.GetComponent<GridLayoutGroup>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        SetDynamicCellSize();
	}

    private void SetDynamicCellSize()
    {
        Vector2 cellSize = new Vector2(rectTransform.rect.width / columnCount, rectTransform.rect.height / rowCount);
        grid.cellSize = cellSize;
    }	
}
