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
        currentHP -= GetDamageAmount(damage);
        if(currentHP < 0)
        {
            currentHP = 0;
        }

        Debug.Log(data.EntityID.ToString() + " took " + damage.damageAmount * (1 - data.Defense/100) + " damage.");

        return IsAlive();
    }

    public virtual float GetDamageAmount(DamageData damage, bool capAtCurrentHP = true)
    {
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
    }

    public virtual string GetCurrentActionDescription()
    {
        return queuedAction.data.GetAbilityPerformedDescription(queuedAction.source, queuedAction.target);
    }
}

