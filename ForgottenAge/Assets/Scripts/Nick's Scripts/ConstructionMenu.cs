using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ConstructionMenu : MonoBehaviour
{
    public Concentration concentration;
    public Button deconstructButton; // Reference to the deconstruct button

    public List<TextMeshProUGUI> prices;

    public List<Button> buttons;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (concentration.GetConcentration() < int.Parse(prices[0].text.TrimEnd('c')))
        {
            buttons[0].interactable = false;
        }
        else
        {
            buttons[0].interactable = true;
        }

        if (concentration.GetConcentration() < int.Parse(prices[1].text.TrimEnd('c')))
        {
            buttons[1].interactable = false;
        }
        else
        {
            buttons[1].interactable = true;
        }

        if (concentration.GetConcentration() < int.Parse(prices[2].text.TrimEnd('c')))
        {
            buttons[2].interactable = false;
        }
        else
        {
            buttons[2].interactable = true;
        }

        if (concentration.GetConcentration() < int.Parse(prices[3].text.TrimEnd('c')))
        {
            buttons[3].interactable = false;
        }
        else
        {
            buttons[3].interactable = true;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void SetPrices(int barracksPrice, int towerPrice, int upgradedBarracksPrice, int conStoragePrice)
    {

        prices[0].text = barracksPrice + "c";

        prices[1].text = towerPrice + "c";

        prices[2].text = upgradedBarracksPrice + "c";

        prices[3].text = conStoragePrice + "c";

    }
}
