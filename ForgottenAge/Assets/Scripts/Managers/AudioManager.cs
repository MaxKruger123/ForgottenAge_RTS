using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource music;
    public AudioSource SFX;

    public AudioClip menuClick;
    public AudioClip menuClickReversed;
    public AudioClip freeze;
    public AudioClip healing;
    public AudioClip bomb;
    public AudioClip mainMenuMusic;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip song)
    {
        music.clip = song;
        music.Play();
    }
}
