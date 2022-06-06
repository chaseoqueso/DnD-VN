using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inspect", menuName = "Combat Data/Attacks/Inspect")]
public class Inspect : CharacterActionData
{
    public new TargetType Target { get {return TargetType.enemies;} }
    public float MinChargeDamageMultiplier { get {return minChargeDamageMultiplier;} }
    public float MaxChargeDamageMultiplier { get {return maxChargeDamageMultiplier;} }

    [Header("Inspect Properties")]
    [SerializeField] [Tooltip("The amount to multiply damage against the target at 0% charge.")]
    private float minChargeDamageMultiplier = 1;
    [SerializeField] [Tooltip("The amount to multiply damage against the target at 100% charge.")]
    private float maxChargeDamageMultiplier = 2;


    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener( () => {
            ( (EnemyInstance)target ).Reveal();
            target.ApplyStatus(new InspectStatus(-1, getDamageMultiplier(chargePercent)));
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        EnemyInstance enemy = (EnemyInstance)target;

        string descString;
        if(target.canReceiveStatuses)
        {
            descString = source.GetDisplayName() + " inspected " + enemy.GetDisplayName() + ", revealing its type to be " + enemy.data.DamageType + " and lowering its defenses!";
        }
        else
        {
            descString = source.GetDisplayName() + " inspected " + enemy.GetDisplayName() + ", revealing its type to be " + enemy.data.DamageType + ". Its defenses couldn't be lowered because it was cleansed!";
        }

        return descString;
    }

    private float getDamageMultiplier(float chargePercent)
    {
        return Mathf.Lerp(minChargeDamageMultiplier, maxChargeDamageMultiplier, chargePercent);
    }
}
