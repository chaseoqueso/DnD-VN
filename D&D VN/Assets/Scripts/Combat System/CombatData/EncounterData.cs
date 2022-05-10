using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter Data", menuName = "Combat Data/Encounter Data")]
public class EncounterData : ScriptableObject
{
    public List<EnemyCombatData> enemies;
}
