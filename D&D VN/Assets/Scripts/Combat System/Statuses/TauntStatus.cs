using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntStatus : ModifyActionStatus
{
    public CreatureInstance taunter { get; private set; }

    public TauntStatus(float endTurn, CreatureInstance taunter) : base(endTurn)
    {
        this.taunter = taunter;
    }

    public override QueuedAction ModifyAction(QueuedAction action, bool triggerStatus)
    {
        action = base.ModifyAction(action, triggerStatus);

        if(action is ChargeableQueuedAction)
        {
            ChargeableQueuedAction chargeAction = (ChargeableQueuedAction)action;
            ChargeableActionData chargeData = chargeAction.data;
            action = chargeData.GetQueuedAction(action.source, taunter, chargeAction.chargePercent);
        }
        else
        {
            action = action.data.GetQueuedAction(action.source, taunter);
        }

        return action;
    }

    public override StatusTrigger GetStatusTrigger()
    {
        return StatusTrigger.PerformAction;
    }

    public override bool IsOneTimeEffect()
    {
        return false;
    }
}
