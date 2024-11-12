using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[System.Serializable]
public class Character
{
    // Serialize Fields
    [SerializeField] CharacterData chrData;

    // Properties
    public CharacterData ChrData {get {return chrData;} set {chrData = value;} }
    public Stats ChrStats {get; set;} 
    public List<StatusInstance> Statuses { get; private set;} = new List<StatusInstance>();
    public Dictionary<StatType, int> StatChanges { get; private set; }
  
    // Variables
    public event System.Action OnStatusChanged;

    // Character Initalisation
    public void SetupCharacter()
    {
        // Setup Variables
        ChrStats = new Stats();

        // Setup Function
        LoadStats();
        ClearStatChanges();
    }

    // Do Methods
    public void LoadStats()
    {
        ChrStats.SetStatValue(StatType.MaxHealth, ChrData.MaxHealth);
        ChrStats.SetStatValue(StatType.Health, ChrData.Health);
        ChrStats.SetStatValue(StatType.Attack, ChrData.Attack);
        ChrStats.SetStatValue(StatType.Defense, ChrData.Defense);
        ChrStats.SetStatValue(StatType.Speed, ChrData.Speed);
    }

    public void FullHeal()
    {
        ChrStats.SetStatValue(StatType.Health, ChrStats.GetStatValue(StatType.MaxHealth));
    }
    
    // Character Check Methods
    public bool isAlive()
    {
        return this.ChrStats.GetStatValue(StatType.Health) > 0;
        
    }

    public bool Advantage(Character target)
    {
        return TypeChart.HasAdvantage(this.ChrData.Type, target.ChrData.Type);
    }

    // Character Request Methods
    public void UpdateStats()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            if (ChrStats.HasStat(statType))
            {
                ChrStats.SetStatValue(statType, ChrStats.GetStatValue(statType) * StatChanges[statType]);
            }
        }
    }

    public Damage SendAttack(Character target)
    {
        Damage damage = new Damage();

        // Calculate Modifiers    
        damage.IsCrit = UnityEngine.Random.value <= ChrData.CritChance;
        damage.CriticalAmount =  damage.IsCrit ? 1.5f : 1;
        damage.HasAdvantage = Advantage(target);
        damage.HasDisAdvantage = target.Advantage(this);
        damage.TypeAmount =  damage.HasDisAdvantage ? 0.5f : damage.HasAdvantage ? 1.5f : 1;

        // Calculate Damage
        damage.Amount = (int)((ChrStats.GetStatValue(StatType.Attack) + UnityEngine.Random.Range(0, (Mathf.Log10(ChrStats.GetStatValue(StatType.Attack)) + 1) * 10)) * damage.TypeAmount * damage.CriticalAmount);
        target.ReceiveAttack(this, damage);
        return damage;
    }

    public void ReceiveAttack(Character target, Damage damage)
    {
        int damageFinal = Mathf.Max(0, damage.Amount - ChrStats.GetStatValue(StatType.Defense));
        damage.Amount = damageFinal;
        ChrStats.SetStatValue(StatType.Health, Mathf.Clamp(ChrStats.GetStatValue(StatType.Health) - damageFinal, 0, ChrStats.GetStatValue(StatType.MaxHealth)));
    }

    // Apply Methods
    public void ApplyStatChanges(StatModifier baseStatChange)
    {        
        if (StatChanges.ContainsKey(baseStatChange.baseStat))
            StatChanges[baseStatChange.baseStat] *= (int)(baseStatChange.changeType == IncrementType.Additive ? (ChrStats.GetStatValue(baseStatChange.baseStat) + baseStatChange.change)/(ChrStats.GetStatValue(baseStatChange.baseStat)) : baseStatChange.change);
        else
            StatChanges[baseStatChange.baseStat] = (int)baseStatChange.change;

        Debug.Log($"{baseStatChange.baseStat} increased by {baseStatChange.change}");
        

        UpdateStats();
        Debug.Log($"{baseStatChange.baseStat} is now at {ChrStats.GetStatValue(baseStatChange.baseStat)}");
    }

    public void ClearStatChanges()
    {
        StatChanges = new Dictionary<StatType, int>();
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            StatChanges[statType] = 1; 
        }
        UpdateStats();
    }

    public void AddStatus(StatusInstance statusInst)
    {
        Statuses.Add(statusInst);        
        OnStatusChanged();
    }

    public void RemoveStatus(StatusInstance statusInst)
    {
        Statuses.Remove(statusInst);        
        OnStatusChanged();
    }

    public void ClearStatuses()
    {
        Statuses = new List<StatusInstance>();
        //OnStatusChanged();
    }
}

public class Damage
{
    public int Amount {get; set;}
    public float CriticalAmount {get; set;}
    public bool IsCrit {get; set;}
    public float TypeAmount {get; set;}
    public bool HasAdvantage {get; set;}
    public bool HasDisAdvantage {get; set;}
}
