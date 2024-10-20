using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
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
        if (collision.CompareTag("Player") || collision.CompareTag("AllyRanged") || collision.gameObject.CompareTag("AllyTank") || collision.gameObject.CompareTag("RepairTroop"))
        {
            AllyTroopStats allyTroopStats = collision.GetComponent<AllyTroopStats>();
            if (allyTroopStats != null)
            {
                allyTroopStats.TakeDamage(cardManager.enemyMeleeDamage);
                audioManager.SFX.PlayOneShot(audioManager.impact);
                Destroy(gameObject);
            }
            Instantiate(particle, transform.position, Quaternion.identity);
        }
        else if(collision.CompareTag("Building"))
        {
            BuildingStats buildingStats = collision.GetComponent<BuildingStats>();

            if (buildingStats != null)
            {
                buildingStats.TakeDamage(cardManager.enemyMeleeDamage);

                Destroy(gameObject);
            }
        }
    }

    void Update()
    {

    }
}
