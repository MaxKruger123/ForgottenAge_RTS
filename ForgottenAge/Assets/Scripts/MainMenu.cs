using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject tutorialPage;

    public void TutPage()
    {
        tutorialPage.SetActive(true);
        Debug.Log("Open Tut Page");
    }

    public void CloseTutPage()
    {
        tutorialPage.SetActive(false);
    }

    public void PlayGame()
    {
        // Load the scene named "SampleScene"
        SceneManager.LoadScene("SampleScene");
    }
}
