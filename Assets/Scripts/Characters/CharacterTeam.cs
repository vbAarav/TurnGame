using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTeam : MonoBehaviour
{
    [SerializeField] public List<Character> characters;

    private void Start()
    {
        foreach (var chr in characters) 
        { 
            chr.SetupCharacter();
        }
    }
}
