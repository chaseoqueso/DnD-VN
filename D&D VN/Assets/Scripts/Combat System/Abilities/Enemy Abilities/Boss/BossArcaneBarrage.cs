using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Arcane Barrage", menuName = "Combat Data/Attacks/Boss Arcane Barrage")]
public class BossArcaneBarrage : ChargeableActionData
{
    [Header("Attack Properties")]
    [SerializeField] [Tooltip("The type of damage to deal with this attack.")]
    protected DamageType damageType;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack at 0% charge.")]
    protected float minDamageMultiplier = 1;
    [SerializeField] [Tooltip("The amount of the character's base damage to deal with this attack at 100% charge.")]
    protected float maxDamageMultiplier = 2;

    public override ChargeableQueuedAction GetQueuedAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        ChargeableQueuedAction action = new ChargeableQueuedAction(this, source, null, chargePercent);
        action.AddListener(() => {
            DamageData damage = calculateDamage(source, chargePercent, true);
            for(int i = 0; i < 3; ++i)
            {
                CharacterInstance character = TurnManager.Instance.GetCharacter(i);
                
                if(character != null && character.IsAlive())
                    character.DealDamage(damage);
            }
        });
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        DamageData damage = calculateDamage(source, chargePercent);

        float damageAmount = damage.damageAmount;
        string descString = "";
        
        for(int i = 0; i < 3; ++i)
        {
            CharacterInstance character = TurnManager.Instance.GetCharacter(i);
            
            if(character != null && character.IsAlive())
                descString += source.GetDisplayName() + " dealt " + character.CalculateDamageTaken(damage) + " damage to " + character.GetDisplayName() + ".\n";
        }

        return descString;
    }

    private DamageData calculateDamage(CreatureInstance source, float chargePercent, bool endOneTimeStatuses = false)
    {
        DamageData damage = new DamageData(source.GetBaseDamage() * Mathf.Lerp(minDamageMultiplier, maxDamageMultiplier, chargePercent), damageType);
        damage = source.TriggerStatuses(StatusTrigger.DealDamage, damage, endOneTimeStatuses);
        return damage;
    }
}
