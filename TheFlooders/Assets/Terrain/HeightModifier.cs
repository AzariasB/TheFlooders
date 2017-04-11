using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Classe qui décrit un effet de modification de hauteur applicable à un terrain (solide, eau, etc.).
/// </summary>
public abstract class HeightModifier
{
    /// <summary>
    /// Indique si ce modificateur est en train de changer. Ce peut
    /// être le cas par exemple pour un modificateur qui fait pousser une
    /// montagne progressivement. Savoir qu'il ne change pas/plus permet
    /// d'éviter de recalculer son effet sur le terrain à chaque frame par exemple.
    /// </summary>
    public virtual bool IsChanging {
        get {
            return false;
        }
    }

    /// <summary>
    /// Retourne la zone (projetée sur le plan du terrain) sur laquelle ce
    /// modificateur peut avoir un effet. ceci évite d'appliquer l'effet
    /// sur l'intégralité du terrain.
    /// </summary>
    public abstract Rect GetAreaOfEffect();

    /// <summary>
    /// Retourne la nouvelle hauteur du terrain suite à l'application de ce modificateur.
    /// </summary>
    public abstract float Apply(TerrainBuilder onTerrain, Vector3 atPosition);
}
