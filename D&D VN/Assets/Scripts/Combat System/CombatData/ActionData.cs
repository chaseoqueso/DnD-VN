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
    
public class QueuedAction : UnityEvent 
{
    public ActionData data;
    public CreatureInstance source;
    public CreatureInstance target;

    public QueuedAction(ActionData data, CreatureInstance source, CreatureInstance target)
    {
        this.data = data;
        this.source = source;
        this.target = target;
    }
};

public abstract class ActionData : ScriptableObject
{
    public TargetType Target { get {return target;} }
    public bool DelayAfterActionPerformed { get {return delayAfterActionPerformed;} }

    [Header("Generic Action Properties")]
    [SerializeField] [Tooltip("Whether this action can target an ally, an enemy, or if this action doesn't need a specific target (e.g. \"self\" or \"all\" effects).")]
    private TargetType target;
    [SerializeField] [Tooltip("Whether to include a delay when requeueing the user after performing the action.")]
    private bool delayAfterActionPerformed = true;

    public abstract QueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target);

    // <summary> Returns the string that will be displayed once the action is performed. Should describe the ability as if it already happened. </summary>
    public abstract string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target);
}
