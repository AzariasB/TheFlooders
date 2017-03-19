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

	public bool iscompleted;

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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
