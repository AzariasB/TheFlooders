using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBinding : MonoBehaviour {

    public int Count;

    private Text _textObject;

    public void BindText()
    {
        _textObject.text = Count.ToString();
    }

    public void Decrement()
    {
        Count--;
        BindText();
    }

    public bool CanDecrement()
    {
        return Count > 0;
    }

	// Use this for initialization
	public void Start () {
        _textObject = gameObject.GetComponent<Text>();
        BindText();
	}
	

	// Update is called once per frame
	void Update () {
		
	}
}
