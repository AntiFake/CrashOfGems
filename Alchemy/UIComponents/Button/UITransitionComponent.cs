using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Компонент, реализующий переход от одного "экрана" к другому в интерфейсе приложения.
/// </summary>
public class UITransitionComponent : MonoBehaviour
{
    public GameObject currentScreen;
    public GameObject nextScreen;
    private Button button;

    private void Start () {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ButtonOnClickEvent());	
	}

    private void ButtonOnClickEvent()
    {
        currentScreen.SetActive(false);
        nextScreen.SetActive(true);
    }
}
