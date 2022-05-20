using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureInstance
{
    public CreatureCombatData data 
    { 
        get { return _data; }
        protected set { _data = value; }
    }

    public bool isChargingAction { get; protected set; }
    protected QueuedAction queuedAction;

    protected CreatureCombatData _data;
    protected float currentHP;

    protected Dictionary<StatusTrigger, List<Status>> statusDictionary;

    public CreatureInstance()
    {
        statusDictionary = new Dictionary<StatusTrigger, List<Status>>();
    }

    public bool IsAlive()
    {
        return currentHP > 0;
    }

    public virtual string GetDisplayName()
    {
        return data.DisplayName;
    }

    public virtual void Heal(float healAmount)
    {
        currentHP += healAmount;

        if(currentHP > data.MaxHP)
        {
            currentHP = data.MaxHP;
        }

        UIManager.instance.combatUI.UpdateUIAfterCreatureHealed( _data, currentHP );
    }

    public virtual bool DealDamage(DamageData damage)
    {
        currentHP -= CalculateDamageTaken(damage);
        if(currentHP < 0)
        {
            currentHP = 0;
        }

        return IsAlive();
    }

    public virtual float CalculateDamageTaken(DamageData damage, bool capAtCurrentHP = true)
    {
        if(statusDictionary.ContainsKey(StatusTrigger.TakeDamage))
        {
            List<Status> statuses = new List<Status>(statusDictionary[StatusTrigger.TakeDamage]);
            foreach(Status status in statuses)
            {
                if(status.StatusHasEnded())
                {
                    statusDictionary[StatusTrigger.TakeDamage].Remove(status);
                }
                else if(status is ModifyDamageStatus)
                {
                    damage = ( (ModifyDamageStatus)status ).ModifyDamage(damage);
                }
                else if(status is ModifyCreatureStatus)
                {
                    ( (ModifyCreatureStatus)status ).ModifyCreature(this);
                }
                else if(status is GenericStatus)
                {
                    ( (GenericStatus)status ).PerformStatus();
                }
                else
                {
                    Debug.LogError("Status " + status + " had StatusTrigger TakeDamage, which is incompatible with Status type " + status.GetType());
                }
            }
        }

        float damageAmount = damage.damageAmount * (1 - data.Defense/100);

        if(capAtCurrentHP && damageAmount > currentHP)
            damageAmount = currentHP;

        return damageAmount;
    }

    public virtual float GetCurrentHealth()
    {
        return currentHP;
    }

    public virtual void QueueChargedAction(QueuedAction action)
    {
        isChargingAction = true;
        queuedAction = action;
    }

    public virtual void PerformChargedAction()
    {
        isChargingAction = false;
        queuedAction.Invoke();
        queuedAction = null;
    }

    public virtual ActionData GetQueuedActionData()
    {
        if(queuedAction == null)
            return null;
        
        return queuedAction.data;
    }

    public virtual string GetCurrentActionDescription()
    {
        return queuedAction.data.GetAbilityPerformedDescription(queuedAction.source, queuedAction.target);
    }

    public void StartTurn()
    {
        if(statusDictionary.ContainsKey(StatusTrigger.TurnStart))
        {
            List<Status> statuses = new List<Status>(statusDictionary[StatusTrigger.TurnStart]);
            foreach(Status status in statuses)
            {
                if(status.StatusHasEnded())
                {
                    statusDictionary[StatusTrigger.TakeDamage].Remove(status);
                }
                else if(status is ModifyCreatureStatus)
                {
                    ( (ModifyCreatureStatus)status ).ModifyCreature(this);
                }
                else if(status is GenericStatus)
                {
                    ( (GenericStatus)status ).PerformStatus();
                }
                else
                {
                    Debug.LogError("Status " + status + " had StatusTrigger TakeDamage, which is incompatible with Status type " + status.GetType());
                }
            }
        }

        foreach(StatusTrigger trigger in statusDictionary.Keys)
        {
            foreach(Status status in new List<Status>(statusDictionary[trigger]))
            {
                if(status.endTurn < 0)
                {
                    statusDictionary[trigger].Remove(status);
                }
            }
        }
    }

    public void ApplyStatus(Status status)
    {
        StatusTrigger trigger = status.GetStatusTrigger();
        if(!statusDictionary.ContainsKey(trigger))
        {
            statusDictionary.Add(trigger, new List<Status>());
        }

        statusDictionary[trigger].Add(status);
    }
}

