using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator shopButtonPanelAnimation;
    public GameObject shopPanel;
    public Sprite closeImage;

    private bool menuIsOpen=false;
    private Sprite shopImage;
    // Start is called before the first frame update
    void Start()
    {
        shopImage = gameObject.GetComponent<Image>().sprite;
        
        if (shopButtonPanelAnimation == null)
        {
            Debug.LogError("Panel Animator not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!menuIsOpen)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            shopButtonPanelAnimation.SetBool("PlayAnimation", true);
            shopButtonPanelAnimation.SetBool("ReverseAnimation", false);
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = closeImage;
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!menuIsOpen)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            shopButtonPanelAnimation.SetBool("ReverseAnimation", true);
            shopButtonPanelAnimation.SetBool("PlayAnimation", false);
        }
        else 
        {
            gameObject.GetComponent<Image>().sprite = shopImage;
        }
            
    }

    public void OpenMenu()
    {
        if (!menuIsOpen)
        {
            shopPanel.SetActive(true);
            menuIsOpen = true;
            
        }
        else
        {
            shopPanel.SetActive(false);
            menuIsOpen = false;
            gameObject.GetComponent<Image>().sprite = shopImage;
        }
            
    }
}
