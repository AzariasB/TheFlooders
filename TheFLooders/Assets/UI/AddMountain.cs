using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddMountain : MonoBehaviour {


    Button _button;
	// Use this for initialization
	void Start () {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(AddMountainTask);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddMountainTask()
    {
        if( (GameObject.Find("mountain_text").GetComponent<TextBinding>() as TextBinding).CanDecrement())
        {
            GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nwObj.transform.position = new Vector3(0, 10, 0);
            //nwObj.AddComponent<Rigidbody>();
            nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwObj.AddComponent<PlaceMountain>();
        }
    }
}
