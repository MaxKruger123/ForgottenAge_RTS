using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ConstructionMenu : MonoBehaviour
{
    public Concentration concentration;
    public Button deconstructButton; // Reference to the deconstruct button
    private TutorialManager tutorialManager;
    public List<TextMeshProUGUI> prices;

    public List<Button> buttons;

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager == null)
        {
            Debug.LogWarning("TutorialManager not found in the scene.");
        }
    }

    void OnEnable()
    {
        Debug.Log("Construction menu enabled");
        tutorialManager = FindObjectOfType<TutorialManager>();
        if (tutorialManager != null)
        {
            tutorialManager.OnBuildMenuOpened();
        }
        else
        {
            Debug.LogWarning("TutorialManager not found in the scene.");
        }
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

        if (concentration.GetConcentration() < int.Parse(prices[4].text.TrimEnd('c')))
        {
            buttons[4].interactable = false;
        }
        else
        {
            buttons[4].interactable = true;
        }

        if (concentration.GetConcentration() < int.Parse(prices[5].text.TrimEnd('c')))
        {
            buttons[5].interactable = false;
        }
        else
        {
            buttons[5].interactable = true;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void SetPrices(int barracksPrice, int towerPrice, int upgradedBarracksPrice, int conStoragePrice, int upgradedTowerPrice, int areaTowerPrice)
    {

        prices[0].text = barracksPrice + "c";

        prices[1].text = towerPrice + "c";

        prices[2].text = upgradedBarracksPrice + "c";

        prices[3].text = conStoragePrice + "c";

        prices[4].text = upgradedTowerPrice + "c";

        prices[5].text = areaTowerPrice + "c";

    }
}
