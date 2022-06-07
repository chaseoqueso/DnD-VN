using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoulder Check", menuName = "Combat Data/Attacks/Shoulder Check")]
public class ShoulderCheck : BasicAttack
{
    [Header("Shoulder Check Properties")]
    [SerializeField] [Tooltip("The percent of the character's turn length to delay the target.")]
    protected float delayMultiplier = 1;

    public override CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => 
        { 
            target.DealDamage(calculateDamage(source, chargePercent, true)); 
            TurnManager.Instance.RequeueCreature(target, calculateDelay((CharacterInstance)source, chargePercent));
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString = base.GetAbilityPerformedDescription(source, target, chargePercent);
        descString += target.GetDisplayName() + "'s turn was delayed.";
        return descString;
    }

    protected float calculateDelay(CharacterInstance source, float chargePercent)
    {
        return source.GetTurnLength() * (0.5f + Mathf.Lerp(MinChargeLengthMultiplier, MaxChargeLengthMultiplier, chargePercent)) * delayMultiplier;
    }
}
