using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Combat Data/Character Data")]
public class CharacterCombatData : CreatureCombatData
{
    public ChargeBarData ChargeData { get {return chargeData;} }
    public CharacterActionData BasicAttack { get {return basicAttack;} }
    public CharacterActionData BasicGuard { get {return basicGuard;} }
    public CharacterActionData Action1 { get {return action1;} }
    public CharacterActionData Action2 { get {return action2;} }
    public CharacterActionData Action3 { get {return action3;} }
    public CharacterActionData Special1 { get {return special1;} }
    public CharacterActionData Special2 { get {return special2;} }
    public CharacterActionData Special3 { get {return special3;} }

    [Header("Character Data")]

    [SerializeField] [Tooltip("Describes the way that the charge bar should fill during this character's charged abilities.")]
    private ChargeBarData chargeData;

    [SerializeField] private CharacterActionData basicAttack;
    [SerializeField] private CharacterActionData basicGuard;
    [SerializeField] private CharacterActionData action1;
    [SerializeField] private CharacterActionData action2;
    [SerializeField] private CharacterActionData action3;
    [SerializeField] private CharacterActionData special1;
    [SerializeField] private CharacterActionData special2;
    [SerializeField] private CharacterActionData special3;
}
