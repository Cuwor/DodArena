using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void NextMusicHandler();

public class PlayerUI : MyTools, IHit
{

    public GameObject cam;
    public int minScale;
    public int maxScale;
    [HideInInspector]
    public float scale;
    public Text tipText;
    public Slider musickSlider;

    private float crosslineScale;
    private bool lockMusicKey;

    [Tooltip("Слайдер для здоровья")]
    public Slider health;

    public GameObject crossline;

    public Image damagePanel;

    public GameObject pinShotgun;
    public Image[] linesShotgun;

    public GameObject pinPistol;
    public Image[] linesAutogun;

    [HideInInspector]
    public SinglePlayerController pc;
    public event NextMusicHandler musicEvent;

    private Animator musicAnim;
    [HideInInspector]
    public Animator damagePanelAnim;
    public Color32 damageColor;
    public Color32 nullColor;
    public Color32 mainColor;

    private bool setColor;

    private void Start()
    {
        crosslineScale = 1;
        tipText.text = string.Empty;
        lockMusicKey = false;
        musicAnim = musickSlider.gameObject.GetComponent<Animator>(); 
        damagePanelAnim = damagePanel.GetComponent<Animator>();
        musicAnim.SetBool("On", false);
        damagePanelAnim.SetFloat("LowToNormal", 1);
        SetToPistol();
        ActiveDamagePanel(false);
    }

    private void Update()
    {
        //DrawCrossline();
        //if(scale != crosslineScale)
        //{
        //    StartCoroutine("ChangeScale");
        //}

        if (!lockMusicKey)
        {
            NextMusic();
        }
    }

    public void ActiveDamagePanel(bool active)
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
        foreach (var im in linesAutogun)
            im.gameObject.SetActive(false);
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
        for (int i = 0; i < 100; i++)
        {
            float s = Mathf.Sign(scale - crosslineScale);
            float step = scale + s * 0.005f;
            if ((s > 0 && step < crosslineScale) || (s > 0 && step < crosslineScale))
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
        if (MyGetComponent(out obj, other.gameObject))
        {
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
        if (Input.GetKey(KeyCode.C))
        {
            musicAnim.SetBool("On", true);
            musickSlider.value += 0.5f * Time.deltaTime;
            if (musickSlider.value >= 1)
            {
                Debug.Log("!");
                if (musicEvent != null)
                {
                    musickSlider.value = 0;
                    musicEvent.Invoke();
                    lockMusicKey = true;
                    Invoke("BackFalseLock", 2);
                }
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

    public void Hit()
    {
        throw new System.NotImplementedException();
    }
}
