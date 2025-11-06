using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI timerTextTMP; // For TextMeshPro timer
    public GameObject questionsPanel;
    public GameObject[] battleUIElements; // Array of UI elements to show during battle

    private void Start()
    {
        HideAllBattleElements();
        EnsureUIOnTop();
    }

    public void ShowBattleUI()
    {
        EnsureUIOnTop();

        // Show all battle UI elements
        if (battleUIElements != null)
        {
            foreach (GameObject element in battleUIElements)
            {
                if (element != null)
                {
                    element.SetActive(true);
                }
            }
        }

        // Legacy support - show questions panel if not in array
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(true);
        }

        ShowTimer();
    }

    public void HideAll()
    {
        HideAllBattleElements();
    }

    private void HideAllBattleElements()
    {
        // Hide all battle UI elements
        if (battleUIElements != null)
        {
            foreach (GameObject element in battleUIElements)
            {
                if (element != null)
                {
                    element.SetActive(false);
                }
            }
        }

        // Legacy support
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(false);
        }

        HideTimer();
        // Removed: gameObject.SetActive(false); - Canvas stays active
    }

    public void ShowQuestionsPanel()
    {
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(true);
        }
    }

    public void HideQuestionsPanel()
    {
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(false);
        }
    }

    public void ShowTimer()
    {

        if (timerTextTMP != null)
        {
            timerTextTMP.gameObject.SetActive(true);
        }
    }

    public void HideTimer()
    {
        if (timerTextTMP != null)
        {
            timerTextTMP.gameObject.SetActive(false);
        }
    }

    public void UpdateTimer(float remainingTime, float normalized)
    {
        // Update TextMeshPro component with ceiling value
        if (timerTextTMP != null)
        {
            timerTextTMP.text = Mathf.Ceil(remainingTime).ToString("0");
        }
    }

    private void EnsureUIOnTop()
    {
        Canvas uiCanvas = GetComponent<Canvas>();
        if (uiCanvas == null)
        {
            uiCanvas = GetComponentInParent<Canvas>();
        }

        if (uiCanvas != null)
        {
            uiCanvas.sortingOrder = 1000;

            if (uiCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
    }
}
