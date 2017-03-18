using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class pour gérer le click sur le bouton "ajouter de l'argile"
/// </summary>
public class AddClay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(AddClayTask);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddClayTask()
    {
        if((GameObject.Find("clay_text").GetComponent<TextBinding>() as TextBinding).CanDecrement())
        {
            GameObject nwObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            nwObj.transform.localScale = new Vector3(2, 2, 2);
            nwObj.layer = LayerMask.NameToLayer("Ignore Raycast");
            nwObj.AddComponent<PlaceClay>();

            //First destroy collider, to avoid pushing 
            Destroy(nwObj.GetComponent<Collider>());

            TextBinding.DisableButtons();
        }
    }
}
