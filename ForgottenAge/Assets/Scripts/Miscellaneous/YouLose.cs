using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouLose : MonoBehaviour
{
    public GameObject youLose;
    public GameObject mainBrain;

    void Start()
    {
        youLose.SetActive(false);

    }

    void Update()
    {
        if (mainBrain == null)
        {
            youLose.SetActive(true);
        }
    }

    public void RestartScene()
    {
        // Get the active scene
        Scene activeScene = SceneManager.GetActiveScene();
        // Reload the active scene
        SceneManager.LoadScene(activeScene.name);
    }


}
