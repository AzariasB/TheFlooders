using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TextBinding.EnableButtons();
            (GameObject.Find("destroy_mountain").GetComponent < RemoveMountain >() as RemoveMountain).EndDeletion();//Notify the button

            // Lower terrain
            LevelInfo info =LevelInfo.Instance;
            if (info != null && info.HeightMap != null) {
                info.HeightMap.CreateMountaintOrHole(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), 30, -35);
            } else {
                Debug.LogWarning ("Le terrain n'a pas été trouvé");
            }

            Destroy(gameObject);
        }
    }
}
