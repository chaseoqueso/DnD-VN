using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter Data", menuName = "ScriptableObjects/Combat Data/Encounter Data")]
public class EncounterData : ScriptableObject
{
    public List<EnemyCombatData> enemies;
}