using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterposeStatus : ModifyDamageStatus
{
    public float damageRedirectPercent { get; private set; }
    public CreatureInstance interposer { get; private set; }

    public InterposeStatus(float endTurn, CreatureInstance interposer, float damageRedirectPercent) : base(endTurn)
    {
        this.damageRedirectPercent = damageRedirectPercent;
        this.interposer = interposer;
    }

    public override DamageData ModifyDamage(DamageData damage, bool triggerStatus)
    {
        damage = base.ModifyDamage(damage, triggerStatus);
        float originalDamageAmount = damage.damageAmount;

        damage.damageAmount *= (1 - damageRedirectPercent);

        DamageData redirectedDamage = new DamageData(originalDamageAmount - damage.damageAmount, damage.damageType);
        interposer.DealDamage(redirectedDamage);

        return damage;
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.TakeDamage;
    }

    public override bool IsOneTimeEffect()
    {
        return false;
    }
}
