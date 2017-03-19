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
        if ((GameObject.Find("mountain_text").GetComponent<TextBinding>() as TextBinding).CanDecrement())
        {
            GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Destroy(nwObj.GetComponent<Collider>());
            nwObj.gameObject.tag = "Mountain";

            //Not perturbing raycast
            nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");

            //Rotate (otherwise, it's upside-down)
            nwObj.transform.Rotate(new Vector3(0, 180, 0));

            Material mountainMaterial = Resources.Load("Mountain_Material", typeof(Material)) as Material;
            nwObj.GetComponent<Renderer>().material = mountainMaterial;

            nwObj.AddComponent<PlaceMountain>();

            TextBinding.DisableButtons();
			AudioSource asource = GetComponent <AudioSource> ();
			if (asource != null) {
				asource.Play();
			}

        }
    }

}
