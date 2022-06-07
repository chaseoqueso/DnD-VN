using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Attack", menuName = "Combat Data/Attacks/Enemy Basic Attack")]
public class EnemyBasicAttack : ActionData
{
    [SerializeField] [Tooltip("The amount of the enemy's base damage to deal with this attack.")]
    private float damageMultiplier = 1;

    public override QueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target)
    {
        QueuedAction action = new QueuedAction(this, source, target);
        DamageData damage = calculateDamage(source);
        action.AddListener(() => target.DealDamage(damage));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target)
    {
        string descString = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(calculateDamage(source)) + " damage to " + target.GetDisplayName() + ". ";
        
        InterposeStatus interposeStatus = target.GetInterposeStatus();
        if(interposeStatus != null)
        {
            CreatureInstance interposer = interposeStatus.interposer;
            DamageData statusDamage = calculateDamage(source);
            statusDamage.damageAmount *= interposeStatus.damageRedirectPercent;

            descString += interposer.GetDisplayName() + " interposed, taking " + interposer.CalculateDamageTaken(statusDamage) + " damage.";
        }

        return descString;
    }

    private DamageData calculateDamage(CreatureInstance source)
    {
        return new DamageData(source.GetBaseDamage() * damageMultiplier, ( (EnemyInstance)source ).GetDamageType());
    }
}
