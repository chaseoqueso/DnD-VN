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
        return source.GetDisplayName() + " dealt " + target.GetDamageAmount(calculateDamage(source)) + " damage to " + target.GetDisplayName() + ".";
    }

    private DamageData calculateDamage(CreatureInstance source)
    {
        return new DamageData(source.data.BaseDamage * damageMultiplier, ( (EnemyInstance)source ).data.DamageType);
    }
}
