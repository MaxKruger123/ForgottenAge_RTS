using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour
{
    public GameObject particle;
    private CardManager cardManager;
    void Start()
    {
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyRanged") || collision.CompareTag("Kamikaze"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(cardManager.allyRangedDamage);
                
                Destroy(gameObject);
            }
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {
        
    }
}
