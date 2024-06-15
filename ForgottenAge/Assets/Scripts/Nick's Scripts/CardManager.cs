using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    public CardScreen cardScreen;
    public Concentration concentration;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConcentrationCard()
    {
        concentration.ConcentrationIncome(concentration.income + cardScreen.GetCardData(0).buffValue);
    }

    public void FocusCard()
    {
        concentration.AddConcentration(cardScreen.GetCardData(3).buffValue);
        // troop cost +5
    }
}
