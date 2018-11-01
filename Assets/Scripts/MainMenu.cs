using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> _ui;

    void Start()
    {
        MainMenuActive();
    }

    public void MainMenuActive()
    {
        ResetUi();
        _ui[0].SetActive(true);
        _ui[1].SetActive(false);
    }

    public void SinglePlayerActive()
    {
        ResetUi();
        _ui[2].SetActive(true);
    }

    public void MultiPlayerActive()
    {
        ResetUi();
        _ui[3].SetActive(true);
    }

    public void SettingsActive()
    {
        ResetUi();
        _ui[4].SetActive(true);
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void IfBoy()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void IfGirl()
    {
        SceneManager.LoadSceneAsync(1);
    }

    private void ResetUi()
    {
        foreach (var el in _ui)
        {
            el.SetActive(false);
        }

        _ui[1].SetActive(true);
    }
}