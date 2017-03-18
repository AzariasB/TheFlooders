using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBinding : MonoBehaviour {

    public int Count { get; set; }

    private string _textContent;

    private Text _textObject;

    public TextBinding(int Count, string textContent)
    {
        this.Count = Count;
        this._textContent = textContent;
    }

    public void BindText()
    {
        _textObject.text = string.Format(_textContent, Count);
    }

    protected void ReduceCount()
    {
        Count--;
        BindText();
    }

	// Use this for initialization
	void Start () {
        _textObject = gameObject.GetComponent<Text>();
        BindText();
	}
	

	// Update is called once per frame
	void Update () {
		
	}
}
