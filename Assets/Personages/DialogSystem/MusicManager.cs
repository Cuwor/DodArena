using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : UsedObject
{
    public AudioClip track;
    public AudioSource aud;

    public override void Use()
    {
        if(aud.isPlaying)
        {
            aud.Stop();
        }
        aud.clip = track;
        aud.Play();
    }
}
