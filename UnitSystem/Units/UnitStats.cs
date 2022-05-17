using UnityEngine;
using System;

[Serializable]
public class UnitStats
{
    private const float BASE_CRIT_CHANCE = 5f;

    private const float STR_AP_MULTIPLIER = 1f;
    private const float STR_HP_MULTIPLIER = 5f;

    private const float AGI_AP_MULTIPLIER = 1f;
    private const float AGI_CR_MULTIPLIER = 0.75f;

    private const float INT_SP_MULTIPLIER = 1.25f;
    private const float INT_MP_MULTIPLIER = 3.4f;

    public int Strength;
    public int Agility;
    public int Intelligence;

    public int FlatAP;
    public int FlatSP;

    [Tooltip("Should be at least 5x str")]
    public int CurrentHealth;
    [Tooltip("Should be at least 3.4x int")]
    public int CurrentMana;

    public int AttackRange;
    public int Initiative;
    public int Armor;

    public int AttackPower()
    {
        return (int)(FlatAP + Strength * STR_AP_MULTIPLIER + Agility * AGI_AP_MULTIPLIER);
    }
    public int SpellPower()
    {
        return (int) (FlatSP + Intelligence * INT_SP_MULTIPLIER);
    }
    public int MaxHealth()
    {
        return (int)(Strength * STR_HP_MULTIPLIER);
    }
    public int MaxMana()
    {
        return (int)(Intelligence * INT_MP_MULTIPLIER);
    }
    public float AttackCritChance()
    {
        return Mathf.Clamp01((Agility * AGI_CR_MULTIPLIER + BASE_CRIT_CHANCE) / 100f);
    }

    public UnitStats Clone()
    {
        return MemberwiseClone() as UnitStats;
    }
}
