using System.Collections;
using System.Collections.Generic;
using Al_AI.Scripts;
using UnityEngine;

public class ServivalManagerScript : MonoBehaviour
{
	public List<Spavn> spavns;
	public float cooldown = 30;
	// Use this for initialization
	void Start ()
	{
		Dialog.OnStart += SpavnStart;
		foreach (var VARIABLE in spavns)
			VARIABLE.SMS = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpavnStart()
	{
		Debug.Log("SpavnStart");
		foreach (var VARIABLE in spavns)
			VARIABLE.State();
		Invoke("DownCooldown",cooldown);
		
	}

	public void DownCooldown()
	{
		foreach (var VARIABLE in spavns)
			VARIABLE.cooldown--;
		Invoke("DownCooldown",cooldown);
	}
	
}
