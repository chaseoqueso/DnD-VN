using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rejuvenate", menuName = "Combat Data/Attacks/Rejuvenate")]
public class Rejuvenate : CharacterActionData
{
    [Header("Basic Attack Properties")]
    [SerializeField] [Tooltip("The amount of HP to heal with this ability at 0% charge.")]
    private float minHealingAmount = 20;
    [SerializeField] [Tooltip("The amount of HP to heal with this ability at 100% charge.")]
    private float maxHealingAmount = 100;

    public override QueuedAction PerformAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        QueuedAction action = new QueuedAction();
        action.AddListener(() => target.Heal(calculateHealAmount(chargePercent)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        return source.GetDisplayName() + " healed " + target.GetDisplayName() + " for " + calculateHealAmount(chargePercent) + " hit points.";
    }

    private float calculateHealAmount(float chargePercent)
    {
        return Mathf.Lerp(minHealingAmount, maxHealingAmount, chargePercent);
    }
}
