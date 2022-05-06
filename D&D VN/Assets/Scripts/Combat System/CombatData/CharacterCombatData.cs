using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Combat Data/Character Data")]
public class CharacterCombatData : CreatureCombatData
{
    public ChargeBarData ChargeData { get {return chargeData;} }
    public string SkillPointName { get {return skillPointName;} }
    public CharacterActionData BasicAttack { get {return basicAttack;} }
    public CharacterActionData BasicGuard { get {return basicGuard;} }
    public CharacterActionData Action1 { get {return action1;} }
    public CharacterActionData Action2 { get {return action2;} }
    public CharacterActionData Action3 { get {return action3;} }
    public CharacterActionData Special1 { get {return special1;} }
    public CharacterActionData Special2 { get {return special2;} }
    public CharacterActionData Special3 { get {return special3;} }

    public CharacterActionData GetActionFromButtonType(ActionButtonType actionType)
    {
        switch(actionType){
            case ActionButtonType.basicAttack:
                return basicAttack;
            case ActionButtonType.basicGuard:
                return basicGuard;
            case ActionButtonType.action1:
                return action1;
            case ActionButtonType.action2:
                return action2;
            case ActionButtonType.action3:
                return action3;
            case ActionButtonType.special1:
                return special1;
            case ActionButtonType.special2:
                return special2;
            case ActionButtonType.special3:
                return special3;
        }
        Debug.LogError("No action found for type " + actionType + " in character data " + EntityID);
        return null;
    }

    [Header("Character Data")]

    [SerializeField] [Tooltip("Describes the way that the charge bar should fill during this character's charged abilities.")]
    private ChargeBarData chargeData;
    
    [SerializeField] [Tooltip("The player-facing flavor name of this character's skill points.")]
    private string skillPointName;

    [SerializeField] private CharacterActionData basicAttack;
    [SerializeField] private CharacterActionData basicGuard;
    [SerializeField] private CharacterActionData action1;
    [SerializeField] private CharacterActionData action2;
    [SerializeField] private CharacterActionData action3;
    [SerializeField] private CharacterActionData special1;
    [SerializeField] private CharacterActionData special2;
    [SerializeField] private CharacterActionData special3;
}
