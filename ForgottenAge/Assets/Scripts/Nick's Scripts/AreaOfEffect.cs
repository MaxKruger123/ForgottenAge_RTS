using UnityEngine;
using UnityEngine.AI;

public class AreaOfEffect : MonoBehaviour
{
    
    private string effectType;
    // Start is called before the first frame update
   
    private bool isActive=false;

    private ShopManager shopManager;

    void Start()
    {
        shopManager = GameObject.Find("ShopMenu").GetComponent<ShopManager>();
        
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
                    //collider.gameObject.GetComponent<AllyTroopStats>().TakeHeals(100);
                    //Debug.Log("Effect Applied to:" + collider.name);
                }
            }
            else if (effectType == "Bomb")
            {
                if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyRanged") || collider.CompareTag("Kamikaze") || collider.CompareTag("Enemy_Tank"))
                {
                    collider.gameObject.GetComponent<EnemyStats>().TakeDamage(8);
                    Debug.Log("Effect Applied to:" + collider.name);
                }
                else if (collider.CompareTag("RepairTroop") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
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
    public string GetEffectType()
    {
        return effectType;
    }


    public void OnTriggerStay2D(Collider2D collision) // HEALING
    {

        if (isActive)
        {
            if (effectType == "Heal Troops")
            {
                if (collision.CompareTag("RepairTroop") || collision.CompareTag("Player") || collision.CompareTag("AllyHealing") || collision.CompareTag("AllyRanged") || collision.CompareTag("AllyTank"))
                {
                    collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
                    collision.gameObject.GetComponent<AllyTroopStats>().TakeHeals(0.05f);
                }
            }
        }
        
    }


    public void SetEffectActive(bool b)
    {
        isActive = b;
    }

  


    public void Bomb()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 5);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyRanged") || collider.CompareTag("Kamikaze") || collider.CompareTag("Enemy_Tank"))
            {
                collider.gameObject.GetComponent<EnemyStats>().TakeDamage(8);
                Debug.Log("Effect Applied to:" + collider.name);
            }
            else if (collider.CompareTag("RepairTroop") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
            {
                collider.gameObject.GetComponent<AllyTroopStats>().TakeDamage(8);
                Debug.Log("Effect Applied to:" + collider.name);
            }
        }
    }


    public void Freeze()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, 5);
        StartCoroutine(shopManager.FreezeCountDown(colliders));
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyRanged") || collider.CompareTag("Kamikaze") || collider.CompareTag("Enemy_Tank"))
            {
                collider.gameObject.GetComponent<EnemyTroop>().enabled = false;
                collider.gameObject.GetComponent<NavMeshAgent>().speed = 0;
                //collider.attachedRigidbody.velocity = new Vector2(0,0);
                Debug.Log("Effect Applied to:" + collider.name);
            }
            else if (collider.CompareTag("RepairTroop") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
            {
                collider.gameObject.GetComponent<AllyTroop>().enabled = false;
                collider.gameObject.GetComponent<NavMeshAgent>().speed = 0;
                //collider.attachedRigidbody.velocity = new Vector2(0,0);
                Debug.Log("Effect Applied to:" + collider.name);
            }
        }
    }
}
