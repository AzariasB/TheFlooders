using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationAgent : MonoBehaviour {

    public Transform target;
    UnityEngine.AI.NavMeshAgent agent;


    // Use this for initialization
    void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        agent.SetDestination(target.position);
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("objectifGauche"))
        {
            other.gameObject.SetActive(false);

        }
    }

}
