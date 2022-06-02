using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AIStates/Idle", fileName = "IdleState")]
public class IdleState : AIState
{
    public override bool UpdateState(UnitManager context)
    {
        context.ExecuteAIAction(ActionRegister.Index<WaitAction>());

        return true;
    }
}