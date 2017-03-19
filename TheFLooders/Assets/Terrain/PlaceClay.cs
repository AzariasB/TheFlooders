using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceClay : MonoBehaviour {

    public bool Placed { get; set; }

    // Use this for initialization
    void Start()
    {
        Placed = false;
    }

    // Update is called once per frame
    void Update()
    {
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
                    (GameObject.Find("clay_text").GetComponent<TextBinding>() as TextBinding).Decrement();
                    transform.position += new Vector3(0, 0.5f, 0);
                    //Add the collider now, to slow the player down
                    gameObject.AddComponent<BoxCollider>();
                    (gameObject.GetComponent<BoxCollider>() as BoxCollider).isTrigger = true;
                    (gameObject.AddComponent<SpeedReducer>() as SpeedReducer).DragDivisor = 0.1f;
                    TextBinding.EnableButtons();
                }

                //Decrement number of mountains
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //Cancel
                Destroy(gameObject);
                TextBinding.EnableButtons();
            }
            else
            {
                transform.position = hitPoint.point;
            }

        }
    }
}
