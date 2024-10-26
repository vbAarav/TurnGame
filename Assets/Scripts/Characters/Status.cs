using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Battle/New Status")]
public class Status : ScriptableObject
{
    // Serialize Fields
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    // Properties
    public string Name { get{ return name; } set {name = value;}}
    public string Description { get{ return description; } set {description = value;}}
    public Sprite Icon { get {return icon; }}
    
}
