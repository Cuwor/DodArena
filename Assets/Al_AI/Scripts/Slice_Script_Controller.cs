using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Slice_Script_Controller : MonoBehaviour {
	[Tooltip("Здесь объект")]
	[Header("Здесь объект")]
	public GameObject Player;
	[Space(20)]
	public NavMeshAgent NavAgent;
	public float DistanceTP;
	[Range(1,10)]
	public float RadiusAttack;
	[Range(90,100)]
	public float HP =95;

	public bool key = true;

	public GameObject[] Eidolons;

	// Use this for initialization
	void Start ()
	{
		NavAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (HP <= 0 && key)
		{
			StartCoroutine(CreateEidolons());
			StartCoroutine(Destroeded());
			key = false;
		}
		DistanceTP = Vector3.Distance(Player.transform.position, transform.position);
		Debug.Log(DistanceTP);
		if (DistanceTP > RadiusAttack)
		{
			NavAgent.enabled = true;
			NavAgent.destination = Player.transform.position;
		}
		else if (DistanceTP<=RadiusAttack)
		{
			NavAgent.enabled = false;
			HP -= 100;
		}

	}

	IEnumerator CreateEidolons()
	{
		yield return new WaitForSeconds(1f);
		Instantiate(Eidolons[0], transform.position, Quaternion.identity);
		Instantiate(Eidolons[0], transform.position, Quaternion.identity);
		Instantiate(Eidolons[0], transform.position, Quaternion.identity);
		Instantiate(Eidolons[0], transform.position, Quaternion.identity);

	}
	IEnumerator Destroeded()
	{
		yield return new WaitForSeconds(2);
		Destroy(this.gameObject);
	}
}

