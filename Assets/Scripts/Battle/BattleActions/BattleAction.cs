using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAction
{
    // Variables
    protected Character invoker;
    protected List<Character> targets;
    protected Character currentTarget;
    protected int damage;

    // Constructors
    public BattleAction(Character invoker, List<Character> targets)
    {
        this.invoker = invoker;
        this.targets = targets;
    }

    public BattleAction(Character invoker, Character target)
    {
        this.invoker = invoker;
        this.currentTarget = target;
    }

    // Methods
    public abstract Damage ExecuteAction();
    
    public List<Character> GetTargets()
    {
        return targets;
    }

    public void SetNewTarget()
    {
        currentTarget = targets[UnityEngine.Random.Range(0, targets.Count)];
    }
}

// Attack
public class AttackAction : BattleAction
{
    public AttackAction(Character invoker, Character target) : base(invoker, target) {}

    public override Damage ExecuteAction()
    {
        Damage damage = invoker.SendAttack(this.currentTarget);
        return damage;
    }

}
