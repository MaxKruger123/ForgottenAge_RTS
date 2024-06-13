using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentMenuTwo : MonoBehaviour
{

    public Concentration concentration;
    public List<Button> buttons;


    


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (concentration.GetConcentration() < 5)
        {
            buttons[0].interactable = false;
        }
        else
        {
            buttons[0].interactable = true;
        }

        if (concentration.GetConcentration() < 10)
        {
            buttons[1].interactable = false;
        }
        else
        {
            buttons[1].interactable = true;
        }
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void SetButton(Building building)
    {
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => building.SpawnTankTroop());
        buttons[1].onClick.RemoveAllListeners();
        buttons[1].onClick.AddListener(() => building.SpawnRangedTroop());
    }
}




