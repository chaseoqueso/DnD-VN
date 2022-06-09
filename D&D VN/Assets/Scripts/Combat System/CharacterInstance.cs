using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstance : CreatureInstance
{
    protected int currentSkillPoints;

    protected new CharacterQueuedAction queuedAction { 
        get { return (CharacterQueuedAction)base.queuedAction; }
        set { base.queuedAction = value; }
    }

    protected new CharacterCombatData data
    {
        get { return (CharacterCombatData) _data; }
        set { _data = value; }
    }

    public CharacterInstance(CharacterCombatData characterData, int maxHP) : base()
    {
        data = characterData;
        currentHP = maxHP;

        currentSkillPoints = 3; // TODO: don't hardcode this, i just needed this here for testing
    }

    public int GetCurrentSkillPoints()
    {
        return currentSkillPoints;
    }

    public override bool DealDamage(DamageData damage)
    {
        bool alive = base.DealDamage(damage);
        UIManager.instance.combatUI.UpdateCharacterPanelValuesForCharacterWithID(this);

        if(data.EntityID == EntityID.MainCharacter && !alive){
            UIManager.instance.ToggleMainCharacterDeathPanelInCombat(true);
        }
        else if(data.EntityID == EntityID.Aeris){
            GameManager.instance.aerisDead = !alive;
        }
        else if(data.EntityID == EntityID.Samara){
            GameManager.instance.samaraDead = !alive;
        }

        return alive;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        UIManager.instance.combatUI.AssignActiveCharacter(this);
    }

    public override string GetCurrentActionDescription()
    {
        if(queuedAction == null)
            return "";

        return queuedAction.data.GetAbilityPerformedDescription(queuedAction.source, queuedAction.target, queuedAction.chargePercent);
    }

    public CharacterActionData GetBasicAttack() {return data.BasicAttack;}
    public CharacterActionData GetBasicGuard() {return data.BasicGuard;}
    public CharacterActionData GetAction1() {return data.Action1;}
    public CharacterActionData GetAction2() {return data.Action2;}
    public CharacterActionData GetAction3() {return data.Action3;}
    public CharacterActionData GetSpecial1() {return data.Special1;}
    public CharacterActionData GetSpecial2() {return data.Special2;}
    public CharacterActionData GetSpecial3()  {return data.Special3;}

    public CharacterActionData GetActionFromButtonType(ActionButtonType actionType)
    {
        return data.GetActionFromButtonType(actionType);
    }
}

