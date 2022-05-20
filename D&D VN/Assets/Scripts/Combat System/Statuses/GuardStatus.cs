using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardStatus : ModifyDamageStatus
{
    private float damageReductionPercent;

    public GuardStatus(float endTurn, float damageReductionPercent) : base(endTurn)
    {
        this.damageReductionPercent = damageReductionPercent;
    }

    public override DamageData ModifyDamage(DamageData damage)
    {
        damage.damageAmount *= (1 - damageReductionPercent);
        return damage;
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.TakeDamage;
    }
}
