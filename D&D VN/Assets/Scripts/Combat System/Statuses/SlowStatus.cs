using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowStatus : GenericStatus
{
    private float slowAmount;
    private CreatureInstance statusTarget;

    public SlowStatus(CreatureInstance target, float endTurn, float slowAmount) : base(endTurn)
    {
        statusTarget = target;
        statusTarget.SetSpeedMultiplier(statusTarget.speedMultiplier * (1 - slowAmount));

        this.slowAmount = slowAmount;
    }

    public override bool StatusHasEnded()
    {
        if(base.StatusHasEnded())
            statusTarget.SetSpeedMultiplier(statusTarget.speedMultiplier / (1 - slowAmount));
        
        return base.StatusHasEnded();
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
