using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator shopButtonPanelAnimation;
    public Animator shopPanelAnimation;
    public GameObject shopPanel;
    public Sprite closeImage;

    private bool menuIsOpen=false;
    private Sprite shopImage;
    // Start is called before the first frame update
    void Start()
    {
        shopImage = gameObject.GetComponent<Image>().sprite;
        
        
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
            
            menuIsOpen = true;
            shopPanelAnimation.SetBool("PlayAnimation", true);
            shopPanelAnimation.SetBool("ReverseAnimation", false);
        }
        else
        {
            
            menuIsOpen = false;
            gameObject.GetComponent<Image>().sprite = shopImage;
            shopPanelAnimation.SetBool("PlayAnimation", false);
            shopPanelAnimation.SetBool("ReverseAnimation", true);
        }
            
    }

    public void CloseMenu()
    {
        menuIsOpen = false;
        gameObject.GetComponent<Image>().sprite = shopImage;
        shopPanelAnimation.SetBool("PlayAnimation", false);
        shopPanelAnimation.SetBool("ReverseAnimation", true);
        transform.GetChild(0).gameObject.SetActive(false);
        shopButtonPanelAnimation.SetBool("ReverseAnimation", true);
        shopButtonPanelAnimation.SetBool("PlayAnimation", false);
    }
}
