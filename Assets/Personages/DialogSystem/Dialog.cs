﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate void StartHandler();
public class Dialog : MonoBehaviour {

    public event StartHandler OnStart;
    public DialogNode[] nodes;
    public int activeNode;
    public bool Show;
    public SinglePlayerController player;

	// Use this for initialization
	void Start () {
	    
        Show = false;
	}

    private void OnGUI()
    {
        if(Show)
        {
            GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 600, 250), "");
            GUI.Label(new Rect(Screen.width / 2, Screen.height - 250, 500, 50), nodes[activeNode].npcText);
            for (int i = 0; i < nodes[activeNode].answers.Length; i++)
            {
                if (GUI.Button(new Rect(50, 50 * i + 10, 500, 50), nodes[activeNode].answers[i].text))
                {
                    if (nodes[activeNode].answers[i].endDialog)
                    {
                        player.inDialog = false;
                        Show = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    for(int j = 0; j < nodes[activeNode].answers[i].actions.Length; j++)
                    {
                        UsedObject obj = nodes[activeNode].answers[i].actions[j];
                        if (obj is MusicManager)
                        {
                            MusicManager MM = obj.GetComponent<MusicManager>();
                            MM.tracks = nodes[activeNode].answers[i].playList;
                            PlayerUI PUI = player.gameObject.GetComponent<PlayerUI>();
                            PUI.musicEvent += MM.OnNextTreck;
                            OnStart += PUI.WhatType;
                            gameObject.GetComponent<RadioScriptController>().OnDead+=PUI.KakayaToFunxia;
                            
                           
                            if (OnStart != null)
                            {
                                Debug.Log("!!!");
                                OnStart.Invoke();
                            }
                            
                        }
                        nodes[activeNode].answers[i].actions[j].Use();

                    }
                    activeNode = nodes[activeNode].answers[i].nextDialogNode;
                }
            }
        }
    }


}

[System.Serializable]
public class DialogNode
{
    [Tooltip("Реплика персонажа, с которым говорим")]
    public string npcText;
    [Tooltip("Варианты ответа")]
    public Answer[] answers;
}

[System.Serializable]
public class Answer
{
    [Tooltip("Текст ответа")]
    public string text;
    [Tooltip("Номер следующего узла диалога")]
    public int nextDialogNode;
    [Tooltip("Заканчивает диалог")]
    public bool endDialog;

    public UsedObject[] actions;
    public AudioClip[] playList;
}
