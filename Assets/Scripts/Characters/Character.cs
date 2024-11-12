using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Character
{
    // Serialize Fields
    [SerializeField] CharacterData chrData;

    // Properties
    public CharacterData ChrData {get {return chrData;} set {chrData = value;} }
    public List<StatusInstance> Statuses { get; private set;} = new List<StatusInstance>();
    public Dictionary<StatType, int> StatChanges { get; private set; }
  
    // Variables
    public event System.Action OnStatusChanged;

    // Character Initalisation
    public void SetupCharacter()
    {
        ClearStatChanges();
    }

    public void Update()
    {

    }
    
    // Character Check Methods
    public bool isAlive()
    {
        return this.ChrData.Health > 0;
    }

    public bool Advantage(Character target)
    {
        return TypeChart.HasAdvantage(this.ChrData.Type, target.ChrData.Type);
    }

    // Character Request Methods
    public void UpdateStats()
    {
        foreach (KeyValuePair<StatType, int> statChange in StatChanges)
        {
            switch (statChange.Key)
            {
                case StatType.MaxHealth:
                    ChrData.MaxHealth = ChrData.MaxHealth + StatChanges[StatType.MaxHealth];
                    break;

                case StatType.Health:
                    ChrData.Health = ChrData.Health + StatChanges[StatType.Health];
                    break;

                case StatType.Attack:
                    ChrData.Attack = ChrData.Attack + StatChanges[StatType.Attack];
                    break;

                case StatType.Speed:
                    ChrData.Speed = ChrData.Speed + StatChanges[StatType.Speed];
                    break;

                case StatType.Defense:
                    ChrData.Defense = ChrData.Defense + StatChanges[StatType.Defense];
                    break;
                default:
                    break;
            }
        }
    }

    public Damage SendAttack(Character target)
    {
        Damage damage = new Damage();

        // Calculate Modifiers    
        damage.IsCrit = Random.value <= ChrData.CritChance;
        damage.CriticalAmount =  damage.IsCrit ? 1.5f : 1;
        damage.HasAdvantage = Advantage(target);
        damage.HasDisAdvantage = target.Advantage(this);
        damage.TypeAmount =  damage.HasDisAdvantage ? 0.5f : damage.HasAdvantage ? 1.5f : 1;

        // Calculate Damage
        damage.Amount = (int)((ChrData.Attack + Random.Range(0, (Mathf.Log10(ChrData.Attack) + 1) * 10)) * damage.TypeAmount * damage.CriticalAmount);
        target.ReceiveAttack(this, damage);
        return damage;
    }

    public void ReceiveAttack(Character target, Damage damage)
    {
        int damageFinal = Mathf.Max(0, damage.Amount - ChrData.Defense);
        damage.Amount = damageFinal;
        chrData.Health = Mathf.Clamp(chrData.Health - damageFinal, 0, chrData.MaxHealth);
    }

    // Apply Methods
    public void ApplyStatChanges(BaseStatModifier baseStatChange)
    {        
        if (StatChanges.ContainsKey(baseStatChange.baseStat))
            StatChanges[baseStatChange.baseStat] += baseStatChange.change;
        else
            StatChanges[baseStatChange.baseStat] = baseStatChange.change;

        UpdateStats();
    }

    public void ClearStatChanges()
    {
        StatChanges = new Dictionary<StatType, int>()
        {
            {StatType.MaxHealth, 0},
            {StatType.Health, 0},
            {StatType.Attack, 0},
            {StatType.Speed, 0},
            {StatType.Defense, 0},
        };
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
