using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIStates/Skeleton/Chasing", fileName = "SkeletonChasingState")]
public class SkeletonChasingState : AIState
{
    [SerializeField] int visionRange;

    public override bool UpdateState(UnitManager context)
    {
        var thisUnit = context.ActiveUnit;
        var position = thisUnit.TilePosition;

        var closestEnemy = FindClosestEnemyUnit(context, thisUnit, position);

        if (closestEnemy == null)
        {
            return false;
        }

        var movePos = AStar.AStarSearch(context, visionRange, position, closestEnemy.TilePosition);

        if (movePos == null) return false;

        context.ExecuteAIAction(ActionRegister.Index<MoveAction>(), movePos.Value);

        return true;
    }

    private Unit FindClosestEnemyUnit(UnitManager context, Unit thisUnit, Vec2 from)
    {
        for (int i = 1; i <= visionRange; i++)
        {
            var unitsInCircle = context.FindUnitAroundCircle(from, i);

            foreach (var unit in unitsInCircle)
            {
                if (unit.Team != thisUnit.Team)
                {
                    return unit;
                }
            }
        }

        return null;
    }

    
}