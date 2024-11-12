using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Character
{
    // Serialize Fields
    [SerializeField] CharacterBase chrStats;

    // Properties
    public CharacterBase ChrBase {get {return chrStats;} set {chrStats = value;} }
    public List<StatusInstance> Statuses { get; private set;} = new List<StatusInstance>();
    public Dictionary<BaseStats, int> CurrentStatChanges { get; private set; }
  
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
        return this.ChrBase.Health > 0;
    }

    public bool Advantage(Character target)
    {
        return TypeChart.HasAdvantage(this.ChrBase.Type, target.ChrBase.Type);
    }

    // Character Request Methods
    public void UpdateStats()
    {
        foreach (KeyValuePair<BaseStats, int> statChange in CurrentStatChanges)
        {
            switch (statChange.Key)
            {
                case BaseStats.MaxHealth:
                    ChrBase.MaxHealth = ChrBase.MaxHealth + CurrentStatChanges[BaseStats.MaxHealth];
                    break;

                case BaseStats.Health:
                    ChrBase.Health = ChrBase.Health + CurrentStatChanges[BaseStats.Health];
                    break;

                case BaseStats.Attack:
                    ChrBase.Attack = ChrBase.Attack + CurrentStatChanges[BaseStats.Attack];
                    break;

                case BaseStats.Speed:
                    ChrBase.Speed = ChrBase.Speed + CurrentStatChanges[BaseStats.Speed];
                    break;

                case BaseStats.Defense:
                    ChrBase.Defense = ChrBase.Defense + CurrentStatChanges[BaseStats.Defense];
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
        damage.IsCrit = Random.value <= ChrBase.CritChance;
        damage.CriticalAmount =  damage.IsCrit ? 1.5f : 1;
        damage.HasAdvantage = Advantage(target);
        damage.HasDisAdvantage = target.Advantage(this);
        damage.TypeAmount =  damage.HasDisAdvantage ? 0.5f : damage.HasAdvantage ? 1.5f : 1;

        // Calculate Damage
        damage.Amount = (int)((ChrBase.Attack + Random.Range(0, (Mathf.Log10(ChrBase.Attack) + 1) * 10)) * damage.TypeAmount * damage.CriticalAmount);
        target.ReceiveAttack(this, damage);
        return damage;
    }

    public void ReceiveAttack(Character target, Damage damage)
    {
        int damageFinal = Mathf.Max(0, damage.Amount - ChrBase.Defense);
        damage.Amount = damageFinal;
        chrStats.Health = Mathf.Clamp(chrStats.Health - damageFinal, 0, chrStats.MaxHealth);
    }

    // Apply Methods
    public void ApplyStatChanges(BaseStatModifier baseStatChange)
    {        
        if (CurrentStatChanges.ContainsKey(baseStatChange.baseStat))
            CurrentStatChanges[baseStatChange.baseStat] += baseStatChange.change;
        else
            CurrentStatChanges[baseStatChange.baseStat] = baseStatChange.change;

        UpdateStats();
    }

    public void ClearStatChanges()
    {
        CurrentStatChanges = new Dictionary<BaseStats, int>()
        {
            {BaseStats.MaxHealth, 0},
            {BaseStats.Health, 0},
            {BaseStats.Attack, 0},
            {BaseStats.Speed, 0},
            {BaseStats.Defense, 0},
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
