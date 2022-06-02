using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public int priority;

    public abstract bool UpdateState(UnitManager context);
}