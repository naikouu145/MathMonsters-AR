using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image timerBar;
    public Color safeColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;

    public void UpdateTimer(float normalized)
    {
        // Guard: ensure timerBar is assigned
        if (timerBar == null) return;

        // Clamp normalized to valid range
        normalized = Mathf.Clamp01(normalized);

        timerBar.fillAmount = normalized;
        if (normalized > 0.6f) timerBar.color = safeColor;
        else if (normalized > 0.3f) timerBar.color = warningColor;
        else timerBar.color = dangerColor;
    }
}
