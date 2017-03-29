using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodMeshAnimator : TerrainBuilder
{
    [Tooltip("Le terrain pour lequel ce script génère un mesh d'inondation")]
    public TerrainHeightMap targetSolidTerrain;

    [Tooltip("Hauteur initiale de l'eau")]
    [SerializeField]
    private float _initialHeight = 10;
    public float InitialHeight {
        get {
            return _initialHeight;
        }
    }

    [Tooltip("Vitesse de montée générale de l'eau")]
    [SerializeField]
    private float _generalRaiseSpeed = 0.03f;
    public float GeneralRaiseSpeed {
        get {
            return _generalRaiseSpeed;
        }
    }

    [Tooltip("Angle du plan d'eau derrière la ligne de crue, en degrés")]
    [Range(0, 20)]
    [SerializeField]
    private float _waveAngle = 10;
    public float WaveAngle {
        get {
            return _waveAngle;
        }
    }

    [Tooltip("Lissage du front de crue")]
    [Range(0, 1)]
    [SerializeField]
    private float _smoothing = 0.15f;
    public float Smoothing {
        get {
            return _smoothing;
        }
    }

    private bool spentFrameWithNoTerrain;

    protected override void Awake()
    {
        base.Awake();
        spentFrameWithNoTerrain = false;
        AddModifier(new WaterHeightModifier(this));
    }

    protected override void Update()
    {
        base.Update();

        if (targetSolidTerrain != null)
        {
            // On recopie toujours le terrain parent.
            Width = targetSolidTerrain.Width;
        }
        else
        {
            // Tentative de réparation : s'il y a un terrain solide sur le même GameObject on se greffe dessus.
            targetSolidTerrain = GetComponent<TerrainHeightMap>();
            if (targetSolidTerrain == null)
            {
                if (!spentFrameWithNoTerrain)
                    spentFrameWithNoTerrain = true;
                else
                {
                    Debug.LogError("Impossible de trouver le terrain cible pour créer un mesh d'inondation");
                    enabled = false;
                    return;
                }
            }
        }
    }

    protected override float ComputeTerrainZDimension(float width)
    {
        float res = 0;
        if (targetSolidTerrain != null && targetSolidTerrain.Width != 0)
            res = width * targetSolidTerrain.Height / targetSolidTerrain.Width;
        return res;
    }

    /// <summary>
    /// Modificateur de base pour le plan d'inondation, qui fait monter
    /// le niveau petit à petit.
    /// </summary>
    private class WaterHeightModifier : HeightModifier {


        /// <summary>
        /// Date à laquelle le mouvement de la caméra commence.
        /// </summary>
        private float _startTime;

        /// <summary>
        /// Vitesse à laquelle la vague se propage
        /// </summary>
        private float _waveSpeed;

        /// <summary>
        /// longueur de transition entre plans
        /// </summary>
        private float _smoothLength;

        private bool gotLevelinfo;

        private readonly FloodMeshAnimator target;

        public WaterHeightModifier(FloodMeshAnimator target) {
            this.target = target;
            _startTime = Time.time;
            gotLevelinfo = false;
        }

        public override bool IsChanging
        {
            get
            {
                return true;
            }
        }

        public override Rect GetAreaOfEffect()
        {
            Rect res = new Rect(new Vector2(-target.Width/2, -target.Height/2), new Vector2(target.Width, target.Height));
            return res;
        }

        public override float Apply(TerrainBuilder onTerrain, Vector3 atPosition)
        {
            // Récupération des infos du niveau
            if (!gotLevelinfo && target.targetSolidTerrain != null && LevelInfo.Instance != null)
            {
                LevelInfo lInfo = LevelInfo.Instance;
                _startTime = lInfo.StartDelay;
                _waveSpeed = -(lInfo.LevelDuration > 0 ? target.targetSolidTerrain.Height / lInfo.LevelDuration: 0);
                if (!target.targetSolidTerrain.IsEmpty)
                    _smoothLength = target.targetSolidTerrain.Width * target.Smoothing;
                else
                    _smoothLength = 0;
                gotLevelinfo = true;
            }

            if (gotLevelinfo)
            {
                float frontPos = target.targetSolidTerrain.Height / 2 + (Time.time - _startTime) * _waveSpeed;
                float pointPos = atPosition.z;

                // Hauteurs de chaque plan
                float unfloodedHeight = target.InitialHeight + target.GeneralRaiseSpeed * Time.time;
                float floodedHeight = target.InitialHeight + (pointPos - frontPos) * Mathf.Tan(target.WaveAngle * Mathf.PI / 180);

                // Paramètre d'interpolation entre les deux plans.
                float alpha;
                if (_smoothLength > 0)
                    alpha = (pointPos - frontPos) / _smoothLength + 0.5f;
                else
                    alpha = (pointPos > frontPos ? 1 : 0);

                alpha = Mathf.Clamp01(alpha);
                float alpha2 = alpha * alpha;
                float finalHeight = alpha2 * floodedHeight + (1 - alpha2) * unfloodedHeight;

                // Ce modificateur est censé donner la hauteur de base du terrain, donc
                // il ignore la valeur de hauteur précédente.
                return finalHeight;
            }
            else
                return 0;
        }
    }

}

