using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LargeSlice", menuName = "Skills/Large Slice")]
public class LargeSlice : SkillData
{
    public override void ActivateEffects(Character target)
    {
        Damage damage = new Damage();
        damage.Amount = 50;
        target.ReceiveAttack(damage);
    }
}
