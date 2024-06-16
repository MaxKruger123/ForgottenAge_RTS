using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingProjectile : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("AllyRanged") || collision.CompareTag("AllyTank"))
        {
            AllyTroopStats allyTroopStats = collision.GetComponent<AllyTroopStats>();
            if (allyTroopStats != null)
            {
                allyTroopStats.TakeHeals(1.0f);

                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
