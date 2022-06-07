using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Taunt", menuName = "Combat Data/Attacks/Taunt")]
public class Taunt : CharacterActionData
{
    [Header("Taunt Properties")]
    [SerializeField] [Tooltip("The percent of incoming damage to block at 0% charge.")]
    protected float minBlockAmount = 0.5f;
    [SerializeField] [Tooltip("The percent of incoming damage to block at 100% charge.")]
    protected float maxBlockAmount = 1;

    public override CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent)
    {
        source.ApplyStatus(new GuardStatus(TurnManager.Instance.currentTurn + CalculateChargeDelay((CharacterInstance)source, chargePercent), calculateBlockAmount(chargePercent)));
        
        foreach(EnemyInstance enemy in TurnManager.Instance.GetAllEnemies())
            enemy.ApplyStatus(new TauntStatus(TurnManager.Instance.currentTurn + CalculateChargeDelay((CharacterInstance)source, chargePercent), source));

        return new CharacterQueuedAction(this, source, target, chargePercent);
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        return source.GetDisplayName() + " finished taunting.";
    }

    protected float calculateBlockAmount(float chargePercent)
    {
        return Mathf.Lerp(minBlockAmount, maxBlockAmount, chargePercent);
    }
}
