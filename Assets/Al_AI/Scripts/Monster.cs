using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace Al_AI.Scripts
{
    public abstract class Monster : MyTools, IAlive
    {
        [Tooltip("Здесь объект")]
        [Header("Здесь объект")]
        public GameObject target;

        [Header("Конфеты")]
        public GameObject[] Candies;
        [Header("Бонусы")]
        public GameObject[] Bonuses;

        public Scene WS; 
        public Scene TS;
        public GameObject radio;
        public bool Alarm;

        public AudioClip[] Moves;
        public AudioSource audioSource;

        [Space(20)] public NavMeshAgent NavAgent;
        [Range(1, 30)] public float RadiusAttack = 5;
        [Range(1, 100)] public float RadiusView = 15;
        [Range(0, 200)] public float HP;
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
                if (SceneManager.GetActiveScene().buildIndex == WS.buildIndex && MusicManager.musicKey)// WaveMode is on && radio is on
                {
                    
                    if (target != null)
                    {
                       
                        
                        if (Alarm)
                        {
                            FindPlayers();
                            Debug.Log(target.name);
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
                            
                          
                                if (radio != null)
                                {
                                    target = radio;
                                    State = EnemyState.Walk;
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
                        if (distanceTP > attackDistance)
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
                else
                {
                    if (target != null)
                    {
                        distanceTP = Vector3.Distance(target.transform.position, transform.position);
                        if (distanceTP <= RadiusView && distanceTP > attackDistance)
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
                            State = EnemyState.Stay;
                        }
                    }
                    else
                    {
                        FindPlayers();
                    }
                }
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
                switch (State)
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
                }
            }
        }

        #region Логика

        public virtual void Initiolize()
        {
            radio = GameObject.FindGameObjectWithTag("Radio");
            WS = SceneManager.GetSceneByBuildIndex(3);
            TS = SceneManager.GetSceneByBuildIndex(4);
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
            StopAttack();
            State = EnemyState.Stay;
            GetAttackDistance();
            size = transform.localScale.x;
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
        
        protected void StartMove()
        {
            if(Health>0)
            {
                alive = true;
                NavAgent.enabled = true;
            }
        }


        #endregion

        #region Атака

        protected virtual void GetAttackDistance()
        {
            attackType = UnityEngine.Random.Range(1, 3);
            attackDistance = attackType == 1 ? RadiusAttack : RadiusAttack - 2 * size;
        }
        private void StartAttack(int trigger)
        {
            AttackAreas[trigger].SetActive(true);
        }

        protected void StopAttack()
        {
            foreach (var trigger in AttackAreas)
            {
                trigger.SetActive(false);
            }
        }

        #endregion

        #region Здоровье и смерть

        public void PlusHealth(float value)
        {
        }
        public virtual void GetDamage(float value)
        {
            if (alive)
            {
                Alarm = true;
                NavAgent.enabled = false;
                Health -= value;
                _anim.SetInteger("Damage", (int)value);
                _anim.SetTrigger("GetDamage");
            }
        }
        public virtual void Death()
        {
            alive = false;
            NavAgent.enabled = false;
            _anim.SetTrigger("Dead");
            StartCoroutine(Drop());
            StartCoroutine(Destroeded());
        }
        protected virtual IEnumerator Destroeded()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        #endregion

        #region Дополнительное

        protected IEnumerator GetRandomStayState()
        {
            _anim.SetFloat("Ystate", -1);
            _anim.SetFloat("Xstate", UnityEngine.Random.Range(-1, 1.1f));
            //FindPlayers();
            yield return new WaitForSeconds(2);
            wait = false;
        }
        protected IEnumerator Drop()
        {
            yield return new WaitForSeconds(2);
            int x = UnityEngine.Random.Range(0, ammos.Length);
            Instantiate(ammos[x], transform.position + Vector3.up*3, new Quaternion());
            x = UnityEngine.Random.Range(0, Bonuses.Length);
            if(Bonuses[x] != null)
            {
                Instantiate(Bonuses[x], transform.position + Vector3.up * 3, new Quaternion());
            }
            foreach (var c in Candies)
            {
                Instantiate(c, randomDropPosition(), new Quaternion());
            }
        }

        private Vector3 randomDropPosition()
        {
            return new Vector3(transform.position.x + Random.Range(-5, 6), transform.position.y + 1, transform.position.z + Random.Range(-5, 6));
        }

        protected virtual void OnTriggerEnter(Collider other)
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
        #endregion

    }
}
