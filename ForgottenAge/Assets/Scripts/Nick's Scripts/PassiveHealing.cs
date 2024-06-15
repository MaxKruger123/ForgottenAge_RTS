using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveHealing : MonoBehaviour
{
    private float timeCounter;
    private CardManager cardManager;


    // Start is called before the first frame update
    void Start()
    {
        cardManager = GameObject.Find("CardScreen").GetComponent<CardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cardManager.passiveHealing)
        {
            timeCounter += Time.deltaTime;

            if (timeCounter >= 1.0f)
            {
                gameObject.GetComponent<AllyTroopStats>().TakeHeals(1);
                timeCounter -= 1.0f;
            }
        }
    
    }
}
