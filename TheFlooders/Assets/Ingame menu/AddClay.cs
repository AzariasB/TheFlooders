﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class pour gérer le click sur le bouton "ajouter de l'argile"
/// </summary>
public class AddClay : MonoBehaviour {

	public Material Clay{ get; set; }
	private Material clay = null;
	[SerializeField]
	private Material _clay = null;

	// Use this for initialization
	void Start () {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(AddClayTask);
	}
	
	// Update is called once per frame
	void Update () {
		if (_clay != clay) {
			Clay = _clay;
		}
		
	}

    void AddClayTask()
    {
        if((GameObject.Find("clay_text").GetComponent<PowerUsesCounter>() as PowerUsesCounter).CanDecrement())
        {
            GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            nwObj.transform.localScale = new Vector3(2, 2, 2);
            nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwObj.AddComponent<PlaceClay>();

            //
            //Material mountainMaterial = Resources.Load("ArgileMaterial", typeof(Material)) as Material;
            //nwObj.GetComponent<Renderer>().material = mountainMaterial;

			nwObj.GetComponent<Renderer>().material = Clay;

            //First destroy collider, to avoid pushing 
            Destroy(nwObj.GetComponent<Collider>());

            PowerUsesCounter.DisableButtons();
			AudioSource asource = GetComponent <AudioSource> ();
			if (asource != null) {
				asource.Play();
			}
        }
    }
}
