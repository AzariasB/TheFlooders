using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cette classe représente un petit morceau d'IHM temporaire
/// qui permet de placer un pouvoir. En première approximation, c'est
/// une texture qui suit la souris, et qui place le pouvoir là où elle se
/// trouve quand on clique gauche.
/// </summary>
public abstract class PowerPlacer : MonoBehaviour {

    [Tooltip("Objet à placer dans la scène là où le pouvoir est déclenché. " +
        "Si ceci est nul, un gameObject par défaut sera placé.")]
    public Power powerPrefab;

    [Tooltip("Compteur d'utilisations du pouvoir en train d'être placé.")]
    public PowerUsesCounter usesCounter;

    /// <summary>
    /// Indique si le pouvoir a été placé.
    /// </summary>
    private bool placed;

    protected virtual void Awake () {
        placed = false;
    }

    protected virtual void Update () {
        if (!placed)
        {
            bool rayCast;
            RaycastHit hitPoint;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            rayCast = Physics.Raycast(ray, out hitPoint, 1000.0f);
            bool donePlacing = false;

            if (Input.GetMouseButtonDown(0))
            {
                placed = rayCast;
                if (placed)
                {
                    Power powerInstance;
                    if (powerPrefab != null)
                        powerInstance = Instantiate(powerPrefab);
                    else
                    {
                        GameObject root = new GameObject("Default power instance");
                        powerInstance = root.AddComponent<Power>();
                    }
                    powerInstance.gameObject.tag = GetPowerTag();
                    powerInstance.transform.position = transform.position;

                    gameObject.AddComponent<BoxCollider>().isTrigger = true;
                    gameObject.AddComponent<SpeedReducer>().DragDivisor = 5;
                    usesCounter.Decrement();
                    powerInstance.Fire();
                    donePlacing = true;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                donePlacing = true;
            }
            else
            {
                transform.position = hitPoint.point;
            }

            if (donePlacing)
            {
                PowerUsesCounter.EnableButtons();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Tag du pouvoir, qui permet de retrouver ses instances
    /// </summary>
    protected abstract string GetPowerTag();
}
