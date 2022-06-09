using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Bleed Attack", menuName = "Combat Data/Attacks/Enemy Bleed Attack")]
public class EnemyBleedAttack : ChargeableActionData
{
    [SerializeField] [Tooltip("The amount of the enemy's base damage to deal with this attack.")]
    private float damageMultiplier = 0.5f;

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargeAmount)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, chargeAmount);
        DamageData damage = calculateDamage(source);
        action.AddListener(() => 
        {
            target.DealDamage(damage);
            target.ApplyStatus(new BleedStatus(TurnManager.Instance.currentTurn + target.GetTurnLength() * 2, damage.damageAmount / 2));
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargeAmount)
    {
        string descString = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(calculateDamage(source)) + " damage to " + target.GetDisplayName() + ", causing them to bleed.";
        
        InterposeStatus interposeStatus = target.GetInterposeStatus();
        if(interposeStatus != null)
        {
            CreatureInstance interposer = interposeStatus.interposer;
            DamageData statusDamage = calculateDamage(source);
            statusDamage.damageAmount *= interposeStatus.damageRedirectPercent;

            descString += interposer.GetDisplayName() + " interposed, taking " + interposer.CalculateDamageTaken(statusDamage) + " damage.";
        }

        return descString;
    }

    private DamageData calculateDamage(CreatureInstance source)
    {
        return new DamageData(source.GetBaseDamage() * damageMultiplier, ( (EnemyInstance)source ).GetDamageType());
    }
}
