using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cleanse", menuName = "Combat Data/Attacks/Cleanse")]
public class Cleanse : CharacterActionData
{
    public new TargetType Target { get {return TargetType.any;} }

    [Header("Cleanse Properties")]
    [SerializeField] [Tooltip("The duration to prevent statuses at 0% charge.")]
    private float minStatusPreventionDuration = 0;
    [SerializeField] [Tooltip("The duration to prevent statuses at 100% charge.")]
    private float maxStatusPreventionDuration = 10;

    public override CharacterQueuedAction GetQueuedAction(CharacterInstance source, CreatureInstance target, float chargePercent)
    {
        CharacterQueuedAction action = new CharacterQueuedAction(this, source, target, chargePercent);
        action.AddListener(() => target.Cleanse(Mathf.Lerp(minStatusPreventionDuration, maxStatusPreventionDuration, chargePercent)));
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        if(target is BossEnemyInstance && ( (BossEnemyInstance)target ).hasBeenCleansed)
        {
            foreach(EnemyInstance enemy in TurnManager.Instance.GetAllEnemies())
            {
                if(enemy != null)
                {
                    enemy.ClearStatuses();
                    enemy.DealDamage(new DamageData(enemy.GetCurrentHealth() - 1, DamageType.Neutral));
                    enemy.Cleanse(0);
                }
            }

            return source.GetDisplayName() + " cleansed the Library Guardians of all their impurities.";
        }
        else if(target is EnemyInstance && ( (EnemyInstance)target ).hasBeenCleansed)
        {
            return source.GetDisplayName() + " cleansed the Lesser Guardian of its impurities.";
        }
        else
        {
            return source.GetDisplayName() + " cleansed " + target.GetDisplayName() + " of all status effects.";
        }
    }
}
