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
        CombatUI combatUI = UIManager.instance.combatUI;
        action.AddListener(() => 
        {
            // SUMMON A RANDOM ENEMY
            EnemyCombatData enemyData = enemyDatas[Random.Range(0, enemyDatas.Count)];
            EnemyInstance enemy = new EnemyInstance(enemyData, enemyData.MaxHP);

            TurnManager.Instance.InsertEnemy(0, enemy);
            TurnManager.Instance.turnOrder.Enqueue(enemy, TurnManager.Instance.currentTurn + enemyData.TurnLength);

            combatUI.AddEntityToTimeline(enemy);
            combatUI.SpawnEnemyAtPosition(0, enemy.GetPortrait(), enemy.GetIcon(), enemy.GetDescription(), enemy.GetMaxHP());
            
            // SUMMON ANOTHER
            enemyData = enemyDatas[Random.Range(0, enemyDatas.Count)];
            enemy = new EnemyInstance(enemyData, enemyData.MaxHP);

            TurnManager.Instance.InsertEnemy(2, enemy);
            TurnManager.Instance.turnOrder.Enqueue(enemy, TurnManager.Instance.currentTurn + enemyData.TurnLength);

            combatUI.AddEntityToTimeline(enemy);
            combatUI.SpawnEnemy(2, enemy.GetPortrait(), enemy.GetIcon(), enemy.GetDescription(), enemy.GetMaxHP());

        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string descString = source.GetDisplayName() + " summoned minions to assist. ";

        return descString;
    }
}
