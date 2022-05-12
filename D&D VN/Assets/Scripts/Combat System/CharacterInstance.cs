using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstance : CreatureInstance
{
    protected int currentSkillPoints;

    public new CharacterCombatData data
    {
        get { return (CharacterCombatData) _data; }
        protected set { _data = value; }
    }

    public CharacterInstance(CharacterCombatData characterData, float maxHP)
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
}

