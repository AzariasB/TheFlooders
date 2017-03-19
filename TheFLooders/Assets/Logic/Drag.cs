using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ajouté au joueur quand il passe dans une zone qui lui donne un boost/inconvénient de speed
/// </summary>
public class Drag : MonoBehaviour {

    public float DragDivisor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        (gameObject.GetComponent < Rigidbody >() as Rigidbody).velocity /= DragDivisor;
	}
}
