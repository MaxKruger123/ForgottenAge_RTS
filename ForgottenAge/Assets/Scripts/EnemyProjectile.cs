using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject particle;
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("AllyRanged"))
        {
            AllyTroopStats allyTroopStats = collision.GetComponent<AllyTroopStats>();
            if (allyTroopStats != null)
            {
                allyTroopStats.TakeDamage(1.0f);

                Destroy(gameObject);
            }
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }

    void Update()
    {

    }
}
