using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedStatus : ModifyCreatureStatus
{
    private float bleedDamage;
    public BleedStatus(float endTurn, float bleedDamage) : base(endTurn)
    {
        this.bleedDamage = bleedDamage;
    }

    public override void ModifyCreature(CreatureInstance creature, bool triggerStatus)
    {
        base.ModifyCreature(creature, triggerStatus);

        creature.DealDamage(new DamageData(bleedDamage, DamageType.Neutral));
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.TurnStart;
    }

    public override bool IsOneTimeEffect()
    {
        return false;
    }
}
