using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITimerComponent : MonoBehaviour {
    public Text timer;
    public Color normalColor;
    public Color deadlineColor;
    public float deadlineThreshold;

    /// <summary>
    /// Установить значение таймера.
    /// </summary>
    /// <param name="timerValue">Время указывается в секундах</param>
    public void SetTimer(float timerValue)
    {
        int minutes = (int)(timerValue / 60);
        int seconds = (int)(timerValue % 60);

        if (timerValue <= deadlineThreshold)
            this.timer.color = deadlineColor;
        else
            this.timer.color = normalColor;

        timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        Debug.Log(timerValue);
    }
}
