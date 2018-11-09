using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PlayerUi;
    [SerializeField] private List<GameObject> _ui;
    public bool pause = false;

    void Start()
    {
        ResetUi();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause = !pause;
            PauseOnOff();
        }
    }

    private void PauseOnOff()
    {
        if (pause)
        {
            Time.timeScale = 0.001f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PlayerUi.SetActive(false);
            ToMenu();  
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            PlayerUi.SetActive(true);
            ResetUi();
        }
        
    }
    public void BackToTheGame()
    {
        pause = false;
        PauseOnOff();
    }

    public void ToMenu()
    {
        ResetUi();
        _ui[0].SetActive(true);
    }

    public void SettingsActive()
    {
        ResetUi();
        _ui[1].SetActive(true);
    }

    public void QuitApp()
    {
        SceneManager.LoadSceneAsync(0);
    }


    private void ResetUi()
    {
        foreach (var el in _ui)
        {
            el.SetActive(false);
        }
    }
}