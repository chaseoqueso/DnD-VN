using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Guard", menuName = "Combat Data/Attacks/Guard")]
public class Guard : CharacterActionData
{
    [Header("Guard Properties")]
    [SerializeField] [Tooltip("The amount to reduce incoming damage at 0% charge.")]
    protected float minDamageReduction = 0.2f;
    [SerializeField] [Tooltip("The amount to reduce incoming damage at 100% charge.")]
    protected float maxDamageReduction = 0.5f;

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        source.ApplyStatus(new GuardStatus(-1, CalculateDamageReduction(chargePercent)));
        return new CharacterQueuedAction(this, source, target, chargePercent);
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        return "This probably won't get seen.";
    }

    protected float CalculateDamageReduction(float chargePercent)
    {
        return Mathf.Lerp(minDamageReduction, maxDamageReduction, chargePercent);
    }
}
