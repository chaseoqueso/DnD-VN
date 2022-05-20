using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusTrigger
{
    TurnStart,
    TakeDamage,
    DealDamage,
    QueueAction,
    PerformAction
}

public abstract class Status
{
    protected float endTurn;

    public Status(float endTurn)
    {
        this.endTurn = endTurn;
    }

    public bool StatusHasEnded()
    {
        return TurnManager.Instance.currentTurn > endTurn;
    }

    public abstract StatusTrigger GetStatusTrigger();
}

public abstract class ModifyDamageStatus : Status
{
    public ModifyDamageStatus(float endTurn) : base(endTurn) {}

    public abstract DamageData ModifyDamage(DamageData data);
}

public abstract class ModifyActionStatus : Status
{
    public ModifyActionStatus(float endTurn) : base(endTurn) {}

    public abstract QueuedAction ModifyAction(QueuedAction action);
}

public abstract class ModifyCreatureStatus : Status
{
    public ModifyCreatureStatus(float endTurn) : base(endTurn) {}

    public abstract void ModifyCreature(CreatureInstance creature);
}

public abstract class GenericStatus : Status
{
    public GenericStatus(float endTurn) : base(endTurn) {}

    public abstract void PerformStatus();
}