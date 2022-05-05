using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "ScriptableObjects/Combat Data/Character Data")]
public class CharacterCombatData : CreatureCombatData
{
    public SpeakerID CharacterID { get {return characterID;} }

    [Header("Character Data")]
    [SerializeField] [Tooltip("The ID that respresents which character this combat data belongs to.")]
    private SpeakerID characterID;
}
