using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Concentration : MonoBehaviour
{

    public TextMeshProUGUI concentrationText;
    public int concentration;

    private float timeCounter;

    void Start()
    {

    }

    void Update()
    {
        concentrationText.text = "Concentration: " + concentration ;
        ConcentrationIncome(1);
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
