using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlissementPieton : MonoBehaviour {

    public float speedPieton;
    public float forceDescente;
    private Vector3 movement = new Vector3(0, 0, -1);

    // Use this for initialization
    void Start () {
		//private Vector3 locaInitiale = GetComponent<Rigidbody>().position;

  //      public double GetHeight(locaInitiale.x, locaInitiale.y)
  //      {
  //          double res = 0;
  //          if (_heightData != null && _heightData.Length >= 2)
  //          {
  //              int colIdx = (int)(Mathf.Clamp(x, 0, _width) / _width * (_heightData.Length - 1));
  //              float[] col = _heightData[colIdx];
  //              if (col.Length >= 2)
  //              {
  //                  int rowIdx = (int)(Mathf.Clamp(y, 0, _height) / _height * (col.Length - 1));
  //                  res = col[rowIdx];
  //              }
  //          }
  //          return res;
  //      }

  //  locaInitiale.z = res;
    // GetComponent<Rigidbody>().position = Vector3.zero;
}
	
	// Update is called once per frame
	private void FixedUpdate () {

        transform.position = transform.position + movement * speedPieton;

    }
}
