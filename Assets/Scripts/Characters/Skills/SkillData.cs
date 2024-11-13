using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Character/New Skill")]
public class SkillData : ScriptableObject
{
    // Variables
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] SkillCategory category;
    [SerializeField] SkillEffects effects;
    
    // Properties
    public string Name { get{ return name; } set {name = value;}}
    public string Description { get{ return description; } set {description = value;}}
    public SkillCategory Category { get{ return category; } }
    public SkillEffects Effects { get{ return effects; } }

    // Methods
    public virtual void ActivateEffects(Character source, Character target) {}

}

// Large Data for the Skills
[System.Serializable]
public class SkillEffects
{
    [SerializeField] List<StatModifier> statChanges;
    [SerializeField] List<StatusInstance> statuses;

    public List<StatModifier> StatChanges { get{ return statChanges; } }
    public List<StatusInstance> Statuses { get{ return statuses; } }
}

public enum SkillCategory
{
    Attack,
    Buff,
    Debuff,
    Status
}

public enum EffectTarget
{
    Self,
    Other
}


