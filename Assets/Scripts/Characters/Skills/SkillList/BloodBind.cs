using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodBind", menuName = "Skills/Blood Bind")]
public class BloodBind : SkillData
{
    public override void ActivateEffects(Character source, Character target)
    {   
        // Sacrfice Health
        int srcDmg = Mathf.Max(1, (int)(0.01f * source.ChrStats.GetStatValue(StatType.MaxHealth)));
        source.SetHealth(Mathf.Max(source.ChrStats.GetStatValue(StatType.Health) - srcDmg, 0));

    }
}
