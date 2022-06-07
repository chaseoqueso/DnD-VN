using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Haste", menuName = "Combat Data/Attacks/Haste")]
public class Haste : CharacterActionData
{
    [Header("Haste Properties")]
    [SerializeField] [Tooltip("The length of the status effect as a percent of the character's turn length.")]
    protected float effectLength = 1;

    public override CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => target.ApplyStatus(new SlowStatus(target, TurnManager.Instance.currentTurn + calculateLength((CharacterInstance)source, chargePercent), -1)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString;

        if(target.canReceiveStatuses)
        {
            descString = source.GetDisplayName() + " doubled " + target.GetDisplayName() + "'s speed. ";
        }
        else
        {
            descString = source.GetDisplayName() + " tried to double " + target.GetDisplayName() + "'s speed, but they were cleansed!";
        }

        return descString;
    }

    protected float calculateLength(CharacterInstance character, float chargePercent)
    {
        return effectLength * (0.5f + CalculateChargeDelay(character, chargePercent));
    }
}
