using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBridge : MonoBehaviour {

    bool Placed { get; set; }
    bool Rotated { get; set; }

	// Use this for initialization
	void Start () {
        Placed = false;
        Rotated = false;
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
            }
            else
            {
                transform.position = (hitPoint.point + new Vector3(0 , 1, 0) );
            }

        }else if(!Rotated)
        {
            float rotation = Input.GetAxis("Mouse ScrollWheel");
            gameObject.transform.Rotate(Vector3.up * rotation * 8192 * Time.deltaTime );

            if (Input.GetMouseButtonDown(0))
            {
                Rotated = true;
            }

        }
    }
}
