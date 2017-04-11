using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveMountain : MonoBehaviour {

    private bool _isDeleting;
    

	// Use this for initialization
	void Start () {
        _isDeleting = false;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(RemoveMountainTask);
	}
	
    public void EndDeletion()
    {
        _isDeleting = false;
    }

	// Update is called once per frame
	void Update () {
        if(_isDeleting && Input.GetMouseButtonDown(1))
        {
            EndDeletion();
            foreach(GameObject mountain in GameObject.FindGameObjectsWithTag("Mountain"))
            {
                Destroy(mountain.GetComponent<ClickDestroy>());
            }
        }
	}

    void RemoveMountainTask()
    {
        GameObject[] mountains = GameObject.FindGameObjectsWithTag("Mountain");
        if(mountains.Length > 0)
        {
            PowerUsesCounter.DisableButtons();
            foreach(GameObject mountain in mountains)
            {
                mountain.AddComponent<ClickDestroy>();
            }
            _isDeleting = true;
        }
    }
}
