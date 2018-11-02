using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Al_AI.Scripts
{
    public abstract class Monster : MyTools, IAlive
    {
        [Tooltip("Здесь объект")]
        [Header("Здесь объект")]
        public GameObject target;


        [Space(20)] public NavMeshAgent NavAgent;
        [Range(1, 30)] public float RadiusAttack = 5;
        [Range(1, 100)] public float RadiusView = 15;
        [Range(0, 100)] public float HP = 100;
        [Range(0, 50)] public float AttackForce = 20;
        [HideInInspector] public Animator _anim;

        public GameObject[] AttackAreas;
        public GameObject[] ammos;

        public EnemyState state;
        protected float distanceTP;
        protected bool alive;

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
                if (value <= 0 && alive)
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
            gameObject.AddComponent<Rigidbody>().isKinematic = true;
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
            if (alive)
            {
                Health -= value;
                _anim.SetInteger("Damage", (int)value);
                _anim.SetTrigger("GetDamage");
            }
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
            {
                Debug.Log(destenation);
                NavAgent.destination = destenation;
            }

            NavAgent.enabled = navAgentEnebled;
            if (navAgentEnebled)
            {
                NavAgent.destination = destenation;
            }

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

        public void OnParticleCollision(GameObject other)
        {
            //Debug.Log("OnParticleCollision");
            DamageScript ds;
            if (MyGetComponent(out ds, other))
            {
                GetDamage(ds.Damage);
            }
        }
    }
}
