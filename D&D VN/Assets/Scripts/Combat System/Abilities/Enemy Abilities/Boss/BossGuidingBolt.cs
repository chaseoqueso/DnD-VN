using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Guiding Bolt", menuName = "Combat Data/Attacks/Boss Guiding Bolt")]
public class BossGuidingBolt : ChargeableActionData
{
    protected DamageType damageType { get { return DamageType.Light; } }

    [SerializeField] [Tooltip("The amount of the creature's base damage to deal with this attack.")]
    protected float damageMultiplier = 1;

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => 
        {
            target.DealDamage(calculateDamage(source, chargePercent, true));
            target.ApplyStatus(new DamageTakenMultiplierStatus(-1, 2));
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString;

        DamageData damage = calculateDamage(source, chargePercent);

        if(target.canReceiveStatuses)
        {
            descString = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + " and lowered its defenses!";
        }
        else
        {
            descString = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + "." + " Its defenses couldn't be lowered because it was cleansed!";
        }
        
        return descString;
    }

    protected DamageData calculateDamage(CreatureInstance source, float chargePercent, bool endOneTimeStatuses = false)
    {
        DamageData damage = new DamageData(source.GetBaseDamage() * damageMultiplier, damageType);
        damage = source.TriggerStatuses(StatusTrigger.DealDamage, damage, endOneTimeStatuses);
        return damage;
    }
}
