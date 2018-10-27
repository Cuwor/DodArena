using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public abstract class Monster : MyTools, IAlive
{
    public GameObject target;


    [Space(20)] public NavMeshAgent NavAgent;
    [Range(1, 30)] public float RadiusAttack;
    [Range(1, 100)] public float RadiusView;
    [Range(0, 100)] public float HP;
    [Range(0, 50)] public float AttackForce;

    public GameObject[] AttackAreas;
    public GameObject[] ammos;

    protected EnemyState state;
    protected float distanceTP;
    protected bool alive;
    protected Animator _anim;
    protected bool wait;
    protected float maxHP;
    protected float size;
    protected int attackType;
    protected float attackDistance;

    public virtual float Health
    {
        get { return HP; }

        set
        {
            if (value <= 0)
            {
                Death();
                alive = false;
                HP = 0;
            }

            HP = value;
        }
    }

    public virtual float DistanceTP
    {
        get
        {
            return distanceTP;
        }

        set
        {
            distanceTP = value;
        }
    }

    public virtual EnemyState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    public virtual void Death()
    {
        NavAgent.enabled = false;
        _anim.SetTrigger("Dead");
        StartCoroutine(Drop());
        StartCoroutine(Destroeded());
    }

    protected IEnumerator Destroeded()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    protected IEnumerator GetRandomStayState()
    {
        _anim.SetFloat("Ystate", -1);
        _anim.SetFloat("Xstate", UnityEngine.Random.Range(-1, 1.1f));
        FindPlayers();
        yield return new WaitForSeconds(2);
        wait = false;
    }

    protected IEnumerator Drop()
    {
        yield return new WaitForSeconds(2);
        int x = UnityEngine.Random.Range(0, ammos.Length);
        Instantiate(ammos[x], transform.position, new Quaternion());
    }

    public void GetDamage(float value)
    {
        Health -= value;
        _anim.SetInteger("Damage", (int)value);
        _anim.SetTrigger("GetDamage");
    }

    public void PlusHealth(float value)
    {
    }

    public void Initiolize()
    {
        FindPlayers();
        NavAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        maxHP = Health;
        alive = true;
        wait = false;
        for (int i = 0; i < AttackAreas.Length; i++)
        {
            AttackArea proj;
            if (MyGetComponent(out proj, AttackAreas[i]))
            {
                proj.Damage = AttackForce;
            }
        }

        State = EnemyState.Stay;
        GetAttackDistance();
        size = transform.localScale.x;
    }

    protected void GetAttackDistance()
    {
        attackType = UnityEngine.Random.Range(1, 3);
        attackDistance = attackType == 1 ? RadiusAttack : RadiusAttack - 2 * size;
    }

    protected void FindPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float distance = Vector3.Distance(transform.position, players[0].transform.position);
        target = players[0];

        for (int i = 1; i < players.Length; i++)
        {
            float newDistance = Vector3.Distance(transform.position, players[i].transform.position);
            if (newDistance < distance && newDistance > 5f)
            {
                distance = newDistance;
                target = players[i];
            }
        }
    }

    protected void CaseMethod(bool navAgentEnebled, float xstate, float ysate, int attack, Vector3 destenation)
    {
        if (NavAgent.enabled)
            NavAgent.destination = destenation;
        NavAgent.enabled = navAgentEnebled;
        if (navAgentEnebled)
            NavAgent.destination = destenation;
        _anim.SetInteger("Attack", attack);
        _anim.SetFloat("Xstate", xstate);
        _anim.SetFloat("Ystate", ysate);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (alive)
        {
            Projectile proj;
            if (MyGetComponent(out proj, other.gameObject))
            {
                GetDamage(proj.damage);
            }
        }
    }
}

public static class SceneScript
{
    public static List<Slice_Script_Controller> MiddleSlame;
    public static List<Slice_Script_Controller> JuniorSlame;
}

public enum EnemyState
{
    Stay,
    Attack,
    Walk,
    Spec,
}

public enum SlameType
{
    Senior,
    Middle,
    Junior,
}

/// <summary>
/// Делегат для ивента
/// </summary>
/// <param name="brother">брат</param>
/// <param name="Death">действие</param>
public delegate void BrotherHelper(Slice_Script_Controller brother, int Death);


