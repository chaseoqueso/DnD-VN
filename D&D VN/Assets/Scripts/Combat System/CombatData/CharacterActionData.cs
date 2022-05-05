using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterActionData : ActionData
{
    public string SkillName { get {return skillName;} }
    public string SkillDescription { get {return skillDescription;} }

    [Header("Character Action Properties")]
    [SerializeField] [Tooltip("The player-facing name of the action.")]
    private string skillName;

    [SerializeField] [TextArea] [Tooltip("The player-facing description of the action.")]
    private string skillDescription;
    
    public abstract void PerformAction(TurnManager.CreatureInstance source, TurnManager.CreatureInstance target, float chargePercent);

    public override void PerformAction(TurnManager.CreatureInstance source, TurnManager.CreatureInstance target)
    {
        PerformAction(source, target, 0);
    }
}
