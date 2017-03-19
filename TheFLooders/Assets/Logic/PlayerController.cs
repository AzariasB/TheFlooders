using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Rigidbody rB = gameObject.GetComponent<Rigidbody>();
        rB.velocity = new Vector3(Input.GetAxis("Horizontal") * 3 , 0, Input.GetAxis("Vertical") * 3);
	}
}
