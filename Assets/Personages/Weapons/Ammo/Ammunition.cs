using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour {

    public byte count;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.up, 2*Time.deltaTime);
	}


}
