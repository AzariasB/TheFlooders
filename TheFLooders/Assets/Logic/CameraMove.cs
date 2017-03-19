using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    private bool _started = false;

    public float TimeStart;

    public int Speed;

    public int MinValue;

    private float _currentTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(_started)
        {

            gameObject.transform.position += new Vector3(0, 0, -Speed * Time.deltaTime);
            if(gameObject.transform.position.z < MinValue)
            {
                //No need to move anymore
                Destroy(gameObject.GetComponent<CameraMove>());
            }
        }
        else
        {
            _currentTime += Time.deltaTime;
            if(_currentTime >= TimeStart)
            {
                _started = true;
            }
        }
	}
}
