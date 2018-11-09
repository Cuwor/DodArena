using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MusicManager : UsedObject
{
    public AudioClip[] tracks;
    public AudioSource aud;
    public Text musicName;
    public bool key;
    public static bool musicKey;
    public int number;

    private void Start()
    {
        number = 0;
        key = true;
        musicName.text = aud.clip.name;
    }

    private void Next()
    {
        if (number == tracks.Length - 1)
        {
            number = 0;
        }
        else
        {
            number++;
        }
        aud.clip = tracks[number];
        Use();
    }

    public override void Use()
    {
        musicKey = true;
        if(aud.isPlaying)
        {
            aud.Stop();
        }
        aud.clip = tracks[number];
        aud.Play();

        musicName.text = aud.clip.name;
    }

    private void FixedUpdate()
    {
        if(key)
        {
            StartCoroutine(CheckClip());
        }
    }

    private IEnumerator CheckClip()
    {
        key = false;
        yield return new WaitForSeconds(5f);
        if(!aud.isPlaying)
        {
            Next();
        }
        key = true;
    }

    public void OnNextTreck()
    {
        Next();
    }
}
