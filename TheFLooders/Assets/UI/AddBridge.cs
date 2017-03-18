using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddBridge : MonoBehaviour {


	// Use this for initialization
	void Start () {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(AddBrigeTask);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddBrigeTask()
    {
        //Check if remain enough bridges
        if((GameObject.Find("bridge_text").GetComponent<TextBinding>() as TextBinding).CanDecrement())
        {
            GameObject nwBrige = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //Change size ...

            nwBrige.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwBrige.AddComponent<PlaceBridge>();
        }
    }
}
