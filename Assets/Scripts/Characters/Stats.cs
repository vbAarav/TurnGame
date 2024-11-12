using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    // Variables
    public Dictionary<StatType, int> stats = new Dictionary<StatType, int>();

    // Methods
    public bool HasStat(StatType statType)
    {
        return stats.ContainsKey(statType);
    }

    public int GetStatValue(StatType statType)
    {
        return stats[statType];
    }

    public void AddStatType(StatType statType)
    {
        if (!HasStat(statType))
            stats[statType] = 0;
    }

    public void SetStatValue(StatType statType, int value)
    {
        stats[statType] = value;
    }
}

// Large Data for All Character Stats
public enum CharacterType
{
    NONE,
    RED,
    BLUE,
    GREEN,
    YELLOW,
    PURPLE
}

public enum StatType
{
    MaxHealth,
    Health,
    Attack,
    Defense,
    Speed
}

public enum IncrementType
{
    Additive,
    Multiply
}

[System.Serializable]
public class StatModifier
{
    public StatType baseStat;
    public float change;
    public IncrementType changeType;
    public int duration;
    public EffectTarget target;
}