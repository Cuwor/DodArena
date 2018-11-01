using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> _ui;

    void Start()
    {
        ResetUi();
        ToMenu();
    }

    public void BackToTheGame()
    {
        ResetUi();
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
        Application.Quit();
    }


    private void ResetUi()
    {
        foreach (var el in _ui)
        {
            el.SetActive(false);
        }
    }
}