using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Combat Data/Enemy Data")]
public class EnemyCombatData : CreatureCombatData
{
    public bool SlowFirstTurn { get {return slowFirstTurn;} }
    public DamageType DamageType { get {return damageType;} }
    public Sprite Portrait { get {return portrait;} }
    public Sprite SecretPortrait { get {return secretPortrait;} }
    public List<ActionData> Actions { get {return actions;} }
    public string SecretName { get {return secretName;} }
    public string SecretDescription { get {return secretDescription;} }

    [Header("Enemy Data")]
    [SerializeField] [Tooltip("Whether this enemy should have half speed for the initial timeline placement.")]
    protected bool slowFirstTurn = true;

    [SerializeField] protected DamageType damageType;
    [Tooltip("Default portrait")]
    [SerializeField] protected Sprite portrait;
    [Tooltip("Portrait that unlocks once you examine")]
    [SerializeField] protected Sprite secretPortrait;
    [SerializeField] protected List<ActionData> actions;

    [SerializeField] [Tooltip("The hidden name you unlock if you inspect this creature w/ Aeris' ability.")]
    protected string secretName;

    [SerializeField] [Tooltip("The hidden description you unlock if you inspect this creature w/ Aeris' ability.")] [TextArea]
    protected string secretDescription;
}
