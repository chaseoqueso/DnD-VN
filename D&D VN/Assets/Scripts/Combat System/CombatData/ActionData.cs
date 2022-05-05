using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    none,

    allies,
    enemies
}

public abstract class ActionData : ScriptableObject
{
    public TargetType Target { get {return target;} }

    [Header("Generic Action Properties")]
    [SerializeField] [Tooltip("Whether this action can target an ally, an enemy, or if this action doesn't need a specific target (e.g. \"self\" or \"all\" effects).")]
    private TargetType target;

    public abstract void PerformAction(TurnManager.CreatureInstance source, TurnManager.CreatureInstance target);
}
