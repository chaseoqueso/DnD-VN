using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstance : CreatureInstance
{
    public new EnemyCombatData data
    {
        get { return (EnemyCombatData) _data; }
        protected set { _data = value; }
    }

    public bool isRevealed { get; protected set; }
    protected DamageType type;

    public EnemyInstance(EnemyCombatData enemyData, float maxHP)
    {
        data = enemyData;
        currentHP = maxHP;
        isRevealed = false;
    }

    public override string GetDisplayName()
    {
        return isRevealed ? data.SecretName : data.DisplayName;
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

    public override float GetDamageAmount(DamageData damage, bool capAtCurrentHP = true)
    {
        float damageAmount = damage.damageAmount * (1 - data.Defense/100) * GetDamageEffectiveness(damage);

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
