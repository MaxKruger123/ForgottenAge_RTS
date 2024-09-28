using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject tutorialPage;
    public AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.mainMenuMusic);
    }


    public void TutPage()
    {
        audioManager.PlaySFX(audioManager.menuClick);
        SceneManager.LoadScene("TutorialScene");
        Debug.Log("Open Tut Page");
    }

    public void CloseTutPage()
    {
        audioManager.PlaySFX(audioManager.menuClickReversed);
        tutorialPage.SetActive(false);
    }

    public void PlayGame()
    {
        // Load the scene named "SampleScene"
        audioManager.PlaySFX(audioManager.menuClick);
        SceneManager.LoadScene("SampleScene");
    }

    public void Quit()
    {
        audioManager.PlaySFX(audioManager.menuClickReversed);
        Application.Quit();
    }
}
