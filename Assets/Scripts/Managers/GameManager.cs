using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIController ui;
    public MathQuestionManager questionManager;
    private MonsterController currentMonster;
    private float questionTimer;
    private float timeLimit = 10f;

    void Start()
    {
        currentMonster = FindObjectOfType<MonsterController>();
        if (currentMonster == null)
        {
            Debug.LogWarning("No MonsterController found in scene at Start.");
        }
        else
        {
            StartQuestion(currentMonster.rarity);
        }
    }

    void Update()
    {
        questionTimer -= Time.deltaTime;
        if (ui != null)
        {
            ui.UpdateTimer(questionTimer, questionTimer / timeLimit);
        }
        else
        {
            // optional: only log once or during development
            // Debug.LogWarning("UIController not assigned. Timer updates skipped.");
        }

        if (questionTimer <= 0)
        {
            OnTimeOut();
        }
    }

    void StartQuestion(string rarity)
    {
        if (questionManager != null)
        {
            questionManager.GenerateQuestion(rarity);
        }
        else
        {
            Debug.LogWarning("MathQuestionManager not assigned. Cannot generate question.");
        }
        questionTimer = timeLimit;
    }

    public void OnCorrectAnswer()
    {
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
        if (currentMonster != null)
        {
            var mAnim = currentMonster.GetComponent<Animator>();
            if (mAnim != null)
            {
                mAnim.SetTrigger("Attack");
            }
            else
            {
                Debug.LogWarning($"Monster '{currentMonster.name}' has no Animator. Skipping Attack trigger.");
            }
        }
        else
        {
            Debug.LogWarning("No current monster for wrong answer.");
        }

        if (player != null)
        {
            player.TakeDamage(Random.Range(10, 20));
        }
        else
        {
            Debug.LogWarning("Player not assigned. Cannot take damage.");
        }

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
        OnWrongAnswer();
    }

    public void OnMonsterDefeated(MonsterController monster)
    {
        Debug.Log($"{monster.monsterName} defeated!");
    }

    public void OnPlayerDefeated()
    {
        Debug.Log("Player Defeated!");
    }
}
