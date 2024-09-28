using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject areaOfEffect;
    public GameObject bombPrefab;
    public GameObject freezePrefab;
    private GameObject spawnedAreaOfEffect;

    public List<Button> shopButtons;

    public Concentration concentration;

    private int cost;

    // UI References
    public UIDetector shopPanel;
    public UIDetector shopButtonPanel;

    // Animators
    public Animator bombAnimator;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (spawnedAreaOfEffect != null)
        {
            foreach (Button button in shopButtons)
            {
                button.interactable = false;
            }
            if (shopPanel.isMouseOver || shopButtonPanel.isMouseOver)
            {
                // If mouse is over the UI panel, hide the area of effect
                spawnedAreaOfEffect.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                // If mouse is not over the UI panel, show the area of effect
                spawnedAreaOfEffect.GetComponent<Renderer>().enabled = true;
                // Updates the circles location
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 19.04f; 
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                spawnedAreaOfEffect.transform.position = worldPosition;

                if (Input.GetMouseButtonDown(0))// apply effect
                {
                    //spawnedAreaOfEffect.GetComponent<AreaOfEffect>().ApplyEffect();
                    string areaOfEffectType = spawnedAreaOfEffect.GetComponent<AreaOfEffect>().GetEffectType();


                    if (areaOfEffectType == "Heal Troops")
                    {
                        spawnedAreaOfEffect.GetComponent<AreaOfEffect>().SetEffectActive(true);
                        audioManager.PlaySFX(audioManager.healing);
                        GameObject particleEffect = spawnedAreaOfEffect.transform.GetChild(0).gameObject;
                        particleEffect.GetComponent<ParticleSystem>().Play();
                        particleEffect.transform.SetParent(null);
                        particleEffect.transform.localScale = new Vector2(1,1);
                        Destroy(particleEffect, 10);
                    }
                    else if(areaOfEffectType == "Bomb")
                    {
                        audioManager.PlaySFX(audioManager.bomb);
                        bombAnimator.SetBool("BlowUp", true);
                        
                        
                    }
                    else if (areaOfEffectType == "Freeze")
                    {
                        audioManager.PlaySFX(audioManager.freeze);
                        spawnedAreaOfEffect.GetComponent<AreaOfEffect>().Freeze();
                        spawnedAreaOfEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                    }
                    
                    Destroy(spawnedAreaOfEffect,5); // DESTROY IN 5 SECONDS
                    spawnedAreaOfEffect = null; // Reset to prevent further interaction

                    // makes buttons interactable again
                    foreach (Button button in shopButtons)
                    {
                        button.interactable = true;
                    }
                }
                else if (Input.GetMouseButtonDown(1)) // cancel effect
                {
                    audioManager.PlaySFX(audioManager.menuClickReversed);
                    Destroy(spawnedAreaOfEffect);
                    spawnedAreaOfEffect = null; // Reset to prevent further interaction
                    Debug.Log("Cancelled");
                    // refund money
                    concentration.AddDreamTokens(cost);
                    // makes buttons interactable again
                    foreach (Button button in shopButtons)
                    {
                        button.interactable = true;
                    }
                }
            }
        }
        else
        {
            foreach (Button button in shopButtons)
            {
                // MAKE SO THAT IF YOU DONT HAVE ENOUGH MONEY YOU CANT BUY
                if(concentration.GetDreamTokens() < int.Parse(button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text))
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
                
            }
        }

        
    }

   
    public void PurchaseItem()// price cost and switching between different aoes
    {
        audioManager.PlaySFX(audioManager.menuClick);
        string selectedItem = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        int itemPrice = int.Parse(EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        if (concentration.concentration >= itemPrice)
        {
            SpawnAreaOfEffect(selectedItem);
            cost = itemPrice;
            concentration.SubtractDreamTokens(cost);
        }
        
    }

    public void SpawnAreaOfEffect(string type)
    {
        // Instantiate the area of effect at the mouse position
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 19.04f; // Set the Z distance to the camera (based on your camera setup)
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        if(type == "Heal Troops")
        {
            spawnedAreaOfEffect = Instantiate(areaOfEffect, worldPosition, Quaternion.identity);
            spawnedAreaOfEffect.GetComponent<AreaOfEffect>().SetEffectType(type);
        }
        else if(type == "Bomb")
        {
            spawnedAreaOfEffect = Instantiate(bombPrefab, worldPosition, Quaternion.identity);
            spawnedAreaOfEffect.GetComponent<AreaOfEffect>().SetEffectType(type);
            bombAnimator = spawnedAreaOfEffect.transform.GetChild(1).GetComponent<Animator>();
        }else if (type == "Freeze")
        {
            spawnedAreaOfEffect = Instantiate(freezePrefab, worldPosition, Quaternion.identity);
            spawnedAreaOfEffect.GetComponent<AreaOfEffect>().SetEffectType(type);
        }
    }


    public IEnumerator FreezeCountDown(Collider2D[] colliders)
    {
        
        yield return new WaitForSecondsRealtime(4.5f);
        foreach (Collider2D collider in colliders)
        {
            if(collider != null)
            {
                if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyRanged") || collider.CompareTag("Kamikaze") || collider.CompareTag("Enemy_Tank"))
                {
                    collider.gameObject.GetComponent<EnemyTroop>().enabled = true;
                    collider.gameObject.GetComponent<NavMeshAgent>().speed = 2;
                    //collider.attachedRigidbody.velocity = new Vector2(0,0);
                    Debug.Log("COUNT DOWN TIMER Effect Applied to:" + collider.name);
                }
                else if (collider.CompareTag("RepairTroop") || collider.CompareTag("AllyHealing") || collider.CompareTag("AllyRanged") || collider.CompareTag("AllyTank"))
                {
                    collider.gameObject.GetComponent<AllyTroop>().enabled = true;
                    collider.gameObject.GetComponent<NavMeshAgent>().speed = 2;
                    //collider.attachedRigidbody.velocity = new Vector2(0,0);
                    Debug.Log("COUNT DOWN TIMER Effect Applied to:" + collider.name);
                }
            }
            
        }
    }


}
