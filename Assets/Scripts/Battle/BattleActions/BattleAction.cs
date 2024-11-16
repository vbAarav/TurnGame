using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAction
{
    // Variables
    Character invoker;
    List<Character> targets;
    Character currentTarget;
    int damage;

    // Constructor
    public BattleAction(Character invoker, List<Character> targets)
    {
        this.invoker = invoker;
        this.targets = targets;
    }

    // Methods
    public abstract void ExecuteAction();
    
    public List<Character> GetTargets()
    {
        return targets;
    }
}
