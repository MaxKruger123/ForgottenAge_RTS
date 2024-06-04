using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentMenu : MonoBehaviour
{
    public Concentration concentration; 

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(concentration.GetConcentration() < 5)
        {
            gameObject.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }

        if(concentration.GetConcentration() < 10)
        {
            gameObject.transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.transform.GetChild(2).GetComponent<Button>().interactable = true;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
