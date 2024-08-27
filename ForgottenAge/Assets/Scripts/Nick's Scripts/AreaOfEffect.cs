using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    
    private CircleCollider2D circleCollider;
    
    private string effectType;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyEffect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (Collider2D collider in colliders)
        {
            if(effectType == "Heal Troops")
            {
                if (collider.CompareTag("Player") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
                {
                    collider.gameObject.GetComponent<AllyTroopStats>().TakeHeals(100);
                }
            }
            else if (effectType == "Bomb")
            {
                if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyRanged"))
                {
                    collider.gameObject.GetComponent<EnemyStats>().TakeDamage(8);
                }
                else if (collider.CompareTag("Player") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
                {
                    collider.gameObject.GetComponent<AllyTroopStats>().TakeDamage(8);
                }
            }
            
            
            
            
        }
    }

    public void SetEffectType(string type)
    {
        effectType = type;
    }
}
