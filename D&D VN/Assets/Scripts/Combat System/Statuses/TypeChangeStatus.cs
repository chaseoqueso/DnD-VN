using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChangeStatus : ModifyDamageStatus
{
    private DamageType damageType;

    public TypeChangeStatus(float endTurn, DamageType newDamageType) : base(endTurn)
    {
        this.damageType = newDamageType;
    }

    public override DamageData ModifyDamage(DamageData damage, bool triggerStatus)
    {
        damage = base.ModifyDamage(damage, triggerStatus);
        damage.damageType = this.damageType;
        return damage;
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.DealDamage;
    }

    public override bool IsOneTimeEffect()
    {
        return false;
    }
}
