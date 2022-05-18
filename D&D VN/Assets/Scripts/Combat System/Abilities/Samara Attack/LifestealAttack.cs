using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lifesteal Attack", menuName = "Combat Data/Attacks/Lifesteal Attack")]
public class LifestealAttack : BasicAttack
{
    [Header("Lifesteal Attack Properties")]
    [SerializeField] [Tooltip("The amount of the attack's damage to heal with this attack at 0% charge.")]
    private float minHealMultiplier = 0.5f;
    [SerializeField] [Tooltip("The amount of the attack's damage to heal with this attack at 100% charge.")]
    private float maxHealMultiplier = 0.5f;

    public override CharacterQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        DamageData damage = calculateDamage(source, chargePercent);
        action.AddListener(() => {
            source.Heal(calculateLifestealAmount(target, damage, chargePercent));
            target.DealDamage(damage);
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        string damageDescription = base.GetAbilityPerformedDescription(source, target, chargePercent);

        DamageData damage = calculateDamage(source, chargePercent);
        float healAmount = calculateLifestealAmount(target, damage, chargePercent);
        if(source.GetCurrentHealth() + healAmount > source.data.MaxHP)
        {
            healAmount = source.data.MaxHP - source.GetCurrentHealth();
        }

        return damageDescription + "\n" + source.GetDisplayName() + " healed for " + healAmount + " hit points.";
    }

    private float calculateLifestealAmount(CreatureInstance target, DamageData damage, float chargePercent)
    {
        return target.GetDamageAmount(damage) * Mathf.Lerp(minHealMultiplier, maxHealMultiplier, chargePercent);
    }
}
