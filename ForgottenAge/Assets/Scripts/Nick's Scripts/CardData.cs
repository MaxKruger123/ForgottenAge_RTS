using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardData
{
    public string title;
    public string buffType;
    public int buff;
    public string debuffType;
    public int debuff;
    public Sprite image;
    public string descriptionText;
    

    public CardData(string title, string buffType, int buff, string debuffType, int debuff, Sprite image, string descriptionText)
    {
        this.title = title;
        this.buffType = buffType;
        this.buff = buff;
        this.debuffType = debuffType;
        this.debuff = debuff;
        this.image = image;
        this.descriptionText = descriptionText;
    }

}
