using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Lifesteal Attack", menuName = "Combat Data/Attacks/Boss Lifesteal Attack")]
public class BossLifestealAttack : ChargeableActionData
{
    [Header("Lifesteal Attack Properties")]
    [SerializeField] [Tooltip("The type of damage to deal with this attack.")]
    protected DamageType damageType;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack.")]
    protected float damageMultiplier = 1;
    [SerializeField] [Tooltip("The amount of the attack's damage to heal with this attack.")]
    private float healMultiplier = 0.5f;

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => {
            DamageData damage = calculateDamage(source, chargePercent, true);
            source.Heal(calculateLifestealAmount(target, damage, chargePercent), false);
            target.DealDamage(damage);
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        DamageData damage = calculateDamage(source, chargePercent);
        string damageDescription = source.GetDisplayName() + " dealt " + target.CalculateDamageTaken(damage) + " damage to " + target.GetDisplayName() + ". ";

        float healAmount = calculateLifestealAmount(target, damage, chargePercent);
        if(source.GetCurrentHealth() + healAmount > source.GetMaxHP())
        {
            healAmount = source.GetMaxHP() - source.GetCurrentHealth();
        }

        return damageDescription + "\n" + source.GetDisplayName() + " healed for " + healAmount + " hit points.";
    }

    protected DamageData calculateDamage(CreatureInstance source, float chargePercent, bool endOneTimeStatuses = false)
    {
        DamageData damage = new DamageData(source.GetBaseDamage() * damageMultiplier, damageType);
        damage = source.TriggerStatuses(StatusTrigger.DealDamage, damage, endOneTimeStatuses);
        return damage;
    }

    private int calculateLifestealAmount(CreatureInstance target, DamageData damage, float chargePercent)
    {
        return Mathf.CeilToInt(target.CalculateDamageTaken(damage) * healMultiplier);
    }
}
