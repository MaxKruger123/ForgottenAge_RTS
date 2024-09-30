using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour
{
    public GameObject particle;
    private CardManager cardManager;

    public AudioManagerr audioManager;
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyRanged") || collision.CompareTag("Kamikaze") || collision.CompareTag("Enemy_Tank"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            Instantiate(particle, transform.position, Quaternion.identity);
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(cardManager.allyRangedDamage);
                audioManager.SFX.PlayOneShot(audioManager.impact);
                Destroy(gameObject);
            }
            
        }
    }

    void Update()
    {
        
    }
}
