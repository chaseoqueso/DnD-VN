using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice Knife", menuName = "Combat Data/Attacks/Ice Knife")]
public class IceKnife : BasicAttack
{
    [Header("Ice Knife Properties")]
    [SerializeField] [Tooltip("The percent of the character's turn length to slow the target for.")]
    protected float statusDuration = 1;

    public override CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => 
        { 
            target.DealDamage(calculateDamage(source, chargePercent, true)); 
            target.ApplyStatus(new SlowStatus(target, TurnManager.Instance.currentTurn + CalculateChargeDelay((CharacterInstance)source, chargePercent) * statusDuration, 0.5f));
        });
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
                effectivenessDescription = "It seemed to shudder at the attack! ";
            }
            else if (damageMultiplier == 0.5f)
            {
                effectivenessDescription = "It seemed somewhat unfazed by the attack. ";
            }
        }

        Debug.Log("Getting description.");
        return source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + ". " + effectivenessDescription;
    }
}