public class Slice_Script_Controller : Monster //в плеере
{
    [Tooltip("Здесь объект")]
    [Header("Здесь объект")]

    public GameObject unionTarget;

    public float distanceUT;

    public GameObject Eidolon;
    public GameObject god;

    public SlameType slameType;

    public bool boss = false;
    public bool ready = false;
    public bool saled = false;
    public int bossReady = 0;

    private int qtyEidolons = 4;

    public List<Slice_Script_Controller> Brothers;

    public override float DistanceTP
    {
        get { return base.DistanceTP; }

        set
        {
            distanceTP = value;
            if (distanceTP <= RadiusView && distanceTP > attackDistance)
            {
                State = EnemyState.Walk;
                ready = false;
                bossReady = 0;
            }
            else if (distanceTP <= attackDistance)
            {
                State = EnemyState.Attack;
                ready = false;
                bossReady = 0;
            }
            else if (Brothers.Count > qtyEidolons - 2 && slameType != SlameType.Senior)
            {
                if (!boss && unionTarget != null)
                {
                    if (distanceUT > attackDistance)
                        State = EnemyState.Spec;
                    else
                    {
                        State = EnemyState.Stay;
                        if (!ready)
                        {
                            ready = true;
                            BrotherNeed(null, 3);
                        }
                    }
                }
                else
                {
                    if (bossReady > qtyEidolons - 2 && !ready)
                    {
                        Unite();
                        ready = true;
                    }
                }
            }
            else
            {
                ready = false;
                bossReady = 0;
                State = EnemyState.Stay;
            }
        }
    }

