using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyShield", menuName = "Combat Data/Attacks/EnemyShield")]
public class EnemyShield : ChargeableActionData
{
    [Header("Inspect Properties")]
    [SerializeField] [Tooltip("The amount to reduce damage against the target.")]
    private float damageReductionPercent = 0.5f;


    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, chargePercent);
        action.AddListener( () => {
            target.ApplyStatus(new DamageTakenMultiplierStatus(-1, 1 - damageReductionPercent));
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString;
        
        if(target.canReceiveStatuses)
        {
            descString = source.GetDisplayName() + " is shielding " + target.GetDisplayName() + " from incoming damage!";
        }
        else
        {
            descString = source.GetDisplayName() + " tried to shield " + target.GetDisplayName() + " from incoming damage, but it was cleansed!";
        }

        return descString;
    }
}
