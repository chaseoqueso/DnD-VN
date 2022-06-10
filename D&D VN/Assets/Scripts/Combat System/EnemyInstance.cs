using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstance : CreatureInstance
{
    private const float spareHealthThreshold = 0.25f;

    protected new EnemyCombatData data
    {
        get { return (EnemyCombatData) _data; }
        set { _data = value; }
    }

    public bool isRevealed { get; protected set; }
    public bool hasBeenCleansed { get; protected set; }
    protected DamageType type;

    public EnemyInstance(EnemyCombatData enemyData, int maxHP) : base()
    {
        data = enemyData;
        type = enemyData.DamageType;
        currentHP = maxHP;

        isRevealed = false;
        hasBeenCleansed = false;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        
        ActionData action = GetNextAction();
        CreatureInstance target = null;

        if(action.Target == TargetType.enemies)
        {
            while(target == null)
            {
                target = TurnManager.Instance.GetCharacter(Random.Range(0, 3));
                if(!target.IsAlive())
                    target = null;
            }
        }
        else
        {
            List<EnemyInstance> enemies = TurnManager.Instance.GetAllEnemies();
            target = enemies[Random.Range(0, enemies.Count)];
        }


        float turnDelay;
        if(action is ChargeableActionData)
        {
            ChargeableActionData chargeAction = (ChargeableActionData)action;
            QueueChargedAction(chargeAction.GetQueuedAction(this, target, 1));
            turnDelay = chargeAction.CalculateChargeDelay(this, 1);
        }
        else
        {
            QueueChargedAction(action.GetQueuedAction(this, target));
            turnDelay = 0;
        }

        TurnManager.Instance.RequeueCurrentTurn(turnDelay);
    }

    public override string GetDisplayName()
    {
        return isRevealed ? data.SecretName : data.DisplayName;
    }

    public virtual DamageType GetDamageType()
    {
        return data.DamageType;
    }

    public virtual Sprite GetPortrait()
    {
        return isRevealed ? data.SecretPortrait : data.Portrait;
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

    public virtual ActionData GetNextAction()
    {
        return data.Actions[Random.Range(0, data.Actions.Count)];
    }

    public void Reveal()
    {
        isRevealed = true;
        
        CombatUI combatUI = UIManager.instance.combatUI;
        int index = TurnManager.Instance.GetEnemyIndex(this);

        combatUI.UpdateEnemyDescriptionWithIndex(index, data.SecretDescription);
        combatUI.RevealHealthUIForEnemyWithIndex(index, isRevealed, data.EntityID == EntityID.DragonBoss);
        combatUI.UpdateEnemyPortraitWithIndex(index, GetPortrait());
    }

    public override void Cleanse(float duration)
    {
        if(currentHP <= GetMaxHP() * spareHealthThreshold)
        {
            // Tell something somewhere that this was cleansed
            hasBeenCleansed = true;
            TurnManager.Instance.RemoveEnemyFromBattlefield(this);
        }
        else
        {
            base.Cleanse(duration);
        }
    }
}

