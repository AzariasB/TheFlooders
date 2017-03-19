using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMovie : MonoBehaviour {

	// Use this for initialization
	void Start () {
		((MovieTexture)GetComponent<MeshRenderer> ().material.mainTexture).Play ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
