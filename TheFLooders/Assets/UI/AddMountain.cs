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
        if((GetComponent<Button>() as Button).IsInteractable())
        {
            if ((GameObject.Find("mountain_text").GetComponent<TextBinding>() as TextBinding).CanDecrement())
            {
                GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                nwObj.transform.position = new Vector3(0, 10, 0);
                nwObj.gameObject.tag = "Mountain";
                nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");
                nwObj.AddComponent<PlaceMountain>();

                GameObject[] objects = GameObject.FindGameObjectsWithTag("ModifierButton");
                foreach (GameObject obj in objects)
                {
                    (obj.GetComponent<Button>() as Button).interactable = false;
                }

                foreach (GameObject obj in objects)
                {
                    print((obj.GetComponent<Button>() as Button).interactable);
                }
            }
        }
    }
}
