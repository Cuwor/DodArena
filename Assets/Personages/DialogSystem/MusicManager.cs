using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : UsedObject
{
    public AudioClip track;
    public AudioSource aud;
    public Text musicName;

    private void Start()
    {
        musicName.text = aud.clip.name;
    }

    public override void Use()
    {
        if(aud.isPlaying)
        {
            aud.Stop();
        }
        aud.clip = track;
        aud.Play();

        musicName.text = aud.clip.name;
    }
}
