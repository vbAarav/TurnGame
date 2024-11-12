using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/New Character")]
public class CharacterData : ScriptableObject
{
    // Properties and Stats
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    // Base Stats
    [SerializeField] int maxHealth;
    [SerializeField] int health;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;

    // Advanced Stats
    [SerializeField, Range(0, 1)] float critChance;

    [SerializeField] CharacterType type;

    [SerializeField] List<SkillInstance> skills;

    // Getters and Setters
    public string Name { get{ return name; } set {name = value;}}
    public Sprite Sprite { get{ return sprite; } set {sprite = value;}}
    public int MaxHealth { get{ return maxHealth; } set {maxHealth = value;}}
    public int Health { get{ return health; } set {health = value;}}
    public int Attack { get{ return attack; } set {attack = value;}}
    public int Defense { get{ return defense; } set {defense = value;}}
    public int Speed { get{ return speed; } set {speed = value;}}

    public float CritChance { get{ return critChance; } set {critChance = value;}}

    public CharacterType Type { get{ return type; } set {type = value;}}

    public List<SkillInstance> Skills { get{ return skills; } set {skills = value;}}
}

public class TypeChart
{
    public static Dictionary<CharacterType, CharacterType> typeAdvantage = new Dictionary<CharacterType, CharacterType>
    {
        {CharacterType.RED, CharacterType.GREEN},
        {CharacterType.BLUE, CharacterType.RED},
        {CharacterType.GREEN, CharacterType.BLUE},
        {CharacterType.YELLOW, CharacterType.PURPLE},
        {CharacterType.PURPLE, CharacterType.YELLOW}
    };

    public static bool HasAdvantage(CharacterType attackType, CharacterType defenderType)
    {
        return typeAdvantage.ContainsKey(attackType) && typeAdvantage[attackType] == defenderType;
    }
}

