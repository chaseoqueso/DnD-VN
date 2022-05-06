using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TargetType
{
    none,

    allies,
    enemies
}
    
public class QueuedAction : UnityEvent {};

public abstract class ActionData : ScriptableObject
{

    public TargetType Target { get {return target;} }

    [Header("Generic Action Properties")]
    [SerializeField] [Tooltip("Whether this action can target an ally, an enemy, or if this action doesn't need a specific target (e.g. \"self\" or \"all\" effects).")]
    private TargetType target;

    public abstract QueuedAction PerformAction(TurnManager.CreatureInstance source, TurnManager.CreatureInstance target);
}
