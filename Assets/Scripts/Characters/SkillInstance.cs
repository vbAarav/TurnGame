using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillInstance
{
    [SerializeField] Skill skillBase;

    // Properties
    public Skill SkillBase {get {return skillBase;} set {skillBase = value;} }

    // Constructor
    public SkillInstance(Skill skill)
    {
        SkillBase = skill;
    }
}
