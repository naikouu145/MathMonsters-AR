using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    public string monsterName;
    public string rarity; // Common, Rare, Epic
    public int maxHealth = 100;
    public int currentHealth = 100; // Made public for debugging, defaults to maxHealth
    public Image healthBarFill; // Assign in Inspector - should be an Image with Fill type
    public float attackAnimationDuration = 1.5f; // Duration of attack animation
    public float attackMoveDistance = 0.5f; // How far forward the monster moves during attack
    public float attackMoveSpeed = 2f; // Speed of forward movement

    private Animator anim;
    private GameManager gameManager;
    private Vector3 originalPosition; // Store original position

    void Start()
    {
        anim = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Store original position
        originalPosition = transform.position;
        
        // Always reset currentHealth to maxHealth at start
        currentHealth = maxHealth;
        
        // Auto-find health bar if not assigned
        if (healthBarFill == null)
        {
            GameObject monsterHPForeground = GameObject.Find("MonsterHPForeground");
            if (monsterHPForeground != null)
            {
                healthBarFill = monsterHPForeground.GetComponent<Image>();
            }
        }
        
        // Force fill to 1.0 at start
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
        }
        
        UpdateHealthBar();
    }

    public void PerformAttack()
    {
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // Store current position as original before attack
        originalPosition = transform.position;
        
        // Pause battle and hide UI
        if (gameManager != null)
        {
            gameManager.PauseBattle();
        }

        // Play attack animation
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            Debug.LogWarning($"Monster '{gameObject.name}' has no Animator. Skipping Attack trigger.");
        }

        // Move forward towards camera
        Vector3 targetPosition = originalPosition + transform.forward * attackMoveDistance;
        float moveTimer = 0f;
        float moveDuration = attackAnimationDuration * 0.3f; // Move during first 30% of animation
        
        while (moveTimer < moveDuration)
        {
            moveTimer += Time.deltaTime;
            float progress = moveTimer / moveDuration;
            transform.position = Vector3.Lerp(originalPosition, targetPosition, progress);
            yield return null;
        }

        // Hold position briefly
        yield return new WaitForSeconds(attackAnimationDuration * 0.4f); // Hold for 40% of animation

        // Move back to original position
        moveTimer = 0f;
        float returnDuration = attackAnimationDuration * 0.3f; // Return during last 30% of animation
        Vector3 currentPos = transform.position;
        
        while (moveTimer < returnDuration)
        {
            moveTimer += Time.deltaTime;
            float progress = moveTimer / returnDuration;
            transform.position = Vector3.Lerp(currentPos, originalPosition, progress);
            yield return null;
        }
        
        // Ensure we're exactly at original position
        transform.position = originalPosition;

        // Return to idle
        if (anim != null)
        {
            anim.SetTrigger("Idle");
        }

        // Resume battle and show UI
        if (gameManager != null)
        {
            gameManager.ResumeBattle();
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        UpdateHealthBar();
        
        if (anim != null)
        {
            anim.SetTrigger("Hit");
            StartCoroutine(ReturnToIdle(1f)); // Adjust timing based on your hit animation length
        }
        else
        {
            Debug.LogWarning($"{nameof(MonsterController)} on '{gameObject.name}' has no Animator. Skipping Hit trigger.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float fillValue = Mathf.Clamp01((float)currentHealth / maxHealth);
            healthBarFill.fillAmount = fillValue;
            
            // Debug to check values
            Debug.Log($"Monster Health: {currentHealth}/{maxHealth} = {fillValue}");
        }
        else
        {
            Debug.LogWarning("Monster health bar fill image not assigned.");
        }
    }

    private IEnumerator ReturnToIdle(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (anim != null && currentHealth > 0)
        {
            anim.SetTrigger("Idle");
        }
    }

    void Die()
    {
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        else
        {
            Debug.LogWarning($"{nameof(MonsterController)} on '{gameObject.name}' has no Animator. Skipping Die trigger.");
        }

        if (gameManager != null)
        {
            gameManager.OnMonsterDefeated(this);
        }
        else
        {
            Debug.LogWarning("GameManager not found. OnMonsterDefeated not called.");
        }

        Destroy(gameObject, 2f);
    }
}
