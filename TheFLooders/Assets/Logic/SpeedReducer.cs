using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedReducer : MonoBehaviour {

    public float DragDivisor;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        (collider.gameObject.AddComponent<Drag>() as Drag).DragDivisor = DragDivisor;
    }

    void OnTriggerExit(Collider collider)
    {
        Destroy(collider.gameObject.GetComponent<Drag>());
    }
}
