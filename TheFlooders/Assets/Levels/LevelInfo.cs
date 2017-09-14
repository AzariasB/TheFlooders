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


    /// <summary>
    /// Terrain du niveau. Le composant qui s'occupe du terrain devrait
    /// s'enregistrer ici quand il exécute Start().
    /// </summary>
    public TerrainHeightMap Ground { get; set; } 

	public FloodMeshAnimator Ground_eau { get; set; } //pareil mais pour le plan d'eau

	protected virtual void Awake () {
        Instance = this;
	}
}
