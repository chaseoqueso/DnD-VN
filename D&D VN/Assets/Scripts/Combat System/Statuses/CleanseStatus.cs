using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanseStatus : GenericStatus
{
    protected CreatureInstance statusTarget;

    public CleanseStatus(CreatureInstance target, float endTurn) : base(endTurn) 
    {
        statusTarget = target;
    }

    public override bool StatusHasEnded()
    {
        statusTarget.canReceiveStatuses = base.StatusHasEnded();
        return base.StatusHasEnded();
    }

    public override void PerformStatus(bool triggerStatus)
    {
        base.PerformStatus(triggerStatus);

        statusTarget.canReceiveStatuses = StatusHasEnded();
        Debug.Log("Can receive statuses: " + StatusHasEnded());
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.ApplyStatus;
    }

    public override bool IsOneTimeEffect()
    {
        return false;
    }
}
