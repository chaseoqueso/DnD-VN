using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Attack", menuName = "Combat Data/Attacks/Enemy Basic Attack")]
public class EnemyBasicAttack : ActionData
{
    [SerializeField] [Tooltip("The amount of the enemy's base damage to deal with this attack.")]
    private float damageMultiplier = 1;

    public override QueuedAction PerformAction(CreatureInstance source, CreatureInstance target)
    {
        QueuedAction action = new QueuedAction();
        DamageData damage = new DamageData(source.data.BaseDamage * damageMultiplier, ( (EnemyInstance)source ).data.DamageType);
        action.AddListener(() => target.DealDamage(damage));
        return action;
    }
}
