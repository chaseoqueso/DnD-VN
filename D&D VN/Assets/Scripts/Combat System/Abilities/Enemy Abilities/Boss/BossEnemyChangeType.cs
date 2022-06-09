using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Enemy Change Type", menuName = "Combat Data/Attacks/Boss Enemy Change Type")]
public class BossEnemyChangeType : ActionData
{
    public override QueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target)
    {
        QueuedAction action = new QueuedAction(this, source, target);
        action.AddListener(() => 
        {
            ( (BossEnemyInstance)source ).ChangeType();
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target)
    {
        string descString = source.GetDisplayName() + "'s type is obscured by ink. ";

        return descString;
    }
}
