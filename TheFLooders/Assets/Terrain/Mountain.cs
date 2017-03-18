using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quand est ajouté à un GameObject, va suivre
/// la souris, jusqu'au clic de celle-ci, quan cliqué,
/// va poser l'objet en question
/// </summary>
public class Mountain : MonoBehaviour {

    public bool Placed { get; set; }

	// Use this for initialization
	void Start () {
        Placed = false;
	}
	
	// Update is called once per frame
	void Update () {


        if (!Placed)
        {
            bool rayCast;
            RaycastHit hitPoint;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            rayCast = Physics.Raycast(ray, out hitPoint, 100.0f);
            if (Input.GetMouseButtonDown(0))
            {
                Placed = rayCast;
                //Change layer
                gameObject.layer = LayerMask.NameToLayer("Default");
            }else
            {
                transform.position = hitPoint.point;
            }

        }
    }
}
