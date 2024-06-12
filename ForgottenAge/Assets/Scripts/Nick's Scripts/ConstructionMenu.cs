using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMenu : MonoBehaviour
{
    public Concentration concentration;
    public Button deconstructButton; // Reference to the deconstruct button

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (concentration.GetConcentration() < 20)
        {
            deconstructButton.interactable = false;
        }
        else
        {
            deconstructButton.interactable = true;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}
