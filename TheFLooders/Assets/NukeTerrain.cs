using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeTerrain : MonoBehaviour {

    public TerrainHeightMap target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.B)) {
            Random.InitState (System.Guid.NewGuid ().GetHashCode());
            float bombX = target.Width*(Random.value * 2 - 1);
            float bombZ = target.Height*(Random.value * 2 - 1);
            float ampl = (Random.value - 0.5f) * 60;
            target.ApplyOnZone(new Rect(bombX - 50, bombZ - 50, bombX + 50, bombZ + 50), source => {
                Vector3 epicenter = new Vector3(bombX, source.y, bombZ);
                float dstSqr = 0.005f * (source - epicenter).sqrMagnitude;
                float delta = ampl / (1 + dstSqr);
                return source.y + delta;
            });
        }
	}
}
