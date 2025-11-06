using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100; // Made public for debugging, defaults to maxHealth
    public Image healthBarFill; // Assign in Inspector - should be an Image with Fill type

    void Start()
    {
        // Always reset currentHealth to maxHealth at start
        currentHealth = maxHealth;

        // Auto-find health bar if not assigned
        if (healthBarFill == null)
        {
            GameObject hpForeground = GameObject.Find("HPForeground");
            if (hpForeground != null)
            {
                healthBarFill = hpForeground.GetComponent<Image>();
            }
        }

        // Force fill to 1.0 at start
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
        }

        UpdateHealthBar();
    }

    public void Attack(MonsterController target)
    {
        target.TakeDamage(Random.Range(15, 25));
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log("Player healed to full health!");
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float fillValue = Mathf.Clamp01((float)currentHealth / maxHealth);
            healthBarFill.fillAmount = fillValue;

            // Debug to check values
            Debug.Log($"Health: {currentHealth}/{maxHealth} = {fillValue}");
        }
        else
        {
            Debug.LogWarning("Health bar fill image not assigned.");
        }
    }

    void Die()
    {
        var gm = FindObjectOfType<GameManager>();

        if (gm != null)
        {
            gm.OnPlayerDefeated();
        }
        else
        {
            Debug.LogWarning("GameManager not found. OnPlayerDefeated not called.");
        }
    }

    public int GetHealth() => currentHealth;
}
