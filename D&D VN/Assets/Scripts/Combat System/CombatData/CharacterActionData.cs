using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Action", menuName = "ScriptableObjects/Combat Data/Character Action")]
public class CharacterActionData : ActionData
{
    public string SkillName { get {return skillName;} }
    public string SkillDescription { get {return skillDescription;} }

    [SerializeField] [Tooltip("The player-facing name of the action.")]
    private string skillName;

    [SerializeField] [TextArea] [Tooltip("The player-facing description of the action.")]
    private string skillDescription;

    public override void PerformAction(TurnManager.CreatureInstance source, TurnManager.CreatureInstance target)
    {
        // TODO
    }
}
