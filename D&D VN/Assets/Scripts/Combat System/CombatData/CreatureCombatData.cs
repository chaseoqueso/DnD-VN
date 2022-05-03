using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureCombatData : ScriptableObject
{
    [Header("Generic Entity Stats")]

    [Range(0, 100)] [Tooltip("The base amount of damage this entity's attacks deal. Simple Actions and Special Skills also scale off of this value.")]
    public float baseDamage = 10f;

    [Range(0, 100)] [Tooltip("The maximum amount of HP this entity can have. Also determines the amount of HP this entity starts with.")]
    public float maxHP = 100f;

    [Range(0, 100)] [Tooltip("Higher speed decreases the amount of time between turns. Affects charged abilities as well.")]
    public float speed = 25f;

    [Range(0, 100)] [Tooltip("The amount that incoming damage is reduced, as a percentage between 0 and 100.")]
    public float defense = 0f;

    public float turnLength
    {
        get { return 100/speed; }
    }
}
