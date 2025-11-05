using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIController ui;
    public MathQuestionManager questionManager;
    public TextMeshProUGUI timerText; // Assign in Inspector to display timer
    public GameObject questionsPanel; // Assign in Inspector - the panel containing questions
    private MonsterController currentMonster;
    private float questionTimer;
    private float timeLimit = 10f;
    private bool isTimerActive = false; // Start as false
    private bool isBattleActive = false; // Track if battle is active

    void Start()
    {
        // Don't start the battle automatically
        // Hide UI elements initially
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(false);
        }
        
        if (ui != null)
        {
            ui.gameObject.SetActive(false);
        }
        
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Only run timer if battle is active
        if (isBattleActive && isTimerActive)
        {
            questionTimer -= Time.deltaTime;

            // Update timer text
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(questionTimer).ToString("0");
            }

            if (ui != null)
            {
                ui.UpdateTimer(questionTimer, questionTimer / timeLimit);
            }

            if (questionTimer <= 0)
            {
                OnTimeOut();
            }
        }
    }

    void StartQuestion(string rarity)
    {
        if (!isBattleActive)
        {
            Debug.LogWarning("Cannot start question: No active battle.");
            return;
        }

        if (questionManager != null)
        {
            questionManager.GenerateQuestion(rarity);
        }
        else
        {
            Debug.LogWarning("MathQuestionManager not assigned. Cannot generate question.");
        }
        questionTimer = timeLimit;
        isTimerActive = true;
        
        // Show questions panel
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(true);
        }
        
        // Show timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }

    public void OnCorrectAnswer()
    {
        if (!isBattleActive)
        {
            Debug.LogWarning("No active battle. Ignoring answer.");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("Player not assigned. Cannot perform attack.");
            return;
        }
        if (currentMonster == null)
        {
            Debug.LogWarning("No current monster to attack.");
            return;
        }

        player.Attack(currentMonster);
        StartQuestion(currentMonster.rarity);
    }

    public void OnWrongAnswer()
    {
        if (!isBattleActive)
        {
            Debug.LogWarning("No active battle. Ignoring answer.");
            return;
        }

        // Hide questions panel during attack
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(false);
        }
        
        // Pause timer during attack
        isTimerActive = false;

        if (currentMonster != null)
        {
            var mAnim = currentMonster.GetComponent<Animator>();
            if (mAnim != null)
            {
                mAnim.SetTrigger("Attack");
                StartCoroutine(ReturnMonsterToIdle(1.5f)); // Adjust timing based on your attack animation length
            }
            else
            {
                Debug.LogWarning($"Monster '{currentMonster.name}' has no Animator. Skipping Attack trigger.");
                // If no animator, show next question immediately
                OnMonsterReturnedToIdle();
            }
        }
        else
        {
            Debug.LogWarning("No current monster for wrong answer.");
            OnMonsterReturnedToIdle();
        }

        if (player != null)
        {
            player.TakeDamage(Random.Range(10, 20));
        }
        else
        {
            Debug.LogWarning("Player not assigned. Cannot take damage.");
        }
    }

    private IEnumerator ReturnMonsterToIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentMonster != null)
        {
            var mAnim = currentMonster.GetComponent<Animator>();
            if (mAnim != null)
            {
                mAnim.SetTrigger("Idle");
            }
        }
        
        // Show next question after returning to idle
        OnMonsterReturnedToIdle();
    }

    private void OnMonsterReturnedToIdle()
    {
        if (!isBattleActive) return;

        if (currentMonster != null)
        {
            StartQuestion(currentMonster.rarity);
        }
        else
        {
            StartQuestion("Common");
        }
    }

    public void OnTimeOut()
    {
        if (!isBattleActive) return;
        OnWrongAnswer();
    }

    public void OnMonsterDefeated(MonsterController monster)
    {
        Debug.Log($"{monster.monsterName} defeated!");
        EndBattle();
    }

    public void OnPlayerDefeated()
    {
        Debug.Log("Player Defeated!");
        EndBattle();
    }

    public void StartBattle(MonsterController monster)
    {
        if (monster == null)
        {
            Debug.LogError("Cannot start battle: Monster is null!");
            return;
        }

        currentMonster = monster;
        isBattleActive = true;

        // Enable battle UI
        if (ui != null)
        {
            ui.gameObject.SetActive(true);
        }

        // Show questions panel
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(true);
        }

        // Show timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        // Start first question
        StartQuestion(monster.rarity);

        Debug.Log("Battle started with " + monster.monsterName);
    }

    private void EndBattle()
    {
        isBattleActive = false;
        isTimerActive = false;
        currentMonster = null;

        // Hide UI elements
        if (questionsPanel != null)
        {
            questionsPanel.SetActive(false);
        }

        if (ui != null)
        {
            ui.gameObject.SetActive(false);
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        Debug.Log("Battle ended.");
    }
}
