using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Enemy Summon", menuName = "Combat Data/Attacks/Boss Enemy Summon")]
public class BossEnemySummon : ChargeableActionData
{
    [SerializeField] [Tooltip("The amount of the enemy's base damage to deal with this attack.")]
    private List<EnemyCombatData> enemyDatas = new List<EnemyCombatData>();

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => 
        {
            // SUMMON TWO RANDOM ENEMIES
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString = source.GetDisplayName() + " summoned minions to assist. ";

        return descString;
    }
}
