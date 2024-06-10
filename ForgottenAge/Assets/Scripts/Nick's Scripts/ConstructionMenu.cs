using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMenu : MonoBehaviour
{
    public Concentration concentration;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (concentration.GetConcentration() < 20)
        {
            gameObject.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
        else
        {
            gameObject.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
    }


    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
