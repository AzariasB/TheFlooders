using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cette classe représente une instance de pouvoir, c'est à dire un pouvoir qui a été
/// lancé et qui se trouve quelque part sur le plateau de jeu.
/// </summary>
public class Power : MonoBehaviour {

    /// <summary>
    /// Déclenche le pouvoir.
    /// </summary>
    public virtual void Fire() { }

    /// <summary>
    /// Arrête le pouvoir et supprime ses effets.
    /// </summary>
    public virtual void Clean() { }
}
