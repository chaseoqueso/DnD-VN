using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interpose", menuName = "Combat Data/Attacks/Interpose")]
public class Interpose : CharacterActionData
{
    [Header("Interpose Properties")]

    [SerializeField] [Tooltip("The percent of damage that will be redirected at 0% charge.")]
    protected float minDamageRedirectPercent = 0.2f;
    [SerializeField] [Tooltip("The percent of damage that will be redirected at 100% charge.")]
    protected float maxDamageRedirectPercent = 1;

    public override CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent)
    {
        target.ApplyStatus(new InterposeStatus(TurnManager.Instance.currentTurn + CalculateChargeDelay((CharacterInstance)source, chargePercent), source, calculateRedirectPercent(chargePercent)));
        return new CharacterQueuedAction(this, source, target, chargePercent);
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        return source.GetDisplayName() + " finished blocking damage for " + target.GetDisplayName() + ".";
    }

    protected float calculateRedirectPercent(float chargePercent)
    {
        return Mathf.Lerp(minDamageRedirectPercent, maxDamageRedirectPercent, chargePercent);
    }
}
