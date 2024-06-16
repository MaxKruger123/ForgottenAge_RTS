using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Concentration : MonoBehaviour
{

    public TextMeshProUGUI concentrationText;
    public TextMeshProUGUI dreamTokenText;
    public int concentration;
    public int maxConcentration;

    public int dreamTokens;

    private float timeCounter;

    public TileController tileController;

    public int income;
    public bool CS = false;
    public CardManager cardManager;

    void Start()
    {
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
        income = 2 + tileController.tilesCaptured;
        maxConcentration = 300;
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return; // Skip the update logic if the game is paused
        }

        concentrationText.text = " " + concentration;
        dreamTokenText.text = " " + dreamTokens;
        ConcentrationIncome(2 + cardManager.incomeModifier);

        if (concentration > maxConcentration)
        {
            concentration = maxConcentration;
        }

        if (CS == true)
        {
            maxConcentration = 500;
        }
    }

    public int GetConcentration()
    {
        return concentration;
    }

    public void SetConcentration(int num)
    {
        concentration = num;
    }

    public void AddConcentration(int num)
    {
        concentration += num;
    }

    public void SubtractConcentration(int num)
    {
        concentration -= num;
    }

    public void ConcentrationIncome(int num) // generates passive concentration every second. num is how much is added every second
    {
        timeCounter += Time.deltaTime;

        if(timeCounter >= 1.0f)
        {
            AddConcentration(num);
            timeCounter -= 1.0f;
        }
    }


}
