using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;

[CreateAssetMenu(menuName = "AIStates/Roaming", fileName = "RoamingState")]
public class RoamingState : AIState
{
    [SerializeField] int roamingDistance;

    private Vec2? destination;

    public override bool UpdateState(UnitManager context)
    {
        SetDestination(context);

        if (destination == null)
        {
            return false;
        }

        var step = AStar.AStarSearch(context, roamingDistance, context.ActiveUnit.TilePosition, destination.Value);

        if (step == null) return false;

        context.ExecuteAIAction(ActionRegister.Index<MoveAction>(), step.Value);

        return true;
    }

    private void SetDestination(UnitManager context)
    {
        var unit = context.ActiveUnit;
        var tile = unit.TilePosition;

        if (destination == null || (destination != null && tile == destination.Value))
        {
            for (int i = roamingDistance; i > 0; i--)
            {
                var possibleDestinations = context.FindEmptyAreaCircle(tile, i);

                if (possibleDestinations.Count != 0)
                {
                    destination = possibleDestinations.TakeRandom();
                }
            }
        }
    }
}