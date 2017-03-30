using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddBridge : MonoBehaviour {


    private Vector3 bridgeSize = new Vector3((float)0.5, 1, 4);

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
        if((GameObject.Find("bridge_text").GetComponent<PowerUsesCounter>() as PowerUsesCounter).CanDecrement())
        {
            GameObject nwBrige = GameObject.CreatePrimitive(PrimitiveType.Plane);

            //Remove collision
            Destroy(nwBrige.GetComponent<Collider>());

            //Set material
            Material bridgeMat = Resources.Load("Pont", typeof(Material)) as Material;
            nwBrige.GetComponent<Renderer>().material = bridgeMat;

            //Change size ...
            nwBrige.transform.localScale = bridgeSize;
            

            nwBrige.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwBrige.AddComponent<PlaceBridge>();

            PowerUsesCounter.DisableButtons();
			AudioSource asource = GetComponent <AudioSource> ();
			if (asource != null) {
				asource.Play();
			}
        }
    }
}
