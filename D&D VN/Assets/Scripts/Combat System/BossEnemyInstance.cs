using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemyInstance : EnemyInstance
{
    protected new BossEnemyCombatData data
    {
        get { return (BossEnemyCombatData) _data; }
        set { _data = value; }
    }

    public DamageType currentDamageType { get; private set; }

    public BossEnemyInstance(EnemyCombatData enemyData, int maxHP) : base(enemyData, maxHP)
    {
        isRevealed = false;

        switch (Random.Range(0, 3))
        {
            case 0:
                currentDamageType = DamageType.Arcane;
                break;
                
            case 1:
                currentDamageType = DamageType.Dark;
                break;
                
            case 2:
                currentDamageType = DamageType.Light;
                break;
        }
    }

    public override string GetDisplayName()
    {
        if(!isRevealed)
            return data.DisplayName;
        
        switch(currentDamageType)
        {
            case DamageType.Arcane:
                return data.ArcaneSecretName;
                
            case DamageType.Dark:
                return data.DarkSecretName;
                
            case DamageType.Light:
                return data.LightSecretName;

            default:
                return data.SecretName;
        }
    }

    public override DamageType GetDamageType()
    {
        return currentDamageType;
    }

    public override ActionData GetNextAction()
    {
        bool canSummon = TurnManager.Instance.GetAllEnemies().Count == 1;
        ActionData nextAction = null;

        // While we haven't picked an action yet, or we chose the summon attack and can't use it, or we chose the change type attack and can't use it, pick a new attack
        while(nextAction == null || (!canSummon && nextAction is BossEnemySummon) || (!isRevealed && nextAction is BossEnemyChangeType))
        {
            nextAction = data.Actions[Random.Range(0, data.Actions.Count)];
        }

        return nextAction;
    }

    public override Sprite GetPortrait()
    {
        if(!isRevealed)
            return data.Portrait;
        
        switch(currentDamageType)
        {
            case DamageType.Arcane:
                return data.ArcaneSecretPortrait;
                
            case DamageType.Dark:
                return data.DarkSecretPortrait;
                
            case DamageType.Light:
                return data.LightSecretPortrait;

            default:
                return data.SecretPortrait;
        }
    }

    public void ChangeType()
    {
        isRevealed = false;

        switch (Random.Range(0, 3))
        {
            case 0:
                currentDamageType = DamageType.Arcane;
                break;
                
            case 1:
                currentDamageType = DamageType.Dark;
                break;
                
            case 2:
                currentDamageType = DamageType.Light;
                break;
        }

        Debug.Log(currentDamageType);

        CombatUI combatUI = UIManager.instance.combatUI;
        int index = TurnManager.Instance.GetEnemyIndex(this);
        
        combatUI.UpdateEnemyDescriptionWithIndex(index, data.Description);
        combatUI.RevealHealthUIForEnemyWithIndex(index, false);
        combatUI.UpdateEnemyPortraitWithIndex(index, GetPortrait());
    }
}

