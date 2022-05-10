using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Attack", menuName = "Combat Data/Attacks/Basic Attack")]
public class BasicAttack : CharacterActionData
{
    [Header("Basic Attack Properties")]
    [SerializeField] [Tooltip("The type of damage to deal with this attack.")]
    private DamageType damageType;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack at 0% charge.")]
    private float minDamageMultiplier = 1;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack at 100% charge.")]
    private float maxDamageMultiplier = 2;

    public override QueuedAction PerformAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        QueuedAction action = new QueuedAction();
        DamageData damage = calculateDamage(source, chargePercent);
        action.AddListener(() => target.DealDamage(damage));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        DamageData damage = calculateDamage(source, chargePercent);

        float damageAmount = damage.damageAmount;
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

        return source.GetDisplayName() + " dealt " + target.GetDamageAmount(damage) + " damage to " + target.GetDisplayName() + "." + effectivenessDescription;
    }

    private DamageData calculateDamage(CreatureInstance source, float chargePercent)
    {
        return new DamageData(source.data.BaseDamage * Mathf.Lerp(minDamageMultiplier, maxDamageMultiplier, chargePercent), damageType);
    }
}