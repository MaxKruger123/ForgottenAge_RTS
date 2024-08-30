using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBomb : MonoBehaviour
{

    AreaOfEffect parent;
    
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.GetComponentInParent<AreaOfEffect>();
    }

    public void BlowUp()
    {
        parent.Bomb();
    }
}
