using System;
using UnityEngine;

public abstract class EffectBase : MonoBehaviour
{
    [SerializeField] protected Board _board;

    public void Start()
    {
        var x = (int)Math.Round(Math.Abs(transform.localPosition.x));
        var y = (int)Math.Round(Math.Abs(transform.localPosition.z));
        var tile = _board.GetTileAtPosition(new Vector2(x, y));
        tile.Effect = this;
        transform.position = tile.transform.position;
    }
    public virtual void Activate(Unit triggerUnit)
    {
        Debug.Log("Activated effect");
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            Gizmos.color = new Color(1, 0, 0, 1);
            var gizmoSize = 0.6f;

            Gizmos.DrawLine(new Vector3(-gizmoSize / 2 + transform.position.x, 0, -gizmoSize / 2 + transform.position.z), new Vector3(-gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z));
            Gizmos.DrawLine(new Vector3(gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z), new Vector3(-gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z));
            Gizmos.DrawLine(new Vector3(gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z), new Vector3(gizmoSize / 2 + transform.position.x, 0, -gizmoSize / 2 + transform.position.z));
            Gizmos.DrawLine(new Vector3(-gizmoSize / 2f + transform.position.x, 0, -gizmoSize / 2 + transform.position.z), new Vector3(gizmoSize / 2 + transform.position.x, 0, -gizmoSize / 2 + transform.position.z));
        }
    }
    #endif
}
