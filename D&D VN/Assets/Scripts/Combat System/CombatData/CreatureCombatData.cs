using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureCombatData : ScriptableObject
{
    public EntityID EntityID { get {return entityID;} }
    public float TurnLength { get {return 100/speed;} }
    public float BaseDamage { get {return baseDamage;} }
    public float MaxHP { get {return maxHP;} }
    public float Speed { get {return speed;} }
    public float Defense { get {return defense;} }
    public string Description { get {return description;} }

    [Header("Generic Entity Stats")]
    [SerializeField] [Tooltip("The ID that respresents which entity this combat data belongs to.")]
    private EntityID entityID;

    [SerializeField] [Range(0, 100)] [Tooltip("The base amount of damage this entity's attacks deal. Simple Actions and Special Skills also scale off of this value.")]
    private float baseDamage = 10f;

    [SerializeField] [Range(0, 100)] [Tooltip("The maximum amount of HP this entity can have. Also determines the amount of HP this entity starts with.")]
    private float maxHP = 100f;

    [SerializeField] [Range(0, 100)] [Tooltip("Higher speed decreases the amount of time between turns. Affects charged abilities as well.")]
    private float speed = 50f;

    [SerializeField] [Range(0, 100)] [Tooltip("The amount that incoming damage is reduced, as a percentage between 0 and 100.")]
    private float defense = 0f;

    [SerializeField] [Tooltip("The description that shows up when you hover over this creature.")] [TextArea]
    private string description;
}
