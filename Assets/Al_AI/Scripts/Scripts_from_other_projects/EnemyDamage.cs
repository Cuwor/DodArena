using UnityEngine;

namespace Al_AI.Scripts.Scripts_from_other_projects
{
    public class EnemyDamage : MonoBehaviour {

        public float Health;
        public GameObject MainObj;
        public float DeadTime;

        private Animator _anim;

        public void Start()
        {
            Health = 100;
            _anim = MainObj.GetComponent<Animator>();
        }

        public void GetDamage(float damage)
        {
            Health -= damage;
            if(Health <= 0)
            {
                _anim.SetBool("Attack", false);
                _anim.SetBool("Walk", false);
                _anim.SetTrigger("TheEnd");
                Invoke("Death", DeadTime);
            }
        }

        private void Death()
        {
            //  GameObject.Find("RelictusWithGun").GetComponent<NewRelictusController>().ShootSound -= gameObject.GetComponent<DemonController>().ListenShoot;
            Destroy(MainObj);
        }

    }
}
