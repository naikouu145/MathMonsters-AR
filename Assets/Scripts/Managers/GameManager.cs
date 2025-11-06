using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Add TextMeshPro namespace

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIController ui;
    public MathQuestionManager questionManager;
    public AudioSource audioSource;
    public Image muteButtonImage;
    public Sprite mutedSprite;
    public Sprite unmutedSprite;
    public TextMeshProUGUI scoreText; // Add TMP text reference for score display
    public int gameOverSceneIndex = 2; // Scene to load on player death (set in Inspector)
    public AudioSource gameOverAudioSource; // Changed: AudioSource for game over audio
    
    private MonsterController currentMonster;
    private float questionTimer;
    private float timeLimit = 10f;
    private bool isTimerActive = false;
    private bool isBattleActive = false;
    private bool isBattlePaused = false;
    private int score = 0; // Add score tracking

    void Start()
    {
        // UI controller handles its own initialization
        UpdateScoreDisplay(); // Initialize score display
    }

    void Update()
    {
        if (isBattleActive && isTimerActive && !isBattlePaused)
        {
            questionTimer -= Time.deltaTime;

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
        
        if (ui != null)
        {
            ui.ShowQuestionsPanel();
            ui.ShowTimer();
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

        if (ui != null)
        {
            ui.HideQuestionsPanel();
        }
        
        isTimerActive = false;

        if (currentMonster != null)
        {
            currentMonster.PerformAttack();
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
        
        // Add score points
        AddScore(100); // Award 100 points per defeated monster
        
        // Heal player to full health
        if (player != null)
        {
            player.HealToFull();
        }
        
        EndBattle();
    }

    public void OnPlayerDefeated()
    {
        Debug.Log("Player Defeated!");
        EndBattle();
        
        // Play game over audio and load scene
        StartCoroutine(PlayAudioAndLoadGameOver());
    }

    private IEnumerator PlayAudioAndLoadGameOver()
    {
        // Play game over audio if assigned
        if (gameOverAudioSource != null && gameOverAudioSource.clip != null)
        {
            gameOverAudioSource.Play();
            
            // Wait for audio to finish playing (or maximum 3 seconds)
            float audioDuration = Mathf.Min(gameOverAudioSource.clip.length, 3f);
            yield return new WaitForSeconds(audioDuration);
        }
        else
        {
            // If no audio, just wait a short moment
            yield return new WaitForSeconds(1f);
            
            if (gameOverAudioSource == null)
            {
                Debug.LogWarning("Game Over AudioSource not assigned.");
            }
            else if (gameOverAudioSource.clip == null)
            {
                Debug.LogWarning("Game Over AudioSource has no AudioClip assigned.");
            }
        }
        
        // Save score before loading new scene
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
        
        LoadScene(gameOverSceneIndex);
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

        // Destroy all other monsters
        DestroyOtherMonsters(monster);

        if (ui != null)
        {
            ui.ShowBattleUI();
        }

        StartQuestion(monster.rarity);

        Debug.Log("Battle started with " + monster.monsterName);
    }

    private void DestroyOtherMonsters(MonsterController currentMonster)
    {
        // Find all game objects with the "Monster" tag
        GameObject[] allMonsters = GameObject.FindGameObjectsWithTag("Monster");
        
        int destroyedCount = 0;
        foreach (GameObject monsterObj in allMonsters)
        {
            // Skip the current monster in battle
            if (monsterObj != currentMonster.gameObject)
            {
                Destroy(monsterObj);
                destroyedCount++;
            }
        }
        
        if (destroyedCount > 0)
        {
            Debug.Log($"Destroyed {destroyedCount} other monster(s) to focus on battle.");
        }
    }

    private void EndBattle()
    {
        isBattleActive = false;
        isTimerActive = false;
        currentMonster = null;

        if (ui != null)
        {
            ui.HideAll();
        }

        Debug.Log("Battle ended.");
    }

    // Score management methods
    private void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
        Debug.Log($"Score: {score}");
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public int GetScore() => score;

    // Scene management methods
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void ExitApplication()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Audio management methods
    public void ToggleMute()
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
            
            if (muteButtonImage != null)
            {
                muteButtonImage.sprite = audioSource.mute ? mutedSprite : unmutedSprite;
            }
        }
        else
        {
            Debug.LogWarning("AudioSource not assigned.");
        }
    }

    public void PauseBattle()
    {
        isBattlePaused = true;
        
        if (ui != null)
        {
            ui.HideQuestionsPanel();
            ui.HideTimer();
        }
    }

    public void ResumeBattle()
    {
        isBattlePaused = false;
        
        // Show next question after monster attack
        OnMonsterReturnedToIdle();
    }
}
