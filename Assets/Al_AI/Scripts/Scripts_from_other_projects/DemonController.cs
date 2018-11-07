using UnityEngine;
using UnityEngine.AI;

namespace Al_AI.Scripts.Scripts_from_other_projects
{
    public class DemonController : MonoBehaviour {

        public GameObject Player;
        public float DemonDistance;
        public float DemonAngel;
        public float StunTime;
        public bool Search;
        public bool Alarm;

        private Animator _anim;
        private NavMeshAgent _NMA;
        // private NewRelictusController _NRC;
        private Vector3 _searchPos;
        private float _time;
        private bool stun;


        void Start () {
            Alarm = false;
            Search = false;
            _anim = GetComponent<Animator>();
            _NMA = GetComponent<NavMeshAgent>();
            //      _NRC = Player.GetComponent<NewRelictusController>();
            //     _NRC.ShootSound += ListenShoot;
        }
	
	
        void Update () {

            if(stun)
            {
                Timer();
                _NMA.destination = transform.position;
            }
            else
            {
                float f = DistanceToPlayer();

                if (Search && !stun)
                {
                    MoveToTarget(_searchPos);
                }
                if (Alarm && !stun)
                {
                    MoveToTarget(Player.transform.position);
                    if (f <= DemonDistance)
                    {
                        Alarm = false;
                    }
                }
                else if (f > DemonDistance && !Search)
                {
                    DefaultState();
                }
                else if (f <= DemonDistance && f > 5f)
                {
                    Search = false;
                    // if (ISeeYou() || _NRC.Fast)
                    // {
                    //      MoveToTarget(Player.transform.position);
                    //  }
                }
                else if (f <= 5 && f > 3.5)
                {
                    Search = false;
                    MoveToTarget(Player.transform.position);
                }
                else if (f <= 3.5f)
                {
                    Search = false;
                    Attack();
                }
            }
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, Player.transform.position);
        }

        private void DefaultState()
        {
            _anim.SetBool("Walk", false);
        }

        private bool ISeeYou()
        {
            Quaternion look = Quaternion.LookRotation(Player.transform.position - transform.position);
            float angel = Quaternion.Angle(transform.rotation, look);
            if(angel < DemonAngel)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.position, Player.transform.position - transform.position);
                if (Physics.Raycast(ray, out hit, DemonDistance))
                {
                    if(hit.transform.gameObject == Player)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void MoveToTarget(Vector3 pos)
        {
            Vector3 targer = pos;
            _anim.SetBool("Walk", true);
            _NMA.destination = targer;
            _anim.SetBool("Attack", false);
        }

        private void Attack()
        {
            transform.LookAt(Player.transform.position);
            _anim.SetBool("Attack", true);
            _anim.SetBool("Walk", true);
        }

        private void StopAttack()
        {
            _anim.SetBool("Attack", false);
        }

        private void GetAlarm()
        {
            Alarm = true;
        }

        public void ListenShoot(Vector3 pos)
        {
            if(DistanceToPlayer() <= 30)
            {
                Search = true;
                _searchPos = pos;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag.Equals("Force"))
            {
                Invoke("GetAlarm", 1f);
                _anim.SetInteger("Damage", -1);
                _anim.SetTrigger("GetDamage");
                StartStun(0.7f);
            }
        }

        public void StartStun(float stunTime)
        {
            _time = 0;
            StunTime = stunTime;
            stun = true;
        }

        private void Timer()
        {
            if(_time + Time.deltaTime <= StunTime)
            {
                _time += Time.deltaTime;
                stun = true;
            }
            else
            {
                _time = StunTime;
                stun = false;
            }
        }
    }
}
