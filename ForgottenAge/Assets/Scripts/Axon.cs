using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Axon : MonoBehaviour
{
    public float maxHealth = 50f; // Maximum health of the Axon
    public float currentHealth; // Current health of the Axon

    public Image healthBar;

    public SpriteRenderer[] spriteRenderers; // Array to hold the sprite renderers of the three objects

    void Start()
    {
        currentHealth = maxHealth; // Set current health to max health at the start
    }

    void Update()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    // Function to take damage
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            ChangeColorToGray();
        }
    }

    // Function to heal the Axon
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Ensure health does not exceed the maximum
        }
    }

    // Function to change the color of the three objects to gray
    void ChangeColorToGray()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = Color.gray;
        }
    }
}
