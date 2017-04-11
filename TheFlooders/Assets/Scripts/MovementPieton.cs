using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPieton : MonoBehaviour {

    private Rigidbody rb;

    public float speedPieton;

    private Vector3 movement = new Vector3(0, 0, -1);

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        rb.AddForce(movement * speedPieton);
    }
}
