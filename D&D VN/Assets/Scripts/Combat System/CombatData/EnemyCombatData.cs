using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Combat Data/Enemy Data")]
public class EnemyCombatData : CreatureCombatData
{
    public DamageType DamageType { get {return damageType;} }
    public Sprite Portrait { get {return portrait;} }
    public Sprite Icon { get {return icon;} }
    public List<ActionData> Actions { get {return actions;} }
    public string SecretDescription { get {return secretDescription;} }

    [Header("Enemy Data")]
    [SerializeField] private DamageType damageType;
    [SerializeField] private Sprite portrait;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<ActionData> actions;

    [SerializeField] [Tooltip("The hidden description you unlock if you inspect this creature w/ Aeris' ability.")] [TextArea]
    private string secretDescription;
}
