using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Slice_Script_Controller : MyTools, IAlive {

    [Tooltip("Здесь объект")]
	[Header("Здесь объект")]
	public GameObject Player;
	[Space(20)]
	public NavMeshAgent NavAgent;
	public float DistanceTP;
	[Range(1,10)]
	public float RadiusAttack;
	[Range(0,100)]
	public float HP;
    [Range(0,50)]
    public float AttackForce;
    public GameObject Eidolon;
    public bool saled;
    public bool key;

    public GameObject[] AttackAreas;

    private Animator _anim;
	private int qtyEidolons = 4;
    private int attackType;
    private float maxHP;
    private bool alive;

	public float Health
	{
		get
		{
			return HP;
		}

		set
		{
			if(value <= 0)
			{
				Death();
                alive = false;
				HP = 0;
			}
			HP = value;
		}
	}
	
	

	// Use this for initialization
	void Start ()
	{
		NavAgent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
        key = true;
        maxHP = Health;
        alive = true;
        for (int i = 0; i < AttackAreas.Length; i++)
        {
            AttackArea proj;
            if (MyGetComponent(out proj, AttackAreas[i]))
            {
                proj.Damage = AttackForce;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(alive)
        {
            DistanceTP = Vector3.Distance(Player.transform.position, transform.position);
            if (DistanceTP > RadiusAttack)
            {
                NavAgent.enabled = true;
                NavAgent.destination = Player.transform.position;
                _anim.SetInteger("Attack", 0);
                _anim.SetFloat("Xstate", 0);
                _anim.SetFloat("Ystate", 1);
            }
            else if (DistanceTP <= RadiusAttack)
            {
                NavAgent.enabled = false;
                attackType = UnityEngine.Random.Range(1, 3);
                _anim.SetInteger("Attack", attackType);
            }
        }
	}

	IEnumerator CreateEidolons()
	{
		yield return new WaitForSeconds(1f);
		GameObject eidolon;
		for (int i = 0; i < qtyEidolons; i++)
		{
			eidolon = Instantiate(Eidolon, GetRandomPositionForEidolons(), Quaternion.identity);
            Slice_Script_Controller SSC;
            if(MyGetComponent(out SSC, eidolon))
            {
                SSC.Player = Player;
                SSC.Health = maxHP / 2;
                SSC.AttackForce = AttackForce / 2;
            }
        }
	}
	IEnumerator Destroeded()
	{
		yield return new WaitForSeconds(2);
		Destroy(gameObject);
	}

	
	public void GetDamage(float value)
	{
		Health -= value;
		_anim.SetInteger("Damage", (int)value);
		_anim.SetTrigger("GetDamage");
		Debug.Log(Health);
	}

	public void PlusHealth(float value)
	{
		
	}

	public void Death()
	{
		_anim.SetTrigger("Dead");
		if(saled)
			StartCoroutine(CreateEidolons());
		StartCoroutine(Destroeded());
		key = false;
	}

    private Vector3 GetRandomPositionForEidolons()
    {
        float x, z;
        x = UnityEngine.Random.Range(-2, 3);
        z = UnityEngine.Random.Range(-2, 3);

        return transform.position + new Vector3(x, 0, z);
    }

    private void OnTriggerEnter(Collider other)
	{
        if(alive)
        {
            Projectile proj;
            if (MyGetComponent(out proj, other.gameObject))
            {
                GetDamage(proj.damage);
            }
        }
	}
}

