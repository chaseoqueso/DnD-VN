using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Attack", menuName = "Combat Data/Attacks/Basic Attack")]
public class BasicAttack : CharacterActionData
{
    [Header("Basic Attack Properties")]
    [SerializeField] [Tooltip("The type of damage to deal with this attack.")]
    protected DamageType damageType;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack at 0% charge.")]
    protected float minDamageMultiplier = 1;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack at 100% charge.")]
    protected float maxDamageMultiplier = 2;

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => target.DealDamage(calculateDamage(source, chargePercent, true)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        DamageData damage = calculateDamage(source, chargePercent);

        string effectivenessDescription = "";

        if(target is EnemyInstance)
        {
            float damageMultiplier = ( (EnemyInstance)target ).GetDamageEffectiveness(damage);

            if(damageMultiplier == 2)
            {
                effectivenessDescription = " It seemed to shudder at the attack!";
            }
            else if (damageMultiplier == 0.5f)
            {
                effectivenessDescription = " It seemed somewhat unfazed by the attack.";
            }
        }

        return source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + "." + effectivenessDescription;
    }

    protected DamageData calculateDamage(CreatureInstance source, float chargePercent, bool endOneTimeStatuses = false)
    {
        DamageData damage = new DamageData(source.data.BaseDamage * Mathf.Lerp(minDamageMultiplier, maxDamageMultiplier, chargePercent), damageType);
        damage = source.TriggerStatuses(StatusTrigger.DealDamage, damage, endOneTimeStatuses);
        return damage;
    }
}
