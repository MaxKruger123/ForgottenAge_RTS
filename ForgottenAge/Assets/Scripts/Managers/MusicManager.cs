using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioManagerr audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManagerr>();
        //audioManager.PlayMusic(audioManager.GetSong(1));
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioManager.music.isPlaying)
        {
            audioManager.PlayMusic(audioManager.GetSong(1));
        }
    }
}
