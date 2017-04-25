using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerButtonListener : MonoBehaviour {

    [Tooltip("Bouton qui déclenche l'action de l'utilisateur, que ce script traite.")]
    public Button button;

    [Tooltip("Compteur d'utilisation du pouvoir déclenché par ce bouton")]
    public PowerUsesCounter usesCounter;

    [Tooltip("Script qui s'occupe de viser le pouvoir sur le terrain et de le valider")]
    public PowerPlacer placerPrefab;

    protected virtual void Start () {
        if (button == null)
            button = GetComponent<Button>();
        if (usesCounter == null)
            usesCounter = GetComponent<PowerUsesCounter>();
        if (placerPrefab == null)
            placerPrefab = GetComponent<PowerPlacer>();
        button.onClick.AddListener(OnTriggeringPower);
    }

    protected virtual void OnTriggeringPower()
    {
        if (usesCounter.CanDecrement())
        {
            PowerPlacer placerInst = Instantiate(placerPrefab);
            placerInst.usesCounter = usesCounter;
            PowerUsesCounter.DisableButtons();
            AudioSource asource = GetComponent <AudioSource> ();
            if (asource != null) {
                asource.Play();
            }
        }
    }

}