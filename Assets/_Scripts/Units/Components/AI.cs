using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] private int _priority;
    [SerializeField] private MovementBehaviour _movementBehaviour;

    public int Priority => _priority;
    public MovementBehaviour MovementBehaviour => _movementBehaviour;
}

public enum MovementBehaviour
{
    ClosestEnemy,
    LowestThreat,
    Forward
}