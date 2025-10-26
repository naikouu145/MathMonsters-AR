using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void Attack(MonsterController target)
    {
        anim.SetTrigger("Attack");
        target.TakeDamage(Random.Range(15, 25));
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        anim.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Die");
        
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
