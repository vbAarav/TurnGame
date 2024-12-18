using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BattleUI : MonoBehaviour
{
    // References
    [SerializeField] public CharacterUI playerChrUI;
    [SerializeField] public CharacterUI enemyChrUI;

    [SerializeField] public BattleDialogue battleDialogue;
    [SerializeField] public TurnBar turnBar;
    [SerializeField] public GameObject damagePopup;

   

    // Start is called before the first frame update
    public IEnumerator SetupUI(Character startingLeft, Character startingRight)
    {
        // Place Starting UI Elements
        playerChrUI.PlaceCharacterUI(startingLeft, -1);  
        enemyChrUI.PlaceCharacterUI(startingRight, 1);
        battleDialogue.SetSkillNames(startingLeft.ChrData.Skills);

        yield return battleDialogue.TypeDialogue("A new battle approaches.", 30);
    }

    // Battle UI Methods
    public CharacterUI GetCharacterUI(Character character)
    {
        if (playerChrUI.Chr == character)
        {
            return playerChrUI;
        }
        else if (enemyChrUI.Chr == character)
        {
            return enemyChrUI;
        }
        else
        {
            return null;
        }
    }

    // Create Damage Popup
    public void CreatePopup(CharacterUI characterUI, Damage damage)
    {
        // Create Popup
        damagePopup.GetComponent<DamagePopup>().SetText(damage.Amount.ToString());

        // Type
        if (damage.HasAdvantage)
            damagePopup.GetComponent<DamagePopup>().SetTextColour(Color.green);
        else if (damage.HasDisAdvantage)
            damagePopup.GetComponent<DamagePopup>().SetTextColour(Color.red);
        else    
            damagePopup.GetComponent<DamagePopup>().SetTextColour(Color.white);

        // Crit
        if (damage.IsCrit)
        {   
            damagePopup.GetComponent<DamagePopup>().SetOutlineColour(Color.white);
        }
        else
            damagePopup.GetComponent<DamagePopup>().SetOutlineColour(Color.black);

        // Create Popup
        Instantiate(damagePopup, characterUI.transform.position, Quaternion.identity);  
    }

     public IEnumerator AnimateAttackAction(Damage damage, CharacterUI attackerUI, CharacterUI targetUI)
    {
        // Update Dialogue and Character UI
        battleDialogue.EnableActionSelector(false);
        StartCoroutine(battleDialogue.TypeDialogue($"{attackerUI.Chr.ChrData.Name} attacks {targetUI.Chr.ChrData.Name}", 30));
        attackerUI.PlayAttackAnimation();    
        targetUI.PlayHitAnimation();
        yield return new WaitForSeconds(1f);   

        // Update Health UI
        StartCoroutine(targetUI.UpdateHealthAnimateUI());
        CreatePopup(targetUI, damage);
        yield return new WaitForSeconds(1f);

    }
}
