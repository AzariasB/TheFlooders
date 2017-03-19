using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour {

    public Vector3 Destination;

    public float LevelDuration {
        get {
            return _levelDuration;
        }
        set {
            _levelDuration = value;
        }
    }
    [SerializeField]
    [Tooltip("Durée du niveau")]
    private float _levelDuration = 60;


    public float StartDelay {
        get {
            return _startDelay;
        }
        set {
            _startDelay = value;
        }
    }
    [SerializeField]
    [Tooltip("Temps à partir duquel la caméra commence à bouger")]
    public float _startDelay = 3;

    public TerrainHeightMap HeightMap {
        get {
            return _heightMap;
        }
    }
    [SerializeField]
    [Tooltip("L'objet qui génère le terrain du plateau de jeu")]
    private TerrainHeightMap _heightMap;

    public static LevelInfo Instance {
        get {
            return _instance;
        }
        private set {
            _instance = value;
        }
    }
    private static LevelInfo _instance;

	// Use this for initialization
	void Start () {
        Instance = this;

        if (_heightMap == null)
            _heightMap = GameObject.FindObjectOfType<TerrainHeightMap>();
        if (_heightMap == null)
            Debug.LogWarning("Le générateur de terrian n'est pas renseigné et n'a pas pu être trouvé.");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
