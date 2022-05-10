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

public abstract class CharacterActionData : ActionData
{
    public string SkillName { get {return skillName;} }
    public string SkillDescription { get {return skillDescription;} }

    [Header("Character Action Properties")]
    [SerializeField] [Tooltip("The player-facing name of the action.")]
    private string skillName;

    [SerializeField] [TextArea] [Tooltip("The player-facing description of the action.")]
    private string skillDescription;

    [SerializeField] [Tooltip("The minimum amount of time before a charged action will be performed (calculated as a percent of turnLength).")]
    private float minChargeLengthMultiplier = 0f;
    [SerializeField] [Tooltip("The maximum amount of time before a charged action will be performed (calculated as a percent of turnLength).")]
    private float maxChargeLengthMultiplier = 0.5f;
    
    public abstract QueuedAction PerformAction(CreatureInstance source, CreatureInstance target, float chargePercent);

    // <summary> Returns the string that will be displayed once the action is performed. Should describe the ability as if it already happened. </summary>
    public abstract string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent);

    public override QueuedAction PerformAction(CreatureInstance source, CreatureInstance target)
    {
        QueuedAction action = new QueuedAction();
        action.AddListener(() => PerformAction(source, target, 0));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target)
    {
        return GetAbilityPerformedDescription(source, target, 0);
    }

    public float CalculateChargeDelay(CharacterCombatData character, float chargePercent)
    {
        return character.TurnLength * Mathf.Lerp(minChargeLengthMultiplier, maxChargeLengthMultiplier, chargePercent);
    }
}
