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

    public bool canReceiveStatuses = true;

    public bool isChargingAction { get; protected set; }
    protected QueuedAction queuedAction;

    protected CreatureCombatData _data;
    protected int currentHP;

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

    public virtual void Heal(int healAmount)
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
        currentHP -= CalculateDamageTaken(damage, endOneTimeStatuses: true);
        if(currentHP < 0)
        {
            currentHP = 0;
        }

        return IsAlive();
    }

    public virtual int CalculateDamageTaken(DamageData damage, bool capAtCurrentHP = true, bool endOneTimeStatuses = false)
    {
        damage = TriggerStatuses(StatusTrigger.TakeDamage, damage, endOneTimeStatuses);

        int damageAmount = Mathf.RoundToInt(damage.damageAmount * (1 - data.Defense/100));

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

    public bool ApplyStatus(Status status)
    {
        TriggerStatuses(StatusTrigger.ApplyStatus, true);

        if(!canReceiveStatuses && !(status is GuardStatus))
            return false;

        StatusTrigger trigger = status.GetStatusTrigger();
        if(!statusDictionary.ContainsKey(trigger))
        {
            statusDictionary.Add(trigger, new List<Status>());
        }

        Debug.Log("Status " + status + " added to " + GetDisplayName() + " with trigger " + trigger);

        statusDictionary[trigger].Add(status);

        return true;
    }

    public void Cleanse(float duration)
    {
        ClearStatuses();
        ApplyStatus(new CleanseStatus(this, TurnManager.Instance.currentTurn + duration));
        canReceiveStatuses = false;
    }

    public void ClearStatuses()
    {
        statusDictionary.Clear();
    }

    public void TriggerStatuses(StatusTrigger trigger, bool endOneTimeStatuses = false)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            List<Status> statuses = new List<Status>(statusDictionary[trigger]);
            foreach(Status status in statuses)
            {
                if(status.StatusHasEnded())
                {
                    statusDictionary[trigger].Remove(status);
                    Debug.Log("Status: " + status.ToString() + " removed from " + GetDisplayName() + ".");
                }
                else if(status is ModifyCreatureStatus)
                {
                    ( (ModifyCreatureStatus)status ).ModifyCreature(this, endOneTimeStatuses);
                }
                else if(status is GenericStatus)
                {
                    ( (GenericStatus)status ).PerformStatus(endOneTimeStatuses);
                }
            }
        }
    }

    public DamageData TriggerStatuses(StatusTrigger trigger, DamageData damage, bool endOneTimeStatuses = false)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            Debug.Log("Creature: " + GetDisplayName() + " Trigger: " + trigger + " End one time statuses: " + endOneTimeStatuses);
            TriggerStatuses(trigger, endOneTimeStatuses);

            List<ModifyDamageStatus> statuses = statusDictionary[trigger].FindAll((Status status) => status is ModifyDamageStatus).ConvertAll(new System.Converter<Status, ModifyDamageStatus>((Status status) => (ModifyDamageStatus)status));
            foreach(ModifyDamageStatus status in statuses)
            {
                damage = status.ModifyDamage(damage, endOneTimeStatuses);
            }
        }

        return damage;
    }

    public QueuedAction TriggerStatuses(StatusTrigger trigger, QueuedAction action, bool endOneTimeStatuses = false)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            TriggerStatuses(trigger, endOneTimeStatuses);

            List<ModifyActionStatus> statuses = statusDictionary[trigger].FindAll((Status status) => status is ModifyActionStatus).ConvertAll(new System.Converter<Status, ModifyActionStatus>((Status status) => (ModifyActionStatus)status));
            foreach(ModifyActionStatus status in statuses)
            {
                action = status.ModifyAction(action, endOneTimeStatuses);
            }
        }

        return action;
    }

    protected int TriggerStatuses(StatusTrigger trigger, int healAmount, bool endOneTimeStatuses = false)
    {
        if(statusDictionary.ContainsKey(trigger))
        {
            TriggerStatuses(trigger, endOneTimeStatuses);

            List<ModifyHealingStatus> statuses = statusDictionary[trigger].FindAll((Status status) => status is ModifyHealingStatus).ConvertAll(new System.Converter<Status, ModifyHealingStatus>((Status status) => (ModifyHealingStatus)status));
            foreach(ModifyHealingStatus status in statuses)
            {
                healAmount = status.ModifyHealing(healAmount, endOneTimeStatuses);
            }
        }

        return healAmount;
    }
}

