using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    // References
    [SerializeField] public CharacterUI chrLeft;
    [SerializeField] public CharacterUI chrRight;

    [SerializeField] public BattleDialogue battleDialogue;
    [SerializeField] public GameObject damagePopup;
   

    // Start is called before the first frame update
    public IEnumerator SetupUI(Character startingLeft, Character startingRight)
    {
        chrLeft.Setup(startingLeft, -1);  
        chrRight.Setup(startingRight, 1);
        battleDialogue.SetSkillNames(startingLeft.ChrStats.Skills);
        yield return StartCoroutine(battleDialogue.TypeDialogue("A new battle approaches.", 30));
    }

    public void CreatePopup(CharacterUI characterUI, Damage damage)
    {
        // Create Popup
        damagePopup.GetComponent<DamagePopup>().SetText(damage.Amount.ToString());

        // Type
        if (damage.HasAdvantage)
            damagePopup.GetComponent<DamagePopup>().SetTextColour(Color.green);
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
}
