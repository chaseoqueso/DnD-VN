using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstance : CreatureInstance
{
    protected new EnemyCombatData data
    {
        get { return (EnemyCombatData) _data; }
        set { _data = value; }
    }

    public bool isRevealed { get; protected set; }
    protected DamageType type;

    public EnemyInstance(EnemyCombatData enemyData, int maxHP) : base()
    {
        data = enemyData;
        type = enemyData.DamageType;
        currentHP = maxHP;
        isRevealed = false;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        
        ActionData action = GetNextAction();
        CharacterInstance target = null;
        while(target == null)
        {
            target = TurnManager.Instance.GetCharacter(Random.Range(0, 3));
            if(!target.IsAlive())
                target = null;
        }

        QueueChargedAction(action.GetQueuedAction(this, target));
        TurnManager.Instance.RequeueCurrentTurn(0);
    }

    public override string GetDisplayName()
    {
        return isRevealed ? data.SecretName : data.DisplayName;
    }

    public DamageType GetDamageType()
    {
        return data.DamageType;
    }

    public Sprite GetPortrait()
    {
        return data.Portrait;
    }

    public float GetDamageEffectiveness(DamageData damage)
    {
        switch(damage.damageType)
        {
            case DamageType.Arcane:
                if(type == DamageType.Light)
                    return 2f;
                if(type == DamageType.Dark)
                    return 0.5f;
                return 1f;
            
            case DamageType.Light:
                if(type == DamageType.Dark)
                    return 2f;
                if(type == DamageType.Arcane)
                    return 0.5f;
                return 1f;
            
            case DamageType.Dark:
                if(type == DamageType.Arcane)
                    return 2f;
                if(type == DamageType.Light)
                    return 0.5f;
                return 1f;
            
            default:
                return 1f;
        }
    }

    public override bool DealDamage(DamageData damage)
    {
        bool alive = base.DealDamage(damage);

        if(!alive)
        {
            TurnManager.Instance.RemoveEnemyFromBattlefield(this);
        }
        else{
            UIManager.instance.combatUI.UpdateEnemyHealth(TurnManager.Instance.GetEnemyIndex(this), currentHP);
        }

        return alive;
    }

    public override int CalculateDamageTaken(DamageData damage, bool capAtCurrentHP = true, bool endOneTimeStatuses = false)
    {
        damage = TriggerStatuses(StatusTrigger.TakeDamage, damage, endOneTimeStatuses);

        int damageAmount = Mathf.RoundToInt(damage.damageAmount * (1 - data.Defense/100) * GetDamageEffectiveness(damage));

        if(capAtCurrentHP && damageAmount > currentHP)
            damageAmount = currentHP;

        return damageAmount;
    }

    public ActionData GetNextAction()
    {
        return data.Actions[Random.Range(0, data.Actions.Count)];
    }

    public void Reveal()
    {
        isRevealed = true;
        UIManager.instance.combatUI.UpdateEnemyDescriptionWithIndex(TurnManager.Instance.GetEnemyIndex(this), data.SecretDescription);
    }
}

