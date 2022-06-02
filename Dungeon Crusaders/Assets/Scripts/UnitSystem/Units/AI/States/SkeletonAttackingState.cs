using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AIStates/Skeleton/Attacking", fileName = "SkeletonAttackingState")]
public class SkeletonAttackingState : AIState
{
    public override bool UpdateState(UnitManager context)
    {
        var thisUnit = context.ActiveUnit;
        var position = thisUnit.TilePosition;

        var adjacentUnits = context.FindUnitsAround(position, 1);

        foreach (var unit in adjacentUnits)
        {
            if (unit.Team != thisUnit.Team)
            {
                context.ExecuteAIAction(ActionRegister.Index<StrikeAction>(), unit.TilePosition);
                return true;
            }
        }

        return false;
    }
}