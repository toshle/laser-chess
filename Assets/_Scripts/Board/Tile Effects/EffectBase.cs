using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{
    [SerializeField] protected Board _board;

    public void Start()
    {
        var x = (int)Math.Round(transform.localPosition.x);
        var y = (int)Math.Round(transform.localPosition.z);
        var tile = _board.GetTileAtPosition(new Vector2(x, y));
        tile.Effect = this;
    }
    public virtual void Activate(Unit triggerUnit)
    {
        Debug.Log("Activated effect");
    }
}
