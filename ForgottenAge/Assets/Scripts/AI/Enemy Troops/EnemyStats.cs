using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class EnemyStats : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 10f;
    public GameObject centerObject; // Reference to the center object of the enemy

    public Image healthBar;
    public GameObject damageIcon; // Reference to the damage icon

    public GameObject deathEffect;
    public GameObject currencyDrop;
    public GameObject currencyDropTwo;

    public Concentration concentration;

    private Concentration concentrationManager;

    public void Start()
    {
        currentHealth = maxHealth;
        concentrationManager = FindObjectOfType<Concentration>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        

        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    void Die()
    {
        // Handle death (e.g., play animation, remove from scene, etc.)
        Instantiate(deathEffect, gameObject.transform.position, Quaternion.identity);
        
        int random = Random.Range(0, 100);
        if (random >= 0 && random <= 85)
        {
            Instantiate(currencyDrop, gameObject.transform.position, Quaternion.identity);
            if (concentrationManager != null)
            {
                concentrationManager.AddConcentration(1);
            }
        } 
        else if (random >= 86 && random <= 95)
        {
            Instantiate(currencyDropTwo, gameObject.transform.position, Quaternion.identity);
            if (concentrationManager != null)
            {
                concentrationManager.AddDreamTokens(1);
            }
        }
        else if (random >= 96 && random <= 100)
        {
            // Do nothing (no drop)
        }
        Destroy(gameObject);
    }

    public void SetDamageIconActive(bool isActive)
    {
        if (damageIcon != null)
        {
            damageIcon.SetActive(isActive);
        }
    }

    // Method to get the center position of the enemy
    public Vector3 GetCenterPosition()
    {
        if (centerObject != null)
        {
            return centerObject.transform.position;
        }
        else
        {
            // If centerObject is not assigned, return the position of the enemy itself
            return transform.position;
        }
    }
}
