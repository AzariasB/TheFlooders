using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyCity : MonoBehaviour {

    private bool _isDestroying;

	// Use this for initialization
	void Start () {
        _isDestroying = false;
        (GetComponent<Button>() as Button).onClick.AddListener(DestroyCityTask);
	}

    public void EndDestroying()
    {

        _isDestroying = false;
        foreach(GameObject city in GameObject.FindGameObjectsWithTag("City"))
        {
            Destroy(city.GetComponent<ClickChangeMaterial>());
        }
        TextBinding.EnableButtons();
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void DestroyCityTask()
    {
        _isDestroying = true;
        Material ruinMaterial = Resources.Load("RuinMaterial", typeof(Material)) as Material;
        TextBinding.DisableButtons();
        foreach(GameObject city in GameObject.FindGameObjectsWithTag("City"))
        {
            city.AddComponent<ClickChangeMaterial>().NwMaterial = ruinMaterial;
        }

    }
}
