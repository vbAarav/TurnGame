using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillInstance
{
    [SerializeField] SkillData skillData;

    // Properties
    public SkillData SkillData {get {return skillData;} set {skillData = value;} }

    // Constructor
    public SkillInstance(SkillData skill)
    {
        SkillData = skill;
    }
}
