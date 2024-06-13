using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour
{

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("EnemyRanged"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(1.0f);
                
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        
    }
}
