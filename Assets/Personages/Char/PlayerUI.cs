﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void NextMusicHandler();

public class PlayerUI : MyTools {

    public GameObject cam;
    public int minScale;
    public int maxScale;
    [HideInInspector]
    public float scale;
    public Text tipText;
    public Slider musickSlider;

    private float crosslineScale;
    private bool lockMusicKey;
    
    public GameObject crossline;

    [HideInInspector]
    public SinglePlayerController pc;
    public event NextMusicHandler musicEvent;

    private void Start()
    {
        crosslineScale = 1;
        tipText.text = string.Empty;
        lockMusicKey = false;

    }

    private void Update()
    {
        DrawCrossline();
        if(scale != crosslineScale)
        {
            StartCoroutine("ChangeScale");
        }

        if (!lockMusicKey)
        {
            NextMusic();
        }
    }

    private void DrawCrossline()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100))
        {
            crosslineScale = Mathf.Clamp(Vector3.Distance(cam.transform.position, hit.transform.position) / 10, minScale, maxScale);
        }
        else
        {
            crosslineScale = 5;
        }
    }

    private IEnumerator ChangeScale()
    {
        for(int i = 0; i < 100; i++)
        {
            float s = Mathf.Sign(scale - crosslineScale);
            float step = scale + s * 0.005f;
            if((s > 0 && step < crosslineScale) || (s > 0 && step < crosslineScale))
            {
                scale = step;
            }
            else
            {
                scale = crosslineScale;
            }
            crossline.transform.localScale = new Vector3(scale, scale, 0);
            yield return new WaitForSeconds(0.1f);
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        
        UsedObject obj;
        if (MyGetComponent(out obj, other.gameObject))
        {
            
            tipText.text = obj.tip;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        UsedObject obj;
        if(MyGetComponent(out obj, other.gameObject))
        {
            if(Input.GetKeyDown(KeyCode.F) && !pc.inDialog)
            {
                Dialog dialog;
                if(MyGetComponent(out dialog, other.gameObject))
                {
                    Cursor.lockState = CursorLockMode.None;
                    dialog.player = pc;
                    pc.inDialog = true;
                }
                obj.Use();
                CleanTip();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        UsedObject obj;
        if (MyGetComponent(out obj, other.gameObject))
        {
            CleanTip();
        }
    }

    private void CleanTip()
    {
        tipText.text = string.Empty;
    }

    private void NextMusic()
    {
        if(Input.GetKey(KeyCode.C))
        {
            
            musickSlider.value += 0.5f*Time.deltaTime;
            if(musickSlider.value >= 1)
            {
                Debug.Log("!");
                if(musicEvent != null)
                {
                    musickSlider.value = 0;
                    musicEvent.Invoke();
                    lockMusicKey = true;
                    Invoke("BackFalseLock",2);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            musickSlider.value = 0;
        }
    }

    private void BackFalseLock()
    {
        lockMusicKey = false;
    }
}
