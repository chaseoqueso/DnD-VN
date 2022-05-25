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
        healAmount = TriggerStatuses(StatusTrigger.ReceiveHealing, healAmount);

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
        damage = TriggerStatuses(StatusTrigger.TakeDamage, damage);

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
        action = TriggerStatuses(StatusTrigger.QueueAction, action);

        isChargingAction = true;
        queuedAction = action;
    }

    public virtual void PerformChargedAction()
    {
        queuedAction = TriggerStatuses(StatusTrigger.PerformAction, queuedAction);

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
        if(queuedAction == null)
            return "";

        return queuedAction.data.GetAbilityPerformedDescription(queuedAction.source, queuedAction.target);
    }

    public virtual void StartTurn()
    {
        TriggerStatuses(StatusTrigger.TurnStart);

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

    protected void TriggerStatuses(StatusTrigger trigger)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            List<Status> statuses = new List<Status>(statusDictionary[trigger]);
            foreach(Status status in statuses)
            {
                if(status.StatusHasEnded())
                {
                    statusDictionary[trigger].Remove(status);
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
    }

    protected DamageData TriggerStatuses(StatusTrigger trigger, DamageData damage)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            TriggerStatuses(trigger);

            List<ModifyDamageStatus> statuses = statusDictionary[trigger].FindAll((Status status) => status is ModifyDamageStatus).ConvertAll(new System.Converter<Status, ModifyDamageStatus>((Status status) => (ModifyDamageStatus)status));
            foreach(ModifyDamageStatus status in statuses)
            {
                damage = status.ModifyDamage(damage);
            }
        }

        return damage;
    }

    protected QueuedAction TriggerStatuses(StatusTrigger trigger, QueuedAction action)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            TriggerStatuses(trigger);

            List<ModifyActionStatus> statuses = statusDictionary[trigger].FindAll((Status status) => status is ModifyActionStatus).ConvertAll(new System.Converter<Status, ModifyActionStatus>((Status status) => (ModifyActionStatus)status));
            foreach(ModifyActionStatus status in statuses)
            {
                action = status.ModifyAction(action);
            }
        }

        return action;
    }

    protected float TriggerStatuses(StatusTrigger trigger, float healAmount)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            TriggerStatuses(trigger);

            List<ModifyHealingStatus> statuses = statusDictionary[trigger].FindAll((Status status) => status is ModifyHealingStatus).ConvertAll(new System.Converter<Status, ModifyHealingStatus>((Status status) => (ModifyHealingStatus)status));
            foreach(ModifyHealingStatus status in statuses)
            {
                healAmount = status.ModifyHealing(healAmount);
            }
        }

        return healAmount;
    }
}

