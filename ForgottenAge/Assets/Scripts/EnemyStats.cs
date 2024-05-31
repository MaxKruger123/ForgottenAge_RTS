using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth = 10;
    public float currentHealth;

    public Image healthBar;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }

        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
