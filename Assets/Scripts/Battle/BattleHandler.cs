using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public enum BattleState {Start, NextTurn, Selection, PerformMove, BattleOver, Loading}
public enum SelectionState {None, Action, Skill, Enemy}

public class BattleHandler : MonoBehaviour
{
    // References
    public List<Character> teamOne;  
    public List<Character> teamTwo;

    [SerializeField] BattleUI battleUI;

    // Variables
    private Queue<Character> turnOrder;
    private Character activeChr;
    BattleState battleState;
    SelectionState selectionState;
    int currentAction;

    public event Action<bool> OnExitBattle;

    // Called First in this script    
    public void StartBattle(List<Character> teamOne, List<Character> teamTwo)
    {
        this.teamOne = teamOne;
        this.teamTwo = teamTwo;
        
        StartCoroutine(SetupBattle());
    }

    // Initalise Battle Elements
    public IEnumerator SetupBattle()
    {
        // Setup Battle Parameters
        battleState = BattleState.Start;
        selectionState = SelectionState.None;

        // Get Active Characters
        Character left = teamOne.OrderByDescending(x => x.ChrStats.GetStatValue(StatType.Speed)).FirstOrDefault();
        Character right = teamTwo.OrderByDescending(x => x.ChrStats.GetStatValue(StatType.Speed)).FirstOrDefault();

        // Update UI
        yield return StartCoroutine(battleUI.SetupUI(left, right));
        yield return new WaitForSeconds(1f);

        CalculateTurnOrder();
        ResetCharacterValues();      
        battleState = BattleState.NextTurn;
    }

    // Clear Characters Values
    public void ResetCharacterValues()
    {
        var allCharacters = teamOne.Concat(teamTwo).OrderByDescending(x => x.ChrStats.GetStatValue(StatType.Speed)).ToList();

        // Reset Character Values
        allCharacters.ForEach(chr => chr.FullHeal()); // Full Heal All Players
        allCharacters.ForEach(chr => chr.ClearStatChanges()); // Clear Stat Changes
        allCharacters.ForEach(chr => chr.ClearStatuses()); // Clear Statuses Changes
    }

    // Calculate the new turn order
    public void CalculateTurnOrder()
    {
        var activeCharacters = new List<Character>(){battleUI.playerChrUI.Chr, battleUI.enemyChrUI.Chr};
        activeCharacters.Sort((chrOne, chrTwo) => chrOne.ChrStats.GetStatValue(StatType.Speed).CompareTo(chrTwo.ChrStats.GetStatValue(StatType.Speed)));
        turnOrder = new Queue<Character>(activeCharacters);
    }

    // Find which Character's turn it is
    public void DequeueTurn()
    {
        // Move the Turn Order
        activeChr = turnOrder.Dequeue();
        while (!activeChr.isAlive())
           activeChr = turnOrder.Dequeue();
        turnOrder.Enqueue(activeChr);
        battleUI.turnBar.UpdateTurnOrder(turnOrder);
    }

    // Transition to the next character's
    void NextTurn()
    {
        DequeueTurn();        

        // Player's Turn
        if (teamOne.Contains(activeChr))
        {
         
            battleUI.battleDialogue.EnableActionSelector(true);
            battleState = BattleState.Selection;
            selectionState = SelectionState.Action;
        }
        // Enemy's Turn
        else
        {
           
            battleUI.battleDialogue.EnableActionSelector(false);
            battleState = BattleState.Selection;
            selectionState = SelectionState.Enemy;
        }     
    }

    // State Manager
    public void RunUpdate()
    {
        if (battleState == BattleState.NextTurn)
        {
            battleState = BattleState.Loading;
            NextTurn();
        }
        else if (battleState == BattleState.Selection)
        {
            if (selectionState == SelectionState.Action)
            {
                HandleActionSelection();
            }
            else if (selectionState == SelectionState.Skill)
            {
                HandleSkillSelection();
            }
            else if (selectionState == SelectionState.Enemy)
            {
                StartCoroutine(AttackAction(activeChr, battleUI.playerChrUI.Chr));
            }
        }
    }

    // Run when a Character Dies
    public IEnumerator CharacterDies(Character chr, CharacterUI chrUI)
    {
        // UI Update
        yield return battleUI.battleDialogue.TypeDialogue($"{chr.ChrData.Name} dies", 30);
        chrUI.PlayDeathAnimation();
        yield return new WaitForSeconds(2f);

        // Check if the battle has ended
        if(IsBattleOver())
            BattleOver(teamTwo.Contains(chr));
        else
        {
            // Variables
            var aliveTeamOne = teamOne.Where(chr => chr.isAlive()).ToList();
            var aliveTeamTwo = teamTwo.Where(chr => chr.isAlive()).ToList();

            // Switch Characters
            yield return SwitchCharacter(teamOne.Contains(chr) ? aliveTeamOne[UnityEngine.Random.Range(0, aliveTeamOne.Count)] : aliveTeamTwo[UnityEngine.Random.Range(0, aliveTeamTwo.Count)], chrUI);
            CalculateTurnOrder();
            battleState = BattleState.NextTurn;
        }
            
    }

    // Battle Situations
    public bool IsBattleOver()
    {
        return (teamOne.All(chr => !chr.isAlive()) || teamTwo.All(chr => !chr.isAlive()));
    }

    // End the Battle
    public void BattleOver(bool won)
    {
        battleState = BattleState.BattleOver;
        teamOne.ForEach(chr => chr.ClearStatChanges());
        OnExitBattle(won);
    }


