using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardScreen : MonoBehaviour
{
    public WaveManager waveManager;

    public List<CardData> cardDataList = new List<CardData>();
    public List<GameObject> drawnCards = new List<GameObject>();

    //public List<Sprite> cardImages = new List<Sprite>();

    public GameObject minimap;

    public GameObject eventButton;

    public CardManager cardManager;
    // Start is called before the first frame update
    void Start()
    {
        //CreateCardDataSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScreen()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        //minimap.SetActive(false);
        DrawFromDeck();
        HideIconButton();
    }

    public void CloseScreen()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //minimap.SetActive(true);
        HideIconButton();
        waveManager.StartWave();
    }

    public void ShowIconButton()
    {
        Debug.Log("SHOWS BUTTON");
        eventButton.SetActive(true);
    }

    public void HideIconButton()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    /*public void CreateCardDataSet()
    {
        // Title, bufftype, buffvalue, debuff, debuffvalue, sprite, description
        cardDataList.Add(new CardData("Concentration", "Concentration Per Second", 5, "", 0, cardImages[0], "Gain extra concentration per second"));

        cardDataList.Add(new CardData("Glass Cannon", "Ally Melee Damage", 2, "Ally Troop Health", 2, cardImages[0], "Your troops do more damage but are weaker"));

        cardDataList.Add(new CardData("The Dreamer", "Dream Token", 1, "", 0, cardImages[0], "Gain one dream token"));

        cardDataList.Add(new CardData("Focus", "Concentration", 100, "All Troop Cost", 5, cardImages[0], ""));

        cardDataList.Add(new CardData("Dreamer's Resolve", "Dream Token", 3, "Destroy Random Building", 2, cardImages[0], ""));

        cardDataList.Add(new CardData("Perilous Insight", "Concentration Per Second", 3, "Enemy damage multiply", 2, cardImages[0], ""));

        cardDataList.Add(new CardData("Fortified Respite", "Passive Tank Healing", 0, "Ally Ranged Reload", -1, cardImages[0], ""));

        cardDataList.Add(new CardData("Cursed Blessing", "Ally Damage", 2, "Memory Tile Are Lost", 2, cardImages[0], ""));

        cardDataList.Add(new CardData("Neuron Activation", "Concentration", 400, "Skip Wave", 3, cardImages[0], ""));

        cardDataList.Add(new CardData("Shifting Tides", "Gain Memory Tile", 2, "Chance Allies Become Enemies", 0, cardImages[0], ""));

        cardDataList.Add(new CardData("Cognitive Fortification", "Allies Health", 20, "", 0, cardImages[0], "%"));

        cardDataList.Add(new CardData("Structural Recall", "Heal All Buidlings", 1, "", 0, cardImages[0], ""));

        cardDataList.Add(new CardData("Synaptic Overload", "Defense Tower Damage", 2, "", 0, cardImages[0], ""));

    }*/

    public void DrawFromDeck()
    {
        int randNum;

        for(int i = 0; i < 3; i++)
        {
            randNum = Random.Range(0, cardDataList.Count);
            drawnCards[i].GetComponent<Card>().title.text = cardDataList[randNum].title; // sets drawn card title to the randomly selected cards title
            drawnCards[i].GetComponent<Card>().image.sprite = cardDataList[randNum].image; // sets drawn card image to the randomly selected cards image
            drawnCards[i].GetComponent<Card>().descriptionText.text = cardDataList[randNum].descriptionText;

            // buff
            drawnCards[i].GetComponent<Card>().buffText.text = cardDataList[randNum].buffType;
            // if there is no buff it wont display anything for it
            if (cardDataList[randNum].buffValue == 0)
            {
                drawnCards[i].GetComponent<Card>().buffEffectText.text = "";
            }
            else
            {
                drawnCards[i].GetComponent<Card>().buffEffectText.text = cardDataList[randNum].buffValue.ToString();
            }
            
            // debuff
            drawnCards[i].GetComponent<Card>().debuffText.text = cardDataList[randNum].debuffType;
            // if there is no debuff it wont display anything for it
            if (cardDataList[randNum].debuffValue == 0)
            {
                drawnCards[i].GetComponent<Card>().debuffEffectText.text = "";
            }
            else
            {
                drawnCards[i].GetComponent<Card>().debuffEffectText.text = cardDataList[randNum].debuffValue.ToString();
            }
                
            
        }
    }

    public CardData GetCardData(int index)
    {
        return cardDataList[index];
    }

   
    public void SelectCardLeft()
    {
        ApplyCardEffects(2);
        CloseScreen();
    }

    public void SelectCardMiddle()
    {
        ApplyCardEffects(1);
        CloseScreen();
    }

    public void SelectCardRight()
    {
        ApplyCardEffects(0);
        CloseScreen();
    }
    
    public void ApplyCardEffects(int num)
    {
        string cardTitle = drawnCards[num].GetComponent<Card>().title.text;
        switch (cardTitle)
        {
            case "Concentration":
                cardManager.ConcentrationCard();
                Debug.Log("Effect Applied");
                break;
            case "Glass Cannon":
                cardManager.GlassCannonCard();
                Debug.Log("Effect Applied");
                break;
            case "The Dreamer":
                cardManager.TheDreamerCard();
                Debug.Log("Effect Applied");
                break;
            case "Focus":
                cardManager.FocusCard();
                Debug.Log("Effect Applied");
                break;
            case "Dreamer's Resolve":
                cardManager.DreamersResolveCard();
                Debug.Log("Effect Applied");
                break;
            case "Perilous Insight":
                cardManager.PerilousInsightCard();
                Debug.Log("Effect Applied");
                break;
            case "Fortified Respite":
                cardManager.FortifiedRespiteCard();
                Debug.Log("Effect Applied");
                break;
            case "Cursed Blessing":
                cardManager.CursedBlessingCard();
                Debug.Log("Effect Applied");
                break;
            case "Neuron Activation":
                cardManager.NeuronActivationCard();
                Debug.Log("Effect Applied");
                break;
            case "Shifting Tides":
                cardManager.ShiftingTidesCard();
                Debug.Log("Effect Applied");
                break;
            case "Cognitive Fortification":
                cardManager.CognitiveFortificationCard();
                Debug.Log("Effect Applied");
                break;
            case "Structural Recall":
                cardManager.StructuralRecallCard();
                Debug.Log("Effect Applied");
                break;
            case "Synaptic Overload":
                cardManager.SynapticOverloadCard();
                Debug.Log("Effect Applied");
                break;
            default:
                Debug.Log("Error: Could not find card");
                break;
        }
    }

}
