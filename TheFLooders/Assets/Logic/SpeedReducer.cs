using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedReducer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        collider.gameObject.AddComponent<Drag>();
        (collider.gameObject.GetComponent<Drag>() as Drag).DragDivisor = 0.5f;
    }

    void OnTriggerExit(Collider collider)
    {
        Destroy(collider.gameObject.GetComponent<Drag>());
    }
}
