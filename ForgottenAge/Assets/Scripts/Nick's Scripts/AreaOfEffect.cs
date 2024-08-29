using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    
    private CircleCollider2D circleCollider;

    
    private string effectType;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyEffect()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 5);
        

        foreach (Collider2D collider in colliders)
        {
            if(effectType == "Heal Troops")
            {
                if (collider.CompareTag("RepairTroop") || collider.CompareTag("Player") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
                {
                    collider.gameObject.GetComponent<AllyTroopStats>().TakeHeals(100);
                    Debug.Log("Effect Applied to:" + collider.name);
                }
            }
            else if (effectType == "Bomb")
            {
                if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyRanged") || collider.CompareTag("Kamikaze") || collider.CompareTag("Enemy_Tank"))
                {
                    collider.gameObject.GetComponent<EnemyStats>().TakeDamage(8);
                    Debug.Log("Effect Applied to:" + collider.name);
                }
                else if (collider.CompareTag("Player") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
                {
                    collider.gameObject.GetComponent<AllyTroopStats>().TakeDamage(8);
                    Debug.Log("Effect Applied to:" + collider.name);
                }
            }

            


        }
    }

    public void SetEffectType(string type)
    {
        effectType = type;
    }

   

    
}
