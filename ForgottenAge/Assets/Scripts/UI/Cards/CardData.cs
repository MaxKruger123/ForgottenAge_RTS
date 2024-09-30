using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardData
{
    public string title;
    public string buffType;
    public int buffValue;
    public string debuffType;
    public int debuffValue;
    public Sprite image;
    public string descriptionText;
    

    public CardData(string title, string buffType, int buffValue, string debuffType, int debuffValue, Sprite image, string descriptionText)
    {
        this.title = title;
        this.buffType = buffType;
        this.buffValue = buffValue;
        this.debuffType = debuffType;
        this.debuffValue = debuffValue;
        this.image = image;
        this.descriptionText = descriptionText;
    }

}
