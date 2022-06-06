using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cleanse", menuName = "Combat Data/Attacks/Cleanse")]
public class Cleanse : CharacterActionData
{
    public new TargetType Target { get {return TargetType.any;} }

    [Header("Cleanse Properties")]
    [SerializeField] [Tooltip("The duration to prevent statuses at 0% charge.")]
    private float minStatusPreventionDuration = 0;
    [SerializeField] [Tooltip("The duration to prevent statuses at 100% charge.")]
    private float maxStatusPreventionDuration = 10;

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => target.Cleanse(Mathf.Lerp(minStatusPreventionDuration, maxStatusPreventionDuration, chargePercent)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        return source.GetDisplayName() + " cleansed " + target.GetDisplayName() + " of all status effects.";
    }
}
