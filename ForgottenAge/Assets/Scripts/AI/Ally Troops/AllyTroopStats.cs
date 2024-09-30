using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class AllyTroopStats : MonoBehaviour
{
    public float maxHealth = 10;
    public float currentHealth;
    public AudioManagerr audioManager;
    public Image healthBar;
    public CardManager cardManager;
    public GameObject deathEffect;

    public void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        currentHealth = maxHealth;
        if (gameObject.tag == "Player")
        {
            maxHealth = cardManager.allyMeleeMaxHealth;
            currentHealth = maxHealth;
        }
    }

    void Update()
    {

        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            Instantiate(deathEffect, gameObject.transform.position, Quaternion.identity);
            audioManager.SFX.PlayOneShot(audioManager.death1);
            Destroy(gameObject);
        }
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void TakeHeals(float amount)
    {
        
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
