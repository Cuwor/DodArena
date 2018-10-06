using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReactor : MonoBehaviour {

    private Rigidbody _rb;

	// Use this for initialization
	void Start () {
        _rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)//sda
    {
        if(other.tag.Equals("Force"))
        {
            Vector3 forceVector;
//            if (other.GetComponent<ForceScript>().Type == ForceType.impulse)
//            {
//                forceVector = other.transform.up;
//            }
//            else
//            {
//                forceVector = -other.transform.up;
//            }
           // _rb.AddForce(forceVector.normalized * 100,ForceMode.Impulse);
        }
    }
}
