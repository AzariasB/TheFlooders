using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Place silt
/// </summary>
public class PlaceSilt : MonoBehaviour {


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
            rayCast = Physics.Raycast(ray, out hitPoint, 10000.0f);
            if (Input.GetMouseButtonDown(0))
            {
                Placed = rayCast;
                if (Placed)
                {
                    //Change layer
                    (GameObject.Find("siltText").GetComponent<PowerUsesCounter>() as PowerUsesCounter).Decrement();
                    transform.position += new Vector3(0, 0.5f, 0);


                    PowerUsesCounter.EnableButtons();
                }

                //Decrement number of mountains
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //Cancel
                Destroy(gameObject);
                PowerUsesCounter.EnableButtons();
            }
            else
            {
                transform.position = hitPoint.point;
            }

        }
    }
}
