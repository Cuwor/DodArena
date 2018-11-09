using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void NextMusicHandler();

public interface IPinChanged
{
    void PinChanged(bool pin);
}

public enum Gametype
{
    Wave,
    Timer,
    Test
}

public class PlayerUI : MyTools, IHit, IPinChanged
{
    public GameObject cam;

    public GameObject crossline;

    private float crosslineScale;

    public Image damagePanel;

    [HideInInspector] public Animator damagePanelAnim;

    public Animator deadPanel;
    public Text displayMode;

    [Tooltip("Слайдер для здоровья")] public Slider health;

    private float hit;
    public Image[] linesAutogun;
    public Image[] linesShotgun;
    private bool lockMusicKey;
    public int maxScale;
    public int minScale;
    private bool timelock;

    private Animator musicAnim;
    public Slider musickSlider;

    [HideInInspector] public SinglePlayerController pc;

    public Animator pinHit;

    public GameObject pinPistol;

    [HideInInspector] public Animator pinPistolAnim;

    public GameObject pinShotgun;

    [HideInInspector] public Animator pinShotgunAnim;

    [HideInInspector] public float scale;

    public float stat;

    public Text[] statistics;
    public GameObject ReturnButton;

    public bool SurvaivalKey;
    public Text tipText;
    public Gametype type;

    public float Hit1
    {
        get { return hit; }

        set
        {
            hit = value;
            pinHit.SetFloat("Hit", value);
        }
    }

    public void Hit()
    {
        Hit1++;
    }

    public void PinChanged(bool pin)
    {
        if (pin)
            if (pinShotgun.activeSelf)
            {
            }
    }

    public event NextMusicHandler musicEvent;

   
    public void KakayaToFunxia()
    {
        GetComponent<SinglePlayerController>().Death();
    }

    public void DrawWave(int number)
    {
        Debug.Log("&&&");
        displayMode.text = "волна " + number.ToString();
    }



    public void WhatType()
    {
        if (type == Gametype.Timer)
        {
            SurvaivalKey=true;
            timelock = true;
        }
    }

    private void Start()
    {
        timelock = true;
        foreach (var c in statistics) c.gameObject.SetActive(false);
        pinPistolAnim = pinPistol.gameObject.GetComponent<Animator>();
        pinShotgunAnim = pinShotgun.gameObject.GetComponent<Animator>();
        crosslineScale = 1;
        tipText.text = string.Empty;
        lockMusicKey = false;
        musicAnim = musickSlider.gameObject.GetComponent<Animator>();
        damagePanelAnim = damagePanel.GetComponent<Animator>();
        musicAnim.SetBool("On", false);
        damagePanelAnim.SetFloat("LowToNormal", 1);
        SetToPistol();
    }

    private void FixedUpdate()
    {
        //DrawCrossline();
        //if(scale != crosslineScale)
        //{
        //    StartCoroutine("ChangeScale");
        //}
        if (!pc.death)
        {
            if (Hit1 > 0) Hit1 -= 0.1f;
            if (!lockMusicKey) NextMusic();
        }

        if (SurvaivalKey && timelock ) StartCoroutine(Timer());
    }

    public IEnumerator Timer()
    {
        //hi!
        Debug.Log("Gease");
        timelock = false;
        yield return new WaitForSeconds(1);
        
        timelock = true;
        stat++;
        
        displayMode.text = stat.ToString();
    }

    public void ActiveDamagePanel()
    {
        damagePanelAnim.SetTrigger("Damage");
    }

    public void SetToPistol()
    {
        pinPistol.SetActive(true);
        foreach (var im in linesAutogun)
            im.gameObject.SetActive(false);
        pinShotgun.SetActive(false);
    }

    public void SetToAutogun()
    {
        pinPistol.SetActive(true);
        foreach (var im in linesAutogun)
            im.gameObject.SetActive(true);
        pinShotgun.SetActive(false);
    }

    public void SetToShotgun()
    {
        pinShotgun.SetActive(true);
        pinPistol.SetActive(false);
    }

    private void DrawCrossline()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100))
            crosslineScale = Mathf.Clamp(Vector3.Distance(cam.transform.position, hit.transform.position) / 10,
                minScale, maxScale);
        else
            crosslineScale = 5;
    }

    private IEnumerator ChangeScale()
    {
        for (var i = 0; i < 100; i++)
        {
            var s = Mathf.Sign(scale - crosslineScale);
            var step = scale + s * 0.005f;
            if (s > 0 && step < crosslineScale || s > 0 && step < crosslineScale)
                scale = step;
            else
                scale = crosslineScale;
            crossline.transform.localScale = new Vector3(scale, scale, 0);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UsedObject obj;
        if (MyGetComponent(out obj, other.gameObject)) tipText.text = obj.tip;
    }

    private void OnTriggerStay(Collider other)
    {
        UsedObject obj;
        if (MyGetComponent(out obj, other.gameObject))
            if (Input.GetKeyDown(KeyCode.F) && !pc.inDialog)
            {
                Dialog dialog;
                if (MyGetComponent(out dialog, other.gameObject))
                {
                    Cursor.lockState = CursorLockMode.None;
                    dialog.player = pc;
                    pc.inDialog = true;
                }

                obj.Use();
                CleanTip();
            }
    }

    private void OnTriggerExit(Collider other)
    {
        UsedObject obj;
        if (MyGetComponent(out obj, other.gameObject)) CleanTip();
    }

    private void CleanTip()
    {
        tipText.text = string.Empty;
    }

    private void NextMusic()
    {
        if (Input.GetKey(KeyCode.C))
        {
            musicAnim.SetBool("On", true);
            musickSlider.value += 0.5f * Time.deltaTime;
            if (musickSlider.value >= 1)
                if (musicEvent != null)
                {
                    musickSlider.value = 0;
                    musicEvent.Invoke();
                    lockMusicKey = true;
                    Invoke("BackFalseLock", 2);
                }
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            musickSlider.value = 0;
            musicAnim.SetBool("On", false);
        }
    }

    private void BackFalseLock()
    {
        lockMusicKey = false;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void GetResults()
    {
        statistics[0].text = "Счёт: " + gameObject.GetComponent<SinglePlayerController>().candyCount.text;
        statistics[0].gameObject.SetActive(true);
        ReturnButton.SetActive(true);
        if(ReturnButton)

        if (type == Gametype.Wave)
        {
            statistics[1].text = "Количество волн: " + displayMode.text;
            statistics[1].gameObject.SetActive(true);
        }
        else if (type == Gametype.Timer)
        {
            SurvaivalKey = false;
            statistics[2].text = "Время: " + displayMode.text;
            statistics[2].gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
    }
}