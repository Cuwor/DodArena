using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioScriptController : MyTools, IAlive
{
	public event DeathHandler OnDead;

	public float health;
	
	public float Health
	{
		get
		{
			return health;
		}

		set
		{
			if(value < 0)
			{
				
				Death();

				health = 0;
			}
			health = value;
		}
	}
	
	public void GetDamage(float value)
	{
		Health -= value;
	}

	public void PlusHealth(float value)
	{
		throw new System.NotImplementedException();
	}

	public void Death()
	{
		if (OnDead != null)
		{
			OnDead.Invoke();
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		AttackArea proj;
		if(MyGetComponent(out proj, other.gameObject))
		{
			GetDamage(proj.Damage);
		}
	}
	
	

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public delegate void DeathHandler();

