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

    public void EndDestroying(GameObject destroyed)
    {
        if(destroyed != null)
        {
            GameObject.Find("cityDestroyText").GetComponent<TextBinding>().Decrement();
        }
        _isDestroying = false;
        foreach(GameObject city in GameObject.FindGameObjectsWithTag("City"))
        {
            if(city == destroyed)
            {//add component 'waterHelper' => water moves faster
                city.tag = "CityDestroyed";
            }
            Destroy(city.GetComponent<ClickChangeMaterial>());
        }
        TextBinding.EnableButtons();
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(1))
        {
            EndDestroying(null);
        }
	}

    public void DestroyCityTask()
    {
        if(GameObject.Find("cityDestroyText").GetComponent<TextBinding>().CanDecrement() )
        {
            _isDestroying = true;
            Material ruinMaterial = Resources.Load("RuinMaterial", typeof(Material)) as Material;
            TextBinding.DisableButtons();
            foreach (GameObject city in GameObject.FindGameObjectsWithTag("City"))
            {
                city.AddComponent<ClickChangeMaterial>().NwMaterial = ruinMaterial;
            }
        }

    }
}
