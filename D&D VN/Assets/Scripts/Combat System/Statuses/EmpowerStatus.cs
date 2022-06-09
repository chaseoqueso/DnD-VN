using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpowerStatus : ModifyDamageStatus
{
    private float damageMultiplier;

    public EmpowerStatus(float endTurn, float damageMultiplier) : base(endTurn)
    {
        this.damageMultiplier = damageMultiplier;
    }

    public override DamageData ModifyDamage(DamageData damage, bool triggerStatus)
    {
        damage = base.ModifyDamage(damage, triggerStatus);
        damage.damageAmount *= damageMultiplier;
        return damage;
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.DealDamage;
    }

    public override bool IsOneTimeEffect()
    {
        return true;
    }
}
