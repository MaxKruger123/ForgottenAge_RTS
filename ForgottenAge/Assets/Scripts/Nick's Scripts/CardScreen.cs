using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScreen : MonoBehaviour
{
    public WaveManager waveManagera;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowScreen()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        HideIconButton();
    }

    public void CloseScreen()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        HideIconButton();
        waveManagera.StartWave();
    }

    public void ShowIconButton()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void HideIconButton()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
}
