using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Al_AI.Scripts
{
	public class Eidolon_Script_Controller : MonoBehaviour {

		public GameObject Player;
		[Space(20)]
		public NavMeshAgent NavAgent;
		public float DistanceTP;
		[Range(1,10)]
		public float RadiusAttack=2;
		[Range(90,100)]
		public float HP =95;
		private Animator _anim;
		// Use this for initialization
		void Start () {
			NavAgent = GetComponent<NavMeshAgent>();
			Player = GameObject.FindWithTag("Player");
			_anim = GetComponent<Animator>();
		}
	
		// Update is called once per frame
		void Update () {
		
			if (HP <= 0)
			{
				_anim.SetTrigger("Dead");
				StartCoroutine(Destroeded());
			

			}
			DistanceTP = Vector3.Distance(Player.transform.position, transform.position);
			//Debug.Log(DistanceTP);
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

		IEnumerator Destroeded()
		{
			yield return new WaitForSeconds(2);
			Destroy(this.gameObject);
		}
	}
}
