using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardData
{
    
    public int buff;
    public int debuff;
    public Sprite image;
    public string title;
    public string descriptionText;
    public string buffType;
    public string debuffType;

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
