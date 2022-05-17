using System.Collections.Generic;

public static class StatusRegister
{
    private static readonly List<IStatusEffect> _statusEffects = new()
    {
        
    };

    public static IStatusEffect Get(int idx)
    {
        return _statusEffects[idx];
    }

    public static int Index<T>() where T : IStatusEffect
    {
        for (int i = 0; i < _statusEffects.Count; i++)
        {
            if (_statusEffects[i] is T)
            {
                return i;
            }
        }

        return -1;
    }
}