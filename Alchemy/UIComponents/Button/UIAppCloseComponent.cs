using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Компонент для кнопки, который обеспечивает закртие приложения.
/// </summary>
public class UIAppCloseComponent : MonoBehaviour
{
    private Button button;

	private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Application.Quit());
	}
}
