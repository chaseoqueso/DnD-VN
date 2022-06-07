using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// <summary> Describes how the charge bar functions during a charged action </summary>
public struct ChargeBarData
{
    // <summary> Dictates what should happen when the charge bar reaches full </summary>
    public enum ChargeOverflowAction
    {
        reset,
        bounce
    }

    // <summary> Dictates the rate at which the charge bar should increase/decrease </summary>
    public enum ChargeSpeed
    {
        linear,
        exponential,
        sporadic
    }
}

public class CharacterQueuedAction : ChargeableQueuedAction
{
    public new CharacterActionData data
    {
        get { return (CharacterActionData)base.data; }
        set { base.data = value; }
    }

    public CharacterQueuedAction(CharacterActionData data, CreatureInstance source, CreatureInstance target, float chargePercent) : base(data, source, target, chargePercent)
    {
        this.data = data;
    }
}

public abstract class CharacterActionData : ChargeableActionData
{
    public string SkillName { get {return skillName;} }
    public string SkillDescription { get {return skillDescription;} }

    [Header("Character Action Properties")]
    [SerializeField] [Tooltip("The player-facing name of the action.")]
    private string skillName;

    [SerializeField] [TextArea] [Tooltip("The player-facing description of the action.")]
    private string skillDescription;
    
    public abstract CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent);

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, 0);
        action.AddListener(() => GetQueuedAction(source, target, 0));
        return action;
    }
}
