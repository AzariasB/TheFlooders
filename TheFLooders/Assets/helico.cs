using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helico : MonoBehaviour {
	private bool ismoving;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!ismoving) {
			LevelInfo info = LevelInfo.Instance;
			if (info.iscompleted) {
				ismoving = true;
				GetComponent <AudioSource>().Play ();
			}
		}
		else{transform.Translate(new Vector3(1,0,0)*5*Time.deltaTime);
		}
	}
}
