using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Combat Data/Enemy Data")]
public class EnemyCombatData : CreatureCombatData
{
    [Header("Enemy Data")]
    public DamageType damageType;
}
