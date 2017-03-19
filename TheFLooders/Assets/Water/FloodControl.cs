using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodControl : MonoBehaviour {

    [Tooltip("Angle en radians du plan d'inondation")]
    public float tiltAngleInDegrees = -10f;

    [Tooltip("Angle de rotation du plan d'inondation")]
    public Vector3 rotAxis = new Vector3 (-1, 0, 0);

    [Tooltip("Plan de l'inondation")]
    public MeshRenderer tiltedPlane;

    [Tooltip("Plan du niveau ordinaire de l'eau")]
    public MeshRenderer stillWaterPlane;

    private float _accumulatedTime;
    private float _speed;
    private float _halfHeight;

    void Start () {
        if (tiltedPlane == null) 
            Debug.LogError("Plan incliné de l'inondation manquant");
        if (stillWaterPlane == null) 
            Debug.LogError ("Plan du niveau ordinaire de l'eau manquant");
        if (tiltedPlane == null || stillWaterPlane == null) {
            enabled = false;
            return;
        }

        _halfHeight = tiltedPlane.bounds.extents.z;
        float angleInRadians = tiltAngleInDegrees * 2 * Mathf.PI / 360;
        _speed = (
            LevelInfo.Instance.LevelDuration > 0 ?
            2 * _halfHeight * Mathf.Abs(Mathf.Sin (angleInRadians)) / LevelInfo.Instance.LevelDuration:
            0);
        tiltedPlane.transform.localRotation = Quaternion.AngleAxis (tiltAngleInDegrees, rotAxis);
    }

    void Update () {
        _accumulatedTime += Time.deltaTime;
        if (_accumulatedTime > LevelInfo.Instance.StartDelay) {
            float elapsedTime = _accumulatedTime - LevelInfo.Instance.StartDelay;
            Vector3 refPos = stillWaterPlane.transform.position;
            float angleInRadians = tiltAngleInDegrees * 2 * Mathf.PI / 360;
                transform.position = new Vector3 (refPos.x, refPos.y - _halfHeight * Mathf.Abs(Mathf.Sin(angleInRadians)) + elapsedTime * _speed, refPos.z);
        }
    }
}
