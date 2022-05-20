using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Arcane Barrage", menuName = "Combat Data/Attacks/Arcane Barrage")]
public class ArcaneBarrage : BasicAttack
{
    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, null, chargePercent);
        DamageData damage = calculateDamage(source, chargePercent);
        foreach(EnemyInstance enemy in TurnManager.Instance.GetAllEnemies())
        {
            action.AddListener(() => enemy.DealDamage(damage));
        }
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        DamageData damage = calculateDamage(source, chargePercent);

        float damageAmount = damage.damageAmount;
        string descString = "";
        
        foreach(EnemyInstance enemy in TurnManager.Instance.GetAllEnemies())
        {
            descString += source.GetDisplayName() + " dealt " + enemy.CalculateDamageTaken(damage) + " damage to " + enemy.GetDisplayName() + ".\n";
        }

        return descString;
    }
}
