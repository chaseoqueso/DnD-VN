using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rejuvenate", menuName = "Combat Data/Attacks/Rejuvenate")]
public class Rejuvenate : CharacterActionData
{
    public new TargetType Target { get {return TargetType.allies;} }

    [Header("Rejuvenate Properties")]
    [SerializeField] [Tooltip("The amount of HP to heal with this ability at 0% charge.")]
    private float minHealingAmount = 20;
    [SerializeField] [Tooltip("The amount of HP to heal with this ability at 100% charge.")]
    private float maxHealingAmount = 100;
    [SerializeField] [Tooltip("Whether this ability can heal a downed ally.")]
    private bool canRevive = false;

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => target.Heal(calculateHealAmount(chargePercent)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        return source.GetDisplayName() + " healed " + target.GetDisplayName() + " for " + calculateHealAmount(chargePercent) + " hit points.";
    }

    private int calculateHealAmount(float chargePercent)
    {
        return Mathf.CeilToInt(Mathf.Lerp(minHealingAmount, maxHealingAmount, chargePercent));
    }
}
