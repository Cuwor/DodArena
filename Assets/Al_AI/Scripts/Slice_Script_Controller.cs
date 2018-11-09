using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Al_AI.Scripts
{
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
        [HideInInspector]
        public GameObject unionTarget;

        public float distanceUT;

        public GameObject Eidolon;
        public GameObject god;
        public GameObject Player;

        public SlameType slameType;

        public bool boss = false;
        public bool ready = false;
        public bool key;
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
                if (SceneManager.GetActiveScene().buildIndex == WS.buildIndex && MusicManager.musicKey) // WaveMode is on && radio is on
                {

                    if (target != null)
                    {


                        if (Alarm)
                        {
                            FindPlayers();

                            Alarm = false;
                        }
                        else if (distanceTP <= RadiusView && distanceTP > attackDistance)
                        {
                            State = EnemyState.Walk;
                        }
                        else if (distanceTP <= attackDistance)
                        {
                            State = EnemyState.Attack;
                            NavAgent.enabled = false;
                        }
                        else
                        {


                            {
                                if (radio != null)
                                {
                                    target = radio;
                                    State = EnemyState.Walk;
                                }

                            }
                        }
                    }
                    else
                    {

                        FindPlayers();
                    }
                }
                else if (SceneManager.GetActiveScene().buildIndex == TS.buildIndex && MusicManager.musicKey) // TimerMode is on && radio is on
                {
                    if (target != null)
                    {
                        distanceTP = Vector3.Distance(target.transform.position, transform.position);
                        if (distanceTP > attackDistance) // либо идет либо атакует
                        {
                            State = EnemyState.Walk;
                        }
                        else if (distanceTP <= attackDistance)
                        {
                            State = EnemyState.Attack;
                            NavAgent.enabled = false;
                        }
                    }
                    else
                    {
                        FindPlayers();
                    }
                }
                else //radio is off
                {

                    if (!boss && slameType != SlameType.Senior && (State == EnemyState.Stay || State == EnemyState.Spec) && unionTarget != null)
                    {

                        distanceUT = Vector3.Distance(unionTarget.transform.position, transform.position);

                    }

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
                            {
                                State = EnemyState.Spec;
                            }
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
        }

        public override EnemyState State
        {
            get { return state; }

            set
            {
                state = value;
                switch (state)
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
                        PlayThisClip(audioSource, Moves[1]);
                        CaseMethod(true, 0, 1, 0, target.transform.position);
                        break;

                    case EnemyState.Attack:
                        GetAttackDistance();
                        PlayThisClip(audioSource, Moves[0]);
                        CaseMethod(false, 0, 0, attackType, target.transform.position);
                        break;

                    case EnemyState.Spec:
                        PlayThisClip(audioSource, Moves[1]);
                        CaseMethod(true, 0, 1, 0, unionTarget.transform.position);
                        break;
                }
            }
        }

        public event BrotherHelper BrothersNeedChange;

        public override void Initiolize()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            base.Initiolize();
            ready = false;
            bossReady = 0;
        }

        // Use this for initialization
        private void Start()
        {
            if (slameType == SlameType.Senior)
            {
                Initiolize();
            }
        }

        private void FixedUpdate()
        {
            if (alive)
            {
                if (target != null)
                    DistanceTP = Vector3.Distance(target.transform.position, transform.position);
                else
                    FindPlayers();
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
                    SSC.radio = radio;
                    SSC.Initiolize();
                    Ini2(brothers, ref SSC, false);

                    SSC.unionTarget = eidolons[0];

                    if (slameType == SlameType.Senior)
                    {
                        SSC.slameType = SlameType.Middle;
                    }
                    else if (slameType == SlameType.Middle)
                    {
                        SSC.slameType = SlameType.Junior;
                    }
                }
            }

            SubsBrother(brothers);

            eidolons[0].GetComponent<Slice_Script_Controller>().boss = true;
            eidolons[0].name += "boss";
        }
        private Vector3 GetRandomPositionForEidolons()
        {
            float x, z;
            x = UnityEngine.Random.Range(-2, 3);
            z = UnityEngine.Random.Range(-2, 3);

            return transform.position + new Vector3(x, 0, z);
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
                        {
                            SSC.Brothers.Add(SceneScript.MiddleSlame[i]);
                        }

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
                    //Debug.Log("death " + " Brother " + Brothers.Remove(brother));
                    break;
                case 1:
                    Brothers.Add(brother);
                    BrothersNeedChange += brother.GetComponent<Slice_Script_Controller>().BrotherChange;
                    //Debug.Log("Add new Brother");
                    break;
                case 2:
                    BrothersNeedChange = null;
                    Brothers = new List<Slice_Script_Controller>();
                    //Debug.Log("Brothers To Slames");
                    break;
                case 3:
                    //Debug.Log("Boss = " + boss + " ready = " + bossReady);
                    if (boss)
                    {
                        bossReady++;
                    }

                    break;
                case 4:
                    saled = false;
                    Health = 0;
                    //Debug.Log("Unite");
                    break;
                case 5:
                    unionTarget = brother.gameObject;
                    //Debug.Log("unionTargetChange");
                    break;
            }

        }

        public void MeChangedInvoke(Slice_Script_Controller brother, int death)
        {
            if (BrothersNeedChange != null)
            {
                BrothersNeedChange.Invoke(brother, death);
            }
        }

        private void SubsBrother(List<Slice_Script_Controller> Brothers)
        {
            foreach (var vary in Brothers)
            {
                foreach (var vary1 in Brothers)
                {
                    if (vary != vary1)
                    {
                        vary.BrothersNeedChange += vary1.BrotherChange;
                        //Debug.Log("SubsBrother");
                    }
                }
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

        public override void Death()
        {
            try
            {
                //BrotherNeed(this, 0);
                if (Brothers.Count > 0)
                {
                    Brothers.Remove(this);
                }
                else if (slameType == SlameType.Middle)
                {
                    SceneScript.MiddleSlame.Remove(this);
                }
                else if (slameType == SlameType.Junior)
                {
                    SceneScript.JuniorSlame.Remove(this);
                }

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

        private void ReBro(ref List<Slice_Script_Controller> gameObjects)
        {
            if (gameObjects == null)
            {
                gameObjects = new List<Slice_Script_Controller>();
            }

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

        protected override void OnTriggerEnter(Collider other)
        {
            if (alive)
            {
                Projectile proj;
                if (MyGetComponent(out proj, other.gameObject))
                {
                    GetDamage(proj.damage);
                }
            }
            if (key)
            {
                ObstacleScript OS;
                if (MyGetComponent(out OS, other.gameObject))
                {
                    Debug.Log("!");
                    _anim.SetTrigger("Spec");
                    key = false;
                }


            }
            else
            {
                key = true;
            }
        }


    }
}