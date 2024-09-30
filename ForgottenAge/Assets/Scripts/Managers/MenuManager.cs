using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> Menus;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetMenuObject(string menuName)
    {
        foreach (GameObject menu in Menus)
        {
            if(menu.name == menuName)
            {
                Debug.Log(menu.name);
                return menu;
            }
        }
        return null;
    }
}
