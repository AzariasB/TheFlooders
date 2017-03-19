using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quand est ajouté à un GameObject, va suivre
/// la souris, jusqu'au clic de celle-ci, quan cliqué,
/// va poser l'objet en question
/// </summary>
public class PlaceMountain : MonoBehaviour {

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
                    // Raise terrain
                    LevelInfo info =LevelInfo.Instance;
                    if (info != null && info.HeightMap != null) {
                        info.HeightMap.CreateMountaintOrHole(new Vector2(hitPoint.point.x, hitPoint.point.z), 30, 35);
                    } else {
                        Debug.LogWarning ("Le terrain n'a pas été trouvé");
                    }

                    //Change layer
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    gameObject.transform.position += new Vector3(0, 0.5f, 0);
                    (gameObject.AddComponent<BoxCollider>() as BoxCollider).isTrigger = true;
                    (gameObject.AddComponent<SpeedReducer>() as SpeedReducer).DragDivisor = 5;
                    (GameObject.Find("mountain_text").GetComponent<TextBinding>() as TextBinding).Decrement();
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
