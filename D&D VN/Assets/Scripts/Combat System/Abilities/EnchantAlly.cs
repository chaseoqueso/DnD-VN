using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enchant Ally", menuName = "Combat Data/Attacks/Enchant Ally")]
public class EnchantAlly : CharacterActionData
{
    [Header("Enchant Ally Properties")]
    [SerializeField] [Tooltip("The type of damage to enchant the ally with.")]
    protected DamageType damageType;
    [SerializeField] [Tooltip("The base length of the status effect.")]
    protected float baseEffectLength = 1;
    [SerializeField] [Tooltip("The percent of the base length the status effect will last for at 0% charge.")]
    protected float minEffectLengthMultiplier = 1;
    [SerializeField] [Tooltip("The percent of the base length the status effect will last for at 100% charge.")]
    protected float maxEffectLengthMultiplier = 2;

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => target.ApplyStatus(new TypeChangeStatus(TurnManager.Instance.currentTurn + calculateLength(chargePercent), damageType)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        Debug.Log("Getting description.");
        return source.GetDisplayName() + " enchanted " + target.GetDisplayName() + "'s next attack with " + damageType.ToString() + " energy.";
    }

    protected float calculateLength(float chargePercent)
    {
        return baseEffectLength * Mathf.Lerp(minEffectLengthMultiplier, maxEffectLengthMultiplier, chargePercent);
    }
}
