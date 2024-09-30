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
    public AudioClip death1;
    public AudioClip death2;
    public AudioClip impact;
    public AudioClip impact2;
    public AudioClip shoot1;
    public AudioClip shoot2;
    public AudioClip buildingBuilt;
    public AudioClip towerShoot1;
    public AudioClip towerShoot2;
    public AudioClip towerShoot3;
    public AudioClip towerShoot4;

    public AudioClip mainMenuMusic;

    public List<AudioClip> songs;

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

    public AudioClip GetSong(int i)
    {
        return songs[i];
    }
}
