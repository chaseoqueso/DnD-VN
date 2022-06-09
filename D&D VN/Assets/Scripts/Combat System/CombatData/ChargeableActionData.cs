using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChargeableQueuedAction : QueuedAction
{
    public new ChargeableActionData data
    {
        get { return (ChargeableActionData)base.data; }
        set { base.data = value; }
    }

    public float chargePercent;

    public ChargeableQueuedAction(ChargeableActionData data, CreatureInstance source, CreatureInstance target, float chargePercent) : base(data, source, target)
    {
        this.chargePercent = chargePercent;
        this.data = data;
    }
}

public abstract class ChargeableActionData : ActionData
{
    public float MinChargeLengthMultiplier {get {return minChargeLengthMultiplier;}}
    public float MaxChargeLengthMultiplier {get {return maxChargeLengthMultiplier;}}

    [SerializeField] [Tooltip("The minimum amount of time before a charged action will be performed (calculated as a percent of turnLength).")]
    private float minChargeLengthMultiplier = 0f;
    [SerializeField] [Tooltip("The maximum amount of time before a charged action will be performed (calculated as a percent of turnLength).")]
    private float maxChargeLengthMultiplier = 0.5f;
    
    public abstract ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent);

    // <summary> Returns the string that will be displayed once the action is performed. Should describe the ability as if it already happened. </summary>
    public abstract string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent);

    public override QueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, 0);
        action.AddListener(() => GetQueuedAction(source, target, 0));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target)
    {
        return GetAbilityPerformedDescription(source, target, 0);
    }

    public float CalculateChargeDelay(CreatureInstance creature, float chargePercent)
    {
        return creature.GetTurnLength() * Mathf.Lerp(MinChargeLengthMultiplier, MaxChargeLengthMultiplier, chargePercent);
    }
}
