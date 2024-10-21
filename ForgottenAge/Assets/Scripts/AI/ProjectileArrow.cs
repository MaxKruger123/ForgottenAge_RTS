using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : MonoBehaviour
{
    public GameObject particle;
    private int damage = 2;
    private int Towerdamage = 5;

    public AudioManagerr audioManager;
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.name == "Projectile" && collision.CompareTag("Enemy") || collision.CompareTag("EnemyRanged") || collision.CompareTag("Kamikaze") || collision.CompareTag("Enemy_Tank"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            Instantiate(particle, transform.position, Quaternion.identity);
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
                audioManager.SFX.PlayOneShot(audioManager.impact);
                Destroy(gameObject);
            }
            
        }

        if (gameObject.tag == "TowerProjectile" && collision.CompareTag("Enemy") || collision.CompareTag("EnemyRanged") || collision.CompareTag("Kamikaze") || collision.CompareTag("Enemy_Tank"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            Instantiate(particle, transform.position, Quaternion.identity);
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(Towerdamage);
                audioManager.SFX.PlayOneShot(audioManager.impact);
                Destroy(gameObject);
            }

        }
    }

    void Update()
    {
        
    }
}
