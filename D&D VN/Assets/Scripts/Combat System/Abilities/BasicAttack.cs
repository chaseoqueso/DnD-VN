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
        DamageData damage = new DamageData(source.data.BaseDamage * Mathf.Lerp(minDamageMultiplier, maxDamageMultiplier, chargePercent), damageType);
        action.AddListener(() => target.DealDamage(damage));
        return action;
    }
}
