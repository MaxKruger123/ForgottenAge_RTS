using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
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
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyRanged"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(cardManager.towerDamage);

                Destroy(gameObject);
            }
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }

    
}
