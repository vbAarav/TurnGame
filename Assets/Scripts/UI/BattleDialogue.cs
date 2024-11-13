using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogueState {Free, Occupied}

public class BattleDialogue : MonoBehaviour
{
    // References
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Color textColour;
    [SerializeField] Color highlightedColour;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] public List<TextMeshProUGUI> actionTexts;
    [SerializeField] public List<TextMeshProUGUI> skillTexts;

    // Variables
    public DialogueState dialogueState = DialogueState.Free;

    // Update Text and Dialogue
    public void SetDialogue(string newText)
    {
        dialogueText.text = newText;
    }

    // Create a typing animation for the dialogue
    public IEnumerator TypeDialogue(string newText, int lettersPerSecond)
    {
        // Variables
        dialogueState = DialogueState.Occupied;
        dialogueText.text = "";

        foreach (char c in newText.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }

        dialogueState = DialogueState.Free;
    }

    public void SetSkillNames(List<SkillInstance> skills)
    {
        for (int i=0; i<skillTexts.Count; i++)
        {
            if (i < skills.Count)
                skillTexts[i].text = skills[i].SkillData.Name;
            else
                skillTexts[i].text = "---";
        }
    }

    // Enable Dialgoue to be shown
    public void EnableDialogue(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
    }

    // Update Selection
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i=0; i<actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColour;
            }
            else
            {
                actionTexts[i].color = textColour;
            }
        }
    }

    public void UpdateSkillSelection(int selectedAction)
    {
        for (int i=0; i<skillTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                skillTexts[i].color = highlightedColour;
            }
            else
            {
                skillTexts[i].color = textColour;
            }
        }
    }
}
