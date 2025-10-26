using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MathQuestionManager : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;

    private int correctAnswer;

    public void GenerateQuestion(string rarity)
    {
        // Basic guards
        if (answerButtons == null || answerButtons.Length == 0)
        {
            Debug.LogWarning("GenerateQuestion aborted: answerButtons not assigned or empty.");
            return;
        }

        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        string op = "+";

        switch (rarity)
        {
            case "Common": op = "+"; break;
            case "Rare": op = Random.value > 0.5f ? "+" : "-"; break;
            case "Epic": op = Random.value > 0.5f ? "×" : "÷"; break;
        }

        int result = op == "+" ? a + b :
                     op == "-" ? a - b :
                     op == "×" ? a * b :
                     Mathf.Max(1, a / b);

        correctAnswer = result;
        if (questionText != null)
            questionText.text = $"{a} {op} {b} = ?";
        else
            Debug.LogWarning("questionText is not assigned.");

        List<int> choices = new List<int> { result, result + 1, result - 1, result + 2 };

        // Ensure we have at least as many choices as buttons
        while (choices.Count < answerButtons.Length)
        {
            choices.Add(result + Random.Range(3, 10));
        }

        choices = choices.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            int value = choices[i];
            Text btnText = answerButtons[i].GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = value.ToString();
            else
                Debug.LogWarning($"Answer button at index {i} has no Text child.");

            // Ensure we capture the current value for the listener
            int captured = value;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswer(captured));
        }
    }

    void OnAnswer(int answer)
    {
        var gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogWarning("GameManager not found when answering question.");
            return;
        }

        if (answer == correctAnswer)
            gm.OnCorrectAnswer();
        else
            gm.OnWrongAnswer();
    }
}
