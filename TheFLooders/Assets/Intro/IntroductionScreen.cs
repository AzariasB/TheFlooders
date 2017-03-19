using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroductionScreen : MonoBehaviour {

    private MovieTexture _movieTex;

	void Start () {
        MeshRenderer mr = GetComponent<MeshRenderer> ();
        Material mat = (mr != null ? mr.material : null);
        _movieTex = (mat != null ? mat.mainTexture as MovieTexture : null);

        if (_movieTex == null) {
            Debug.LogError ("Vidéo à lancer non trouvée");
            return;
        }
        _movieTex.Play();
	}
	
    // Change de scène une fois que la vidéo est terminée.
	void Update () {
        if (!_movieTex.isPlaying) {
            SceneManager.LoadScene("Level 1");
			enabled = false;
        }
	}
}
