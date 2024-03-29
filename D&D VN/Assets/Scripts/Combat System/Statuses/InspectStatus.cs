using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTakenMultiplierStatus : ModifyDamageStatus
{
    private float damageMultiplier;

    public DamageTakenMultiplierStatus(float endTurn, float damageMultiplier) : base(endTurn)
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
        return StatusTrigger.TakeDamage;
    }

    public override bool IsOneTimeEffect()
    {
        return true;
    }
}
