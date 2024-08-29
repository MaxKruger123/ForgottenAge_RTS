using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject areaOfEffect;
    private GameObject spawnedAreaOfEffect;

    public List<Button> shopButtons;

    public Concentration concentration;

    private int cost;

    // UI References
    public UIDetector shopPanel;
    public UIDetector shopButtonPanel;
    // Start is called before the first frame update
    void Start()
    {
        
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

                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 19.04f; 
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                
                spawnedAreaOfEffect.transform.position = worldPosition;

                if (Input.GetMouseButtonDown(0))// apply effect
                {
                    spawnedAreaOfEffect.GetComponent<AreaOfEffect>().ApplyEffect();
                    Destroy(spawnedAreaOfEffect);
                    spawnedAreaOfEffect = null; // Reset to prevent further interaction

                    // makes buttons interactable again
                    foreach (Button button in shopButtons)
                    {
                        button.interactable = true;
                    }
                }
                else if (Input.GetMouseButtonDown(1)) // cancel effect
                {
                    Destroy(spawnedAreaOfEffect);
                    spawnedAreaOfEffect = null; // Reset to prevent further interaction
                    Debug.Log("Cancelled");
                    // refund money
                    concentration.AddConcentration(cost);
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
                if(concentration.concentration < 5)
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
        string selectedItem = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        int itemPrice = 5;
        if (concentration.concentration >= itemPrice)
        {
            SpawnAreaOfEffet(selectedItem);
            cost = itemPrice;
            concentration.SubtractConcentration(cost);
        }
        
    }

    public void SpawnAreaOfEffet(string type)
    {
        // Instantiate the area of effect at the mouse position
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 19.04f; // Set the Z distance to the camera (based on your camera setup)
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        spawnedAreaOfEffect = Instantiate(areaOfEffect, worldPosition, Quaternion.identity);
      
        spawnedAreaOfEffect.GetComponent<AreaOfEffect>().SetEffectType(type);

        if(type == "Bomb")
        {
            spawnedAreaOfEffect.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.66f); 
        }
    }


}
