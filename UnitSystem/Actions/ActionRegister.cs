using System.Collections.Generic;

public static class ActionRegister
{
    private static readonly List<UnitAction> _actions = new()
    {
        new MoveAction(),
        new WaitAction(),
        new StrikeAction(),
        new FireBallAction()
    };

    public static UnitAction Get(int idx)
    {
        return _actions[idx];
    }

    public static int Index<T>() where T: UnitAction
    {
        for (int i = 0; i < _actions.Count; i++)
        {
            if (_actions[i] is T)
            {
                return i;
            }
        }

        return -1;
    }
}