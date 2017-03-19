using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ajouter du limon sur la map
/// </summary>
public class AddSilt : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(AddSiltTask);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddSiltTask()
    {
        if ((GameObject.Find("siltText").GetComponent<TextBinding>() as TextBinding).CanDecrement())
        {
            GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            nwObj.transform.localScale = new Vector3(2, 2, 2);
            nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwObj.AddComponent<PlaceSilt>();
            nwObj.tag = "Silt";

            //
            Material mountainMaterial = Resources.Load("SiltMaterial", typeof(Material)) as Material;
            nwObj.GetComponent<Renderer>().material = mountainMaterial;

            //First destroy collider, to avoid pushing 
            Destroy(nwObj.GetComponent<Collider>());

            TextBinding.DisableButtons();
        }
    }
}
