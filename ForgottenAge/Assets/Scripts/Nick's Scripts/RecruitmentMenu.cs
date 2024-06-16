using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecruitmentMenu : MonoBehaviour
{
    public Concentration concentration;
    public List<Button> buttons;

    public List<TextMeshProUGUI> prices;

    
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

        if(concentration.GetConcentration() < int.Parse(prices[1].text.TrimEnd('c')))
        {
            buttons[1].interactable = false;
        }
        else
        {
            buttons[1].interactable = true;
        }

        if (concentration.dreamTokens < 1)
        {
            buttons[2].interactable = false;
            buttons[3].interactable = false;
        }
        else
        {
            buttons[2].interactable = true;
            buttons[3].interactable = true;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void SetButton(Building building)
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => building.SpawnTroop());
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(() => building.SpawnRangedTroop());
        buttons[2].onClick.RemoveAllListeners();
        buttons[2].onClick.AddListener(() => building.SpawnTroopInstant());
        buttons[3].onClick.RemoveAllListeners();
        buttons[3].onClick.AddListener(() => building.SpawnRangedTroopInstant());
    }

    public void SetPrices(int meleePrice, int rangedPrice)
    {
        
        // Melee Ally price
        prices[0].text = meleePrice + "c";

        // Ranged Ally price
        prices[1].text = rangedPrice + "c";

    }
}

