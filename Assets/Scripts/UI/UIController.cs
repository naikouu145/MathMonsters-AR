using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text timerText;

    public void UpdateTimer(float remainingTime, float normalized)
    {
        // Guard: ensure timerText is assigned
        if (timerText == null) return;

        // Display time with 1 decimal place
        timerText.text = remainingTime.ToString("F1") + "s";
    }
}
