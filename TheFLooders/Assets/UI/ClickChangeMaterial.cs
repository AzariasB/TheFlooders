using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickChangeMaterial : MonoBehaviour {

    public Material NwMaterial;

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
            TextBinding.EnableButtons();
            gameObject.GetComponent<Renderer>().material = NwMaterial;
            GameObject.Find("destroy_city").GetComponent<DestroyCity>().EndDestroying(gameObject);
        }
    }
}
