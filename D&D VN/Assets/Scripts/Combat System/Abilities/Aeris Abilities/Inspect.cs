using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inspect", menuName = "Combat Data/Attacks/Inspect")]
public class Inspect : CharacterActionData
{
    public new TargetType Target { get {return TargetType.enemies;} }

    public override QueuedAction PerformAction(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        QueuedAction action = new QueuedAction();
        action.AddListener( () => ( (EnemyInstance)target ).Reveal() );
        return action;
    }

    public override string GetAbilityPerformedDescription(CreatureInstance source, CreatureInstance target, float chargePercent)
    {
        EnemyInstance enemy = (EnemyInstance)target;
        return source.GetDisplayName() + " inspected " + enemy.GetDisplayName() + ", revealing its type to be " + enemy.data.DamageType + " and lowering its defenses!";
    }
}
