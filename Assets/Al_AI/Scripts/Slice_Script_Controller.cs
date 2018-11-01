using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public abstract class Monster : MyTools, IAlive
{
    [Tooltip("Здесь объект")]
    [Header("Здесь объект")]
    public GameObject target;


    [Space(20)] public NavMeshAgent NavAgent;
    [Range(1, 30)] public float RadiusAttack;
    [Range(1, 100)] public float RadiusView;
    [Range(0, 100)] public float HP;
    [Range(0, 50)] public float AttackForce;

    public GameObject[] AttackAreas;
    public GameObject[] ammos;

    public EnemyState state;
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

    public virtual void Initiolize()
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

    protected IEnumerator Destroeded()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    protected IEnumerator GetRandomStayState()
    {
        //_anim.SetFloat("Ystate", -1);
        //_anim.SetFloat("Xstate", UnityEngine.Random.Range(-1, 1.1f));
        //FindPlayers();
        yield return new WaitForSeconds(2);
        wait = false;
    }

    protected IEnumerator Drop()
    {
        yield return new WaitForSeconds(2);
        int x = UnityEngine.Random.Range(0, ammos.Length);
        Instantiate(ammos[x], transform.position, new Quaternion());
    }

    public virtual void GetDamage(float value)
    {
        Health -= value;
        _anim.SetInteger("Damage", (int)value);
        _anim.SetTrigger("GetDamage");
    }

    public void PlusHealth(float value)
    {
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

public class Slice_Script_Controller : Monster 
{
    public GameObject unionTarget;
    [HideInInspector]
    public float distanceUT;

    public GameObject Eidolon;
    public GameObject god;

    public SlameType slameType;

    public bool boss = false;
    public bool ready = false;
    [HideInInspector]
    public bool saled = false;
    public int bossReady = 0;

    private const int qtyEidolons = 4;

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
                    if (distanceUT > attackDistance + 2)
                        State = EnemyState.Spec;
                    else
                    {
                        State = EnemyState.Stay;
                        if (!ready)
                        {
                            ready = true;
                            MeChangedInvoke(null, 3);
                        }
                    }
                }
                if (boss)
                {
                    State = EnemyState.Stay;
                    if (bossReady > qtyEidolons - 2 && !ready)
                    {
                        StartCoroutine(Unite());
                        ready = true;
                    }

                }
            }
            else
            {
                State = EnemyState.Stay;
            }
        }
    }
    public override EnemyState State
    {
        get { return state; }

        set
        {
            switch (value)
            {
                case EnemyState.Stay:
                    if (!wait)
                    {
                        wait = true;
                        CaseMethod(false, UnityEngine.Random.Range(-1, 1.1f), -1, 0, target.transform.position);
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
            state = value;
        }
    }

    public event BrotherHelper BrothersNeedChange;

    public override void Initiolize()
    {
        base.Initiolize();
        ready = false;
        bossReady = 0;
    }

    public override void Death()
    {
        try
        {
            //BrotherNeed(this, 0);
            if (Brothers.Count > 0)
                Brothers.Remove(this);
            switch (slameType)
            {
                case SlameType.Middle:
                    ReBro(ref SceneScript.MiddleSlame);
                    break;
                case SlameType.Junior:
                    ReBro(ref SceneScript.JuniorSlame);
                    break;
            }
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
        }
    }

    // Use this for initialization
    private void Start()
    {
        if (slameType == SlameType.Senior)
            Initiolize();
    }

    private void FixedUpdate()
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

    private IEnumerator CreateEidolons()
    {
        yield return new WaitForSeconds(1f);
        GameObject[] eidolons = new GameObject[qtyEidolons];
        List<Slice_Script_Controller> brothers = new List<Slice_Script_Controller>();
        for (int i = 0; i < qtyEidolons; i++)
        {
            eidolons[i] = Instantiate(Eidolon, GetRandomPositionForEidolons(), Quaternion.identity);

            Slice_Script_Controller SSC;
            if (MyGetComponent(out SSC, eidolons[i]))
            {
                brothers.Add(SSC);

                SSC.Initiolize();
                Ini2(brothers, ref SSC, false);

                SSC.unionTarget = eidolons[0];

                if (slameType == SlameType.Senior)
                    SSC.slameType = SlameType.Middle;
                else if (slameType == SlameType.Middle)
                    slameType = SlameType.Junior;
            }
        }

        SubsBrother(brothers);

        eidolons[0].GetComponent<Slice_Script_Controller>().boss = true;
        eidolons[0].name += "boss";
    }

    private IEnumerator Unite()
    {
        yield return new WaitForSeconds(1f);

        MeChangedInvoke(null, 4);

        GameObject game = Instantiate(god, transform.position, Quaternion.identity);
        Slice_Script_Controller SSC = game.GetComponent<Slice_Script_Controller>();
        SSC.Initiolize();
        Ini2(new List<Slice_Script_Controller>(), ref SSC, true);

        switch (slameType)
        {
            case SlameType.Junior:
                SSC.slameType = SlameType.Middle;
                SceneScript.MiddleSlame.Add(SSC);
                if (SceneScript.MiddleSlame.Count >= qtyEidolons)
                {
                    var sscc = SceneScript.MiddleSlame[0];
                    sscc.boss = true;
                    sscc.gameObject.name += "boss";

                    for (int i = 0; i < 4; i++)
                        SSC.Brothers.Add(SceneScript.MiddleSlame[i]);

                    SceneScript.MiddleSlame.RemoveRange(0, qtyEidolons);
                    SubsBrother(Brothers);
                }
                break;
            case SlameType.Middle:
                SSC.slameType = SlameType.Senior;
                break;
        }

        saled = false;
        Health = 0;
    }

    public void BrotherChange(Slice_Script_Controller brother, int death)
    {
        switch (death)
        {
            case 0:
                BrothersNeedChange -= brother.BrotherChange;
                Debug.Log("death " + " Brother " + Brothers.Remove(brother));
                break;
            case 1:
                Brothers.Add(brother);
                BrothersNeedChange += brother.GetComponent<Slice_Script_Controller>().BrotherChange;
                Debug.Log("Add new Brother");
                break;
            case 2:
                BrothersNeedChange = null;
                Brothers = new List<Slice_Script_Controller>();
                Debug.Log("Brothers To Slames");
                break;
            case 3:
                Debug.Log("Boss = " + boss + " ready = " + bossReady);
                if (boss)
                    bossReady++;
                break;
            case 4:
                saled = false;
                Health = 0;
                Debug.Log("Unite");
                break;
            case 5:
                unionTarget = brother.gameObject;
                Debug.Log("unionTargetChange");
                break;
        }

    }

    public void MeChangedInvoke(Slice_Script_Controller brother, int death)
    {
        if (BrothersNeedChange != null)
            BrothersNeedChange.Invoke(brother, death);
    }

    private void SubsBrother(List<Slice_Script_Controller> Brothers)
    {
        foreach (var vary in Brothers)
            foreach (var vary1 in Brothers)
                if (vary != vary1)
                {
                    vary.BrothersNeedChange += vary1.BrotherChange;
                    Debug.Log("SubsBrother");
                }
    }

    private void Ini2(List<Slice_Script_Controller> brothers, ref Slice_Script_Controller SSC, bool up)
    {
        SSC.target = target;
        SSC.Health = (up ? maxHP * 2 : maxHP / 2);
        SSC.AttackForce = (up ? AttackForce * 2 : AttackForce / 2);
        SSC.RadiusView = (up ? RadiusView + 2 : RadiusView - 2);
        SSC.NavAgent.speed = (up ? NavAgent.speed * SSC.size : NavAgent.speed / SSC.size);
        SSC.Brothers = brothers;
    }

    private void ReBro(ref List<Slice_Script_Controller> gameObjects)
    {
        if (gameObjects == null)
            gameObjects = new List<Slice_Script_Controller>();
        if (gameObjects.Count > 0)
        {
            Brothers.Add(gameObjects[0]);
            gameObjects[0].Brothers = Brothers;
            gameObjects[0].boss = boss;
            gameObjects[0].BrothersNeedChange = BrothersNeedChange;
            if (boss)
            {
                gameObjects[0].name += "boss";
                MeChangedInvoke(gameObjects[0], 5);
            }
            else
            {
                gameObjects[0].unionTarget = unionTarget;
            }
            gameObjects.RemoveAt(0);

        }
        else
        {
            gameObjects.AddRange(Brothers);
            MeChangedInvoke(null, 2);
            //Debug.Log(gameObjects[0].slameType + " " + gameObjects.Count);
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

