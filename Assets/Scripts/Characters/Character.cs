using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Character
{
    // Serialize Fields
    [SerializeField] Stats chrStats;

    // Properties
    public Stats ChrStats {get {return chrStats;} set {chrStats = value;} }
    public Dictionary<BaseStats, int> StatChanges { get; private set; }
  

    // Variables
    
    // Character Initalisation
    public void SetupCharacter()
    {
        StatChanges = new Dictionary<BaseStats, int>()
        {
            {BaseStats.MaxHealth, 0},
            {BaseStats.Health, 0},
            {BaseStats.Attack, 0},
            {BaseStats.Speed, 0},
            {BaseStats.Defense, 0},
        };
    }
    
    // Character Check Methods
    public bool isAlive()
    {
        return this.ChrStats.Health > 0;
    }

    public bool Advantage(Character target)
    {
        return TypeChart.HasAdvantage(this.ChrStats.Type, target.ChrStats.Type);
    }

    // Character Request Methods
    public Damage SendAttack(Character target)
    {
        Damage damage = new Damage();

        // Calculate Modifiers    
        damage.IsCrit = Random.value <= ChrStats.CritChance;
        damage.CriticalAmount =  damage.IsCrit ? 1.5f : 1;
        damage.HasAdvantage = Advantage(target);
        damage.TypeAmount =  damage.HasAdvantage ? 1.5f : 1;

        // Calculate Damage
        damage.Amount = (int)((ChrStats.Attack + Random.Range(0, (Mathf.Log10(ChrStats.Attack) + 1) * 10)) * damage.TypeAmount * damage.CriticalAmount);
        target.ReceiveAttack(this, damage);
        return damage;
    }

    public void ReceiveAttack(Character target, Damage damage)
    {
        int damageFinal = damage.Amount - ChrStats.Defense;
        chrStats.Health = Mathf.Clamp(chrStats.Health - damageFinal, 0, chrStats.MaxHealth);
    }

    public void ApplyStatChanges(List<StatChange> statChanges)
    {
        foreach (var statChange in statChanges)
        {
            if (StatChanges.ContainsKey(statChange.baseStat))
                StatChanges[statChange.baseStat] += statChange.change;
            else
                StatChanges[statChange.baseStat] = statChange.change;

            Debug.Log($"{statChange.baseStat} increases by {statChange.change}");
        }
    }
}

public class Damage
{
    public int Amount {get; set;}
    public float CriticalAmount {get; set;}
    public bool IsCrit {get; set;}
    public float TypeAmount {get; set;}
    public bool HasAdvantage {get; set;}
}