    // Switch the charcter
    public IEnumerator SwitchCharacter(Character nextChr, CharacterUI characterUI)
    {
        battleState = BattleState.Loading;

        // Old Character Exits the Scene
        characterUI.PlayExitAnimation();        
        yield return new WaitForSeconds(1f);

        // New Character Enters the Scene
        int direction = characterUI.direction;
        characterUI.PlaceCharacterUI(nextChr, direction);
        battleUI.battleDialogue.SetSkillNames(nextChr.ChrData.Skills);

        // UI Dialogue Update
        yield return StartCoroutine(battleUI.battleDialogue.TypeDialogue($"{nextChr.ChrData.Name} approaches.", 30));
        yield return new WaitForSeconds(1f);

    }

    // Manage Selections
    private void HandleActionSelection()
    {
        // Scroll through the different actions
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) 
            currentAction += battleUI.battleDialogue.actionTexts.Count / 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= battleUI.battleDialogue.actionTexts.Count / 2;

        currentAction = Mathf.Clamp(currentAction, 0, battleUI.battleDialogue.actionTexts.Count - 1);

        // Update UI
        battleUI.battleDialogue.UpdateActionSelection(currentAction);

        // Check for a selection
        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (currentAction)
            {
                // Attack
                case 0:
                    StartCoroutine(AttackAction(activeChr, battleUI.enemyChrUI.Chr));
                    break;

                // Skip
                case 1:
                    NextTurn();
                    break;

                // Skills
                case 2:
                    SwitchToSkillSelection();
                    break;

                // Exit
                case 3:
                    OnExitBattle(false);
                    break;

                default:
                    break;
            }
        }
    }

    private void HandleSkillSelection()
    {
        // Scroll through the different actions
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) 
            currentAction += battleUI.battleDialogue.actionTexts.Count / 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= battleUI.battleDialogue.actionTexts.Count / 2;
        currentAction = Mathf.Clamp(currentAction, 0, battleUI.battleDialogue.skillTexts.Count - 1);
        
        // Get Skill
        SkillInstance selectedSkill;
        try
        {
            selectedSkill = activeChr.ChrData.Skills[currentAction];
        }
        catch (Exception)
        {
            selectedSkill = null;
        }
         
        // Update UI
        battleUI.battleDialogue.UpdateSkillSelection(currentAction);
        battleUI.battleDialogue.SetDialogue(selectedSkill == null ? "" : selectedSkill.SkillData.Description);

        // Check for a selection
        if (Input.GetKeyDown(KeyCode.Z) && selectedSkill != null)
            StartCoroutine(SkillAction(activeChr, battleUI.enemyChrUI.Chr, selectedSkill));
        else if (Input.GetKeyDown(KeyCode.X))
            SwitchToActionSelection();
            
    }

    // Move to a different UI Screen
    private void SwitchToActionSelection()
    {
        selectionState = SelectionState.Action;
        battleUI.battleDialogue.EnableSkillSelector(false);
        battleUI.battleDialogue.SetDialogue($"{activeChr.ChrData.Name}'s turn, Choose an action");
        battleUI.battleDialogue.EnableActionSelector(true);
    }

    private void SwitchToSkillSelection()
    {
        selectionState = SelectionState.Skill;
        battleUI.battleDialogue.EnableActionSelector(false);
        battleUI.battleDialogue.SetDialogue("");
        battleUI.battleDialogue.EnableSkillSelector(true);
    }

    // Action Types
    public IEnumerator AttackAction(Character attacker, Character target)
    {
        battleState = BattleState.PerformMove;

        // Get Characters UI
        CharacterUI attackerUI = battleUI.enemyChrUI;
        CharacterUI targetUI = battleUI.playerChrUI;

        if (teamOne.Contains(attacker))
        {
            attackerUI = battleUI.playerChrUI;
            targetUI = battleUI.enemyChrUI;
        }

        // Update Dialogue and Character UI
        battleUI.battleDialogue.EnableActionSelector(false);
        yield return battleUI.battleDialogue.TypeDialogue($"{attacker.ChrData.Name} attacks {target.ChrData.Name}", 30);
        attackerUI.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);     
        targetUI.PlayHitAnimation();   
        
        Damage damage = attacker.SendAttack(target);

        // Update Health UI
        yield return targetUI.UpdateHealthAnimateUI();
        battleUI.CreatePopup(targetUI, damage);

        // Check if target is dead
        if (!target.isAlive())
        {
            yield return CharacterDies(target, targetUI);
        }
        else
        {
            battleState = BattleState.NextTurn;
        }        
    }

    public IEnumerator SkillAction(Character attacker, Character target, SkillInstance skill)
    {
        battleState = BattleState.PerformMove;

        // Get Characters UI
        CharacterUI attackerUI = battleUI.enemyChrUI;
        CharacterUI targetUI = battleUI.playerChrUI;

        if (teamOne.Contains(attacker))
        {
            attackerUI = battleUI.playerChrUI;
            targetUI = battleUI.enemyChrUI;
        }

        // Update Dialogue and Character UI
        battleUI.battleDialogue.EnableSkillSelector(false);
        yield return battleUI.battleDialogue.TypeDialogue($"{attacker.ChrData.Name} used {skill.SkillData.Name}", 30);
        yield return new WaitForSeconds(1f);   

        skill.SkillData.ActivateEffects(attacker, target);
        yield return attackerUI.UpdateHealthAnimateUI();
        yield return targetUI.UpdateHealthAnimateUI();
        

        // Check if target is dead
        if (!target.isAlive())
        {
            yield return CharacterDies(target, targetUI);
        }
        else
        {
            battleState = BattleState.NextTurn;
        }        
    }
}