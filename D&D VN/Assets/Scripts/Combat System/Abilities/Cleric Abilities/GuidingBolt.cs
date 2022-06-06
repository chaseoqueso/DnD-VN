using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Guiding Bolt", menuName = "Combat Data/Attacks/Guiding Bolt")]
public class GuidingBolt : BasicAttack
{
    protected new DamageType damageType { get { return DamageType.Light; } }

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = base.GetQueuedAction(source, target, chargePercent);
        action.AddListener(() => target.ApplyStatus(new InspectStatus(-1, 2)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString;

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

        if(target.canReceiveStatuses)
        {
            descString = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + " and lowered its defenses!" + effectivenessDescription;
        }
        else
        {
            descString = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + "." + effectivenessDescription + " Its defenses couldn't be lowered because it was cleansed!";
        }
        
        return descString;
    }
}
