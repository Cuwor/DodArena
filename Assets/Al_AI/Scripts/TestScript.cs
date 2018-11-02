using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MyTools
{
	public float Health;

	public Material mainmat;

	public Material damagedmat;
	
	// Use this for initialization
	void Start ()
	{
		gameObject.GetComponent<Renderer>().material = mainmat;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		Projectile proj;
		if (MyGetComponent(out proj, other.gameObject))
		{
			GetDamage(proj.damage);
		}
	}
	public void GetDamage(float value)
	{
		Health -= value;
		gameObject.GetComponent<Renderer>().material = damagedmat;
		Invoke("ResetRet",1f);
		
	}

	public void ResetRet()
	{
		gameObject.GetComponent<Renderer>().material = mainmat;
	}
}
