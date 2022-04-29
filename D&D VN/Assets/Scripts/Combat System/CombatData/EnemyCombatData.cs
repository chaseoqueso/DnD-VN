using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/Combat Data")]
public class EnemyCombatData : CreatureCombatData
{
    [Header("Enemy Data")]
    DamageType damageType;
}
