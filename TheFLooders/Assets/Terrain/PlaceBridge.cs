using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBridge : MonoBehaviour {

    bool Placed { get; set; }

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
            float rotation = Input.GetAxis("Mouse ScrollWheel");
            gameObject.transform.Rotate(Vector3.up * rotation * 8192 * Time.deltaTime);
            if (Input.GetMouseButtonDown(0))
            {
                Placed = rayCast;
                //Change layer
                gameObject.layer = LayerMask.NameToLayer("Default");
                (GameObject.Find("bridge_text").GetComponent<BridgeCount>() as BridgeCount).PlaceBridge();
                
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //Cancel
                Destroy(gameObject);
            }
            else
            {
                transform.position = (hitPoint.point + new Vector3(0 , 1, 0) );
            }
        }
    }
}
