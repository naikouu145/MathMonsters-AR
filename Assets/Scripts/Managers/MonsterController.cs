using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public string monsterName;
    public string rarity; // Common, Rare, Epic
    public int maxHealth = 100;
    private int currentHealth;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (anim != null)
        {
            anim.SetTrigger("Hit");
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
