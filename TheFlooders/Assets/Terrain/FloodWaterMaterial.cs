using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloodWaterMaterial : MonoBehaviour {

    private const float DEFAULT_PERIOD = 3.0f;
    private const float DEFAULT_ALPHA_BLEND = 0.3f;
    private const float DEFAULT_X_SLIDE = 0.0f;
    private const float DEFAULT_Y_SLIDE = 0.25f;
    private const float DEFAULT_X_BASE_OFFSET = 0.25f;
    private const float DEFAULT_Y_BASE_OFFSET = 0.0f;

    private const string SHADER_UV_SHIFT_X = "_OverlayUVShiftX";
    private const string SHADER_UV_SHIFT_Y = "_OverlayUVShiftY";
    private const string SHADER_ALPHA = "_OverlayAlpha";

    [Tooltip("Composant qui rend l'eau ; doit avoir un matériau qui utilise le shader FloodWater")]
    public MeshRenderer targetRenderer;

    [Tooltip("Période des ondes")]
    public float period = DEFAULT_PERIOD;

    [Tooltip("Force de l'effet en surimpression")]
    public float alphaBlend = DEFAULT_ALPHA_BLEND;

    [Tooltip("Déplacement de texture de base")]
    public Vector2 baseOffset = new Vector2(DEFAULT_X_BASE_OFFSET, DEFAULT_Y_BASE_OFFSET);

    [Tooltip("Direction du déplacement")]
    public Vector2 slideDirection = new Vector2(DEFAULT_X_SLIDE, DEFAULT_Y_SLIDE);

    private Material lastCheckedMaterial = null;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Material mat = (targetRenderer != null ? targetRenderer.material : null);
        if (!CheckMaterial(mat))
        {
            enabled = false;
            return;
        }
        
		// Réinitialisation des paramètres si invalides
        if (period < 0)
            period = DEFAULT_PERIOD;
        if (slideDirection == Vector2.zero)
            slideDirection = new Vector2(DEFAULT_X_SLIDE, DEFAULT_Y_SLIDE);
        if (alphaBlend < 0 || alphaBlend > 1)
            alphaBlend = DEFAULT_ALPHA_BLEND;

        // Calcul de la phase
        float phase = Time.time / period;

        // Répétition des valeurs de sinus sur l'intervalle [-Pi/2; Pi/2]
        // pour n'avoir que des sous-intervalles croisants pour le déplacement
        float pi = Mathf.PI;
        float slideCoef = Mathf.Sin(phase - pi / 2 - pi * (int) (phase / pi));

        // Alpha : partie absolue de sinus pour que la superposition
        // soit le plus visible quand elle bouge le plus vite
        float alpha = Mathf.Abs(Mathf.Sin(phase));

        Vector2 slide = baseOffset + slideCoef * slideDirection;
        mat.SetFloat(SHADER_UV_SHIFT_X, slide.x);
        mat.SetFloat(SHADER_UV_SHIFT_Y, slide.y);
        mat.SetFloat(SHADER_ALPHA, alpha * alphaBlend);
	}

    private bool CheckMaterial(Material mat) {
        bool res = true;
        if (mat != lastCheckedMaterial)
        {
            if (mat != null)
            {
                foreach (string propertyName in new string[] {
                    SHADER_UV_SHIFT_X,SHADER_UV_SHIFT_Y, SHADER_ALPHA})
                {
                    if (!mat.HasProperty(propertyName))
                    {
                        Debug.LogWarning("Propriété manquante sur le matériau : " + (propertyName ?? "<null>"));
                        res = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Matériau manquant, ce script va être désactivé");
                res = false;
            }
        }

        lastCheckedMaterial = (res ? mat : null);
        return res;
    }
}
