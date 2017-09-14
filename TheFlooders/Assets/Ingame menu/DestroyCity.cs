using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyCity : MonoBehaviour {
	public AudioSource selectionSound;
	public AudioSource destructionSound;

//    private bool _isDestroying;
	public Material city_destroyed;

	// Use this for initialization
	void Start () {
//        _isDestroying = false;
        (GetComponent<Button>() as Button).onClick.AddListener(DestroyCityTask);
	}

    public void EndDestroying(GameObject destroyed)
    {
        if(destroyed != null)
		{   if (destructionSound != null) {
				destructionSound.Play();
			}
            GameObject.Find("cityDestroyText").GetComponent<PowerUsesCounter>().Decrement();
        }
//        _isDestroying = false;
        foreach(GameObject city in GameObject.FindGameObjectsWithTag("City"))
        {
            if(city == destroyed)
            {//add component 'waterHelper' => water moves faster
                city.tag = "CityDestroyed";
            }
            Destroy(city.GetComponent<ClickChangeMaterial>());
        }
        PowerUsesCounter.EnableButtons();

	
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
        if(GameObject.Find("cityDestroyText").GetComponent<PowerUsesCounter>().CanDecrement() )
        {
//            _isDestroying = true;
            //Material ruinMaterial = Resources.Load("RuinMaterial", typeof(Material)) as Material;
            PowerUsesCounter.DisableButtons();
			if (selectionSound != null) {
				selectionSound.Play();
			}
            foreach (GameObject city in GameObject.FindGameObjectsWithTag("City"))
            {
				city.AddComponent<ClickChangeMaterial>().NwMaterial = city_destroyed;
            }
        }

    }
}
