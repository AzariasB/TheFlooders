﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ajouter du limon sur la map
/// </summary>
public class AddSilt : MonoBehaviour {

	public Material Silt{ get; set; }
	private Material silt = null;
	[SerializeField]
	private Material _silt = null;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(AddSiltTask);
	}
	
	// Update is called once per frame
	void Update () {
		if (_silt != silt) {
			Silt = _silt;
		}
		
	}

    void AddSiltTask()
    {
        if ((GameObject.Find("siltText").GetComponent<PowerUsesCounter>() as PowerUsesCounter).CanDecrement())
        {
            GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            nwObj.transform.localScale = new Vector3(2, 2, 2);
            nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwObj.AddComponent<PlaceSilt>();
            nwObj.tag = "Silt";

            //
            //Material mountainMaterial = Resources.Load("SiltMaterial", typeof(Material)) as Material;
            //nwObj.GetComponent<Renderer>().material = mountainMaterial;

			nwObj.GetComponent<Renderer>().material = Silt;

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
