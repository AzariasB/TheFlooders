using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeTerrain : MonoBehaviour {

    public TerrainBuilder target;

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
            target.AddModifier(new NukeMod(new Vector3(bombX, 0, bombZ),ampl));
            Debug.Log("Boum");
        }
	}

    private class NukeMod : HeightModifier {

        public readonly float Amplitude;
        public readonly Vector3 Epicenter;

        public NukeMod(Vector3 epicenter, float amplitude) {
            this.Amplitude = amplitude;
            this.Epicenter = epicenter;
        }

        public override System.Single Apply(TerrainBuilder onTerrain, Vector3 atPosition)
        {
            Vector3 realEpicenter = new Vector3(Epicenter.x, atPosition.y, Epicenter.z);
            float dstSqr = 0.005f * (atPosition - realEpicenter).sqrMagnitude;
            float delta = Amplitude / (1 + dstSqr);
            return atPosition.y + delta;
        }

        public override Rect GetAreaOfEffect()
        {
            return new Rect(new Vector2(Epicenter.x -25, Epicenter.z -25), new Vector2(50, 50));
        }
    }
}
