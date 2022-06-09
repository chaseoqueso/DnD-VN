using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Data", menuName = "Combat Data/Boss Data")]
public class BossEnemyCombatData : EnemyCombatData
{
    public new Sprite SecretPortrait { get {return null; }}
    public new string SecretName { get {return null; }}

    public Sprite ArcaneSecretPortrait { get {return arcaneSecretPortrait;} }
    public Sprite DarkSecretPortrait { get {return darkSecretPortrait;} }
    public Sprite LightSecretPortrait { get {return lightSecretPortrait;} }

    public string ArcaneSecretName { get {return arcaneSecretName;} }
    public string DarkSecretName { get {return darkSecretName;} }
    public string LightSecretName { get {return lightSecretName;} }

    [Tooltip("Portrait that unlocks once you examine")]
    [SerializeField] private Sprite arcaneSecretPortrait;
    [Tooltip("Portrait that unlocks once you examine")]
    [SerializeField] private Sprite darkSecretPortrait;
    [Tooltip("Portrait that unlocks once you examine")]
    [SerializeField] private Sprite lightSecretPortrait;

    [SerializeField] [Tooltip("The hidden name you unlock if you inspect this creature w/ Aeris' ability.")]
    protected string arcaneSecretName;
    [SerializeField] [Tooltip("The hidden name you unlock if you inspect this creature w/ Aeris' ability.")]
    protected string darkSecretName;
    [SerializeField] [Tooltip("The hidden name you unlock if you inspect this creature w/ Aeris' ability.")]
    protected string lightSecretName;
}
