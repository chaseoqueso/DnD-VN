using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Empower", menuName = "Combat Data/Attacks/Enemy Empower")]
public class EnemyEmpower : ChargeableActionData
{
    [SerializeField] [Tooltip("The amount to boost the target's next attack.")]
    private float damageMultiplier = 1.5f;

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargeAmount)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, chargeAmount);
        action.AddListener(() => 
        {
            target.ApplyStatus(new EmpowerStatus(-1, damageMultiplier));
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargeAmount)
    {
        string descString;
        
        if(target.canReceiveStatuses)
        {
            descString = source.GetDisplayName() + " is empowering " + target.GetDisplayName() + "'s next attack!";
        }
        else
        {
            descString = source.GetDisplayName() + " tried to empower " + target.GetDisplayName() + "'s next attack, but it was cleansed!";
        }

        return descString;
    }
}
