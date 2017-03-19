using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            rayCast = Physics.Raycast(ray, out hitPoint, 10000.0f);
            float rotation = Input.GetAxis("Mouse ScrollWheel");
            gameObject.transform.Rotate(Vector3.up * rotation * 8192 * Time.deltaTime);
            if (Input.GetMouseButtonDown(0))
            {
                Placed = rayCast;
                if (Placed)
                {
                    //Change layer
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    (GameObject.Find("bridge_text").GetComponent<TextBinding>() as TextBinding).Decrement();
                    TextBinding.EnableButtons();

                    //Update Graph
                    Vector3 size = GetComponent<Renderer>().bounds.size;
                    Vector3 mPos = transform.position;
                    Vector3 extremity1 = new Vector3(mPos.x + size.x/2, 0, mPos.z + size.z/2);
                    Vector3 extremity2 = new Vector3(mPos.x - size.x / 2, 0, mPos.z - size.z / 2);

                    TileMap tm = GameObject.Find("TileMap").GetComponent<TileMap>();
                    GraphNode g1 = tm.GetNodeAtCoords(extremity1.x, extremity1.z);
                    GraphNode g2 = tm.GetNodeAtCoords(extremity2.x, extremity2.z);
                    if(g1 != null && g2 != null && !g1.Sinked && !g2.Sinked)
                    {
                        Edge bridgeEdge = new Edge(g1, g2);
                    }else
                    {

                       // g1.DebugTrace();
                        //g2.DebugTrace();
                        Debug.LogError("Failed to create bridge edge");
                    }
                        
                    //Tick AI
                    foreach(AI mAI in GameObject.FindObjectsOfType<AI>())
                    {
                        print("Adding edge");
                        mAI.Recaculate();
                    }
                }

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
