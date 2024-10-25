using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Character/New Skill")]
public class Skill : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] SkillCategory category;
    [SerializeField] SkillEffects effects;
    

    public string Name { get{ return name; } set {name = value;}}
    public string Description { get{ return description; } set {description = value;}}
    public SkillCategory Category { get{ return category; } }
    public SkillEffects Effects { get{ return effects; } }
}

// Large Data for the Skills
[System.Serializable]
public class SkillEffects
{
    [SerializeField] List<StatChange> changes;
    [SerializeField] SkillTarget target;

    public List<StatChange> Changes { get { return changes; } }

    public SkillTarget Target { get { return target; } }
}

[System.Serializable]
public class StatChange
{
    public BaseStats baseStat;
    public int change;
}

public enum SkillCategory
{
    Attack,
    Buff,
    Debuff,
    Status
}

public enum SkillTarget
{
    Self,
    Other
}
