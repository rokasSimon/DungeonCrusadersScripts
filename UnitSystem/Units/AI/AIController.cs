using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] AIState defaultState;
    [SerializeField] List<AIState> states;

    private AIState currentState;

    private void Start()
    {
        states.Sort((b, a) => a.priority.CompareTo(b.priority));
    }

    public void TakeTurn(UnitManager context)
    {
        if (currentState == null)
        {
            currentState = defaultState;
        }

        foreach (var state in states)
        {
            if (state.UpdateState(context))
            {
                Debug.Log(state.name);
                return;
            }
        }
    }
}