    public override EnemyState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
            switch (value)
            {
                case EnemyState.Stay:
                    if (!wait)
                    {
                        wait = true;
                        StartCoroutine(GetRandomStayState());
                    }
                    break;

                case EnemyState.Walk:
                    CaseMethod(true, 0, 1, 0, target.transform.position);
                    break;

                case EnemyState.Attack:
                    GetAttackDistance();
                    CaseMethod(false, 0, 0, attackType, target.transform.position);
                    break;

                case EnemyState.Spec:
                    CaseMethod(true, 0, 1, 0, unionTarget.transform.position);
                    break;
            }
        }
    }

    public event BrotherHelper BrothersNeedChange;

    // Use this for initialization
    public void Start()
    {
        if (slameType == SlameType.Senior)
            Initiolize();
    }

    void FixedUpdate()
    {
        if (alive)
        {
            if (target != null)
            {
                DistanceTP = Vector3.Distance(target.transform.position, transform.position);

                if (!boss && slameType != SlameType.Senior && (State == EnemyState.Stay || State == EnemyState.Spec) && unionTarget != null)
                    distanceUT = Vector3.Distance(unionTarget.transform.position, transform.position);
            }
            else
                FindPlayers();
        }
    }

    private void Update()
    {
        if (boss)
        {
            Debug.DrawRay(this.transform.position, Vector3.up, Color.green, 5);
        }
    }

    private void Unite()
    {
        for (int i = 0; i < qtyEidolons - 1; i++)
        {
            var c = Brothers[0].GetComponent<Slice_Script_Controller>();
            c.saled = false;
            c.Health = 0;
        }

        GameObject game = Instantiate(god, transform.position, Quaternion.identity);
        Slice_Script_Controller SSC = game.GetComponent<Slice_Script_Controller>();
        SSC.Initiolize();
        SSC.target = target;
        SSC.Health = maxHP * 2;
        SSC.AttackForce = AttackForce * 2;
        SSC.RadiusView = RadiusView + 2;
        SSC.NavAgent.speed = NavAgent.speed * size;

        if (slameType == SlameType.Junior)
            SSC.slameType = SlameType.Middle;
        else if (slameType == SlameType.Middle)
            slameType = SlameType.Senior;

        saled = false;
        Health = 0;
    }

    public void BrotherChange(Slice_Script_Controller brother, int death)
    {
        switch (death)
        {
            case 0:
                BrothersNeedChange -= brother.BrotherChange;
                Debug.Log(brother + " BrotherChange Remove" + death +" = ");
                for (int i = 0; i < Brothers.Count; i++)
                {
                    if (Brothers[i] == null)
                    {
                        Brothers.RemoveAt(i);
                        i--;
                    }
                }
                break;
            case 1:
                Brothers.Add(brother);
                Debug.Log(brother + " BrotherChange Add");
                break;
            case 2:
                BrothersNeedChange = null;
                Brothers = new List<Slice_Script_Controller>();
                Debug.Log("Death "+ death + " BrotherChange To Slames");
                break;
            case 3:
                if (boss)
                {
                    bossReady++;
                    Debug.Log("Boss " + bossReady + " BrotherChange");
                }
                break;
            case 4:
                BrothersNeedChange += brother.GetComponent<Slice_Script_Controller>().BrotherChange;
                Debug.Log("new BrotherChange");
                break;
        }

    }

    public void BrotherNeed(Slice_Script_Controller brother, int death)
    {
        if (BrothersNeedChange != null)
            BrothersNeedChange.Invoke(brother, death);
    }

    IEnumerator CreateEidolons()
    {
        yield return new WaitForSeconds(1f);
        GameObject[] eidolons = new GameObject[qtyEidolons];
        for (int i = 0; i < qtyEidolons; i++)
        {
            eidolons[i] = Instantiate(Eidolon, GetRandomPositionForEidolons(), Quaternion.identity);

            Slice_Script_Controller SSC;
            if (MyGetComponent(out SSC, eidolons[i]))
            {
                SSC.Initiolize();
                SSC.target = target;
                SSC.Health = maxHP / 2;
                SSC.AttackForce = AttackForce / 2;
                SSC.RadiusView = RadiusView - 2;
                SSC.NavAgent.speed = NavAgent.speed / size;

                if (slameType == SlameType.Senior)
                    SSC.slameType = SlameType.Middle;
                else if (slameType == SlameType.Middle)
                    slameType = SlameType.Junior;
            }
        }

        for (int i = 0; i < eidolons.Length; i++)
        {
            Slice_Script_Controller SSC = eidolons[i].GetComponent<Slice_Script_Controller>();
            SSC.Brothers = new List<Slice_Script_Controller>();
            SSC.unionTarget = eidolons[0];
            for (int j = 0; j < eidolons.Length; j++)
            {
                Slice_Script_Controller SSC1 = eidolons[j].GetComponent<Slice_Script_Controller>();
                if (i != j)
                {
                    SSC.Brothers.Add(SSC1);
                    SSC1.BrothersNeedChange += BrotherChange;
                }
            }
        }
        eidolons[0].GetComponent<Slice_Script_Controller>().boss = true;
        eidolons[0].name += "boss";
    }

    public override void Death()
    {
        try
        {
            
            if (slameType == SlameType.Middle)
                ReBro(SceneScript.MiddleSlame);
            if (slameType == SlameType.Junior)
                ReBro(SceneScript.JuniorSlame);
        }
        finally
        {

            NavAgent.enabled = false;
            _anim.SetTrigger("Dead");
            if (saled)
            {
                StartCoroutine(Drop());
                StartCoroutine(CreateEidolons());
            }
            StartCoroutine(Destroeded());
            BrotherNeed(this, 0);
        }
    }

    private void ReBro(List<Slice_Script_Controller> gameObjects)
    {
        if (gameObjects == null)
            gameObjects = new List<Slice_Script_Controller>();
        if (gameObjects.Count > 0)
        {
            BrotherNeed(gameObjects[0], 1);
            var ssc = gameObjects[0].GetComponent<Slice_Script_Controller>();
            ssc.Brothers = Brothers;
            BrotherNeed(gameObjects[0], 4);
            ssc.boss = boss;
            if (boss)
                gameObjects[0].name += "boss";
            gameObjects.RemoveAt(0);
        }
        else
        {
            gameObjects.AddRange(Brothers);
            BrotherNeed(null, 2);
        }
    }

    private Vector3 GetRandomPositionForEidolons()
    {
        float x, z;
        x = UnityEngine.Random.Range(-2, 3);
        z = UnityEngine.Random.Range(-2, 3);

        return transform.position + new Vector3(x, 0, z);
    }
}

