using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusTrigger
{
    TurnStart,
    TakeDamage,
    DealDamage,
    ReceiveHealing,
    QueueAction,
    PerformAction,
    ApplyStatus
}

public abstract class Status
{
    public float endTurn { get; protected set; }

    // set endTurn to a negative number to end at the start of the creature's next turn
    public Status(float endTurn)
    {
        this.endTurn = endTurn;
    }

    public abstract bool IsOneTimeEffect();

    public virtual bool StatusHasEnded()
    {
        return TurnManager.Instance.currentTurn > endTurn && endTurn >= 0;
    }

    public virtual void TriggerStatus()
    {
        Debug.Log("Actually triggering status " + this);
        if(IsOneTimeEffect())
        {
            this.endTurn = 0;
        }
    }

    public abstract StatusTrigger GetStatusTrigger();
}

public abstract class ModifyDamageStatus : Status
{
    public ModifyDamageStatus(float endTurn) : base(endTurn) {}

    public virtual DamageData ModifyDamage(DamageData damage, bool triggerStatus)
    {
        Debug.Log("Call triggerStatus: " + triggerStatus);
        if(triggerStatus)
            TriggerStatus();

        return damage;
    }
}

public abstract class ModifyActionStatus : Status
{
    public ModifyActionStatus(float endTurn) : base(endTurn) {}

    public virtual QueuedAction ModifyAction(QueuedAction action, bool triggerStatus)
    {
        if(triggerStatus)
            TriggerStatus();
            
        return action;
    }
}

public abstract class ModifyHealingStatus : Status
{
    public ModifyHealingStatus(float endTurn) : base(endTurn) {}

    public virtual int ModifyHealing(int healAmount, bool triggerStatus)
    {
        if(triggerStatus)
            TriggerStatus();
            
        return healAmount;
    }
}

public abstract class ModifyCreatureStatus : Status
{
    public ModifyCreatureStatus(float endTurn) : base(endTurn) {}

    public virtual void ModifyCreature(CreatureInstance creature, bool triggerStatus)
    {
        if(triggerStatus)
            TriggerStatus();
    }
}

public abstract class GenericStatus : Status
{
    public GenericStatus(float endTurn) : base(endTurn) {}

    public virtual void PerformStatus(bool triggerStatus)
    {
        if(triggerStatus)
            TriggerStatus();
    }
}