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

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        
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

        var gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.OnMonsterDefeated(this);
        }
        else
        {
            Debug.LogWarning("GameManager not found. OnMonsterDefeated not called.");
        }

        Destroy(gameObject, 2f);
    }
}
