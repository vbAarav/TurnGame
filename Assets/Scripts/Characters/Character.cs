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
    public List<Status> Statuses { get; private set;} = new List<Status>();
    public Dictionary<BaseStats, int> CurrentStatChanges { get; private set; }
  
    // Variables
    public event System.Action OnStatusChanged;

    // Character Initalisation
    public void SetupCharacter()
    {
        CurrentStatChanges = new Dictionary<BaseStats, int>()
        {
            {BaseStats.MaxHealth, 0},
            {BaseStats.Health, 0},
            {BaseStats.Attack, 0},
            {BaseStats.Speed, 0},
            {BaseStats.Defense, 0},
        };
    }

    public void Update()
    {

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
    public void UpdateStats()
    {
        foreach (KeyValuePair<BaseStats, int> statChange in CurrentStatChanges)
        {
            switch (statChange.Key)
            {
                case BaseStats.MaxHealth:
                    ChrStats.MaxHealth = ChrStats.MaxHealth + CurrentStatChanges[BaseStats.MaxHealth];
                    break;

                case BaseStats.Health:
                    ChrStats.Health = ChrStats.Health + CurrentStatChanges[BaseStats.Health];
                    break;

                case BaseStats.Attack:
                    ChrStats.Attack = ChrStats.Attack + CurrentStatChanges[BaseStats.Attack];
                    break;

                case BaseStats.Speed:
                    ChrStats.Speed = ChrStats.Speed + CurrentStatChanges[BaseStats.Speed];
                    break;

                case BaseStats.Defense:
                    ChrStats.Defense = ChrStats.Defense + CurrentStatChanges[BaseStats.Defense];
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
        damage.IsCrit = Random.value <= ChrStats.CritChance;
        damage.CriticalAmount =  damage.IsCrit ? 1.5f : 1;
        damage.HasAdvantage = Advantage(target);
        damage.HasDisAdvantage = target.Advantage(this);
        damage.TypeAmount =  damage.HasDisAdvantage ? 0.5f : damage.HasAdvantage ? 1.5f : 1;

        // Calculate Damage
        damage.Amount = (int)((ChrStats.Attack + Random.Range(0, (Mathf.Log10(ChrStats.Attack) + 1) * 10)) * damage.TypeAmount * damage.CriticalAmount);
        target.ReceiveAttack(this, damage);
        return damage;
    }

    public void ReceiveAttack(Character target, Damage damage)
    {
        int damageFinal = Mathf.Max(0, damage.Amount - ChrStats.Defense);
        damage.Amount = damageFinal;
        chrStats.Health = Mathf.Clamp(chrStats.Health - damageFinal, 0, chrStats.MaxHealth);
    }

    public void ApplyStatChanges(BaseStatModifier baseStatChange)
    {        
        if (CurrentStatChanges.ContainsKey(baseStatChange.baseStat))
            CurrentStatChanges[baseStatChange.baseStat] += baseStatChange.change;
        else
            CurrentStatChanges[baseStatChange.baseStat] = baseStatChange.change;

        UpdateStats();
    }

    public void AddStatus(StatusApplier statusApplier)
    {
        Statuses.Add(statusApplier.status);        
        Debug.Log($"{statusApplier.status.Name} has been applied");
        OnStatusChanged();
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
