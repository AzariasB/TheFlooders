using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PowerUsesCounter.EnableButtons();
            (GameObject.Find("destroy_mountain").GetComponent < RemoveMountain >() as RemoveMountain).EndDeletion();//Notify the button
            Destroy(gameObject);
        }
    }
}
