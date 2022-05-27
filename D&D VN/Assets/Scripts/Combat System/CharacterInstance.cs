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

    public new CharacterCombatData data
    {
        get { return (CharacterCombatData) _data; }
        protected set { _data = value; }
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
            UIManager.instance.ToggleGameOverPanel(true);
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
}

