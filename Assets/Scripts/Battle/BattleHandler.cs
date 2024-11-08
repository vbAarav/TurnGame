using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState {Start, NextTurn, ActionSelection, SkillSelection, EnemySelection, PerformMove, BattleOver, Loading}

public class BattleHandler : MonoBehaviour
{
    // References
    public List<Character> teamOne;  
    public List<Character> teamTwo;

    [SerializeField] BattleUI battleUI;

    // Variables
    private Queue<Character> turnOrder;
    private Character activeChr;
    BattleState state;
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
        state = BattleState.Start;

        // Calculate turn order based on speed
        var allCharacters = teamOne.Concat(teamTwo).OrderByDescending(x => x.ChrBase.Speed).ToList();
        turnOrder = new Queue<Character>(allCharacters.Where(chr => chr.isAlive())); 

        // Update UI
        Character left = teamOne.OrderByDescending(x => x.ChrBase.Speed).FirstOrDefault();
        Character right = teamTwo.OrderByDescending(x => x.ChrBase.Speed).FirstOrDefault();
        yield return StartCoroutine(battleUI.SetupUI(left, right, turnOrder));
        yield return new WaitForSeconds(1f);

        // Reset Character Values
        allCharacters.ForEach(chr => chr.ChrBase.Health = chr.ChrBase.MaxHealth); // Full Heal All Players
        allCharacters.ForEach(chr => chr.ClearStatChanges()); // Clear Stat Changes
        allCharacters.ForEach(chr => chr.ClearStatuses()); // Clear Statuses Changes

        NextTurn();
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

    // Run the active character's turn
    void NextTurn()
    {
        // Switch to the Next Character's Turn
        Character prevChr = activeChr != null ? activeChr : null;
        DequeueTurn();        

        // Player's Turn
        if (teamOne.Contains(activeChr))
        {
            // Update UI
            if (battleUI.playerChrUI.Chr != activeChr) 
                StartCoroutine(SwitchCharacter(activeChr, battleUI.playerChrUI)); 
        
            battleUI.battleDialogue.EnableActionSelector(true);
            state = BattleState.ActionSelection;
        }
        // Enemy's Turn
        else
        {
            // Update UI
            if (battleUI.enemyChrUI.Chr != activeChr) 
                StartCoroutine(SwitchCharacter(activeChr, battleUI.enemyChrUI)); 
            battleUI.battleDialogue.EnableActionSelector(false);            
            state = BattleState.EnemySelection;
        }     
    }

    // State Manager
    public void RunUpdate()
    {
        if (state == BattleState.NextTurn)
        {
            state = BattleState.Loading;
            NextTurn();
        }
        else if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.SkillSelection)
        {
            HandleSkillSelection();
        }
        else if (state == BattleState.EnemySelection)
        {
            StartCoroutine(AttackAction(activeChr, battleUI.playerChrUI.Chr));
        }
    }

    // Run when a Character Dies
    public IEnumerator CharacterDies(Character chr, CharacterUI chrUI)
    {
        // UI Update
        yield return battleUI.battleDialogue.TypeDialogue($"{chr.ChrBase.Name} dies", 30);
        chrUI.PlayDeathAnimation();
        yield return new WaitForSeconds(2f);

        // Check if the battle has ended
        if(IsBattleOver())
            BattleOver(teamTwo.Contains(chr));
        else
        {
            yield return SwitchCharacter(teamOne.Contains(chr) ? teamOne[UnityEngine.Random.Range(0, teamOne.Count)] : teamTwo[UnityEngine.Random.Range(0, teamTwo.Count)], chrUI);
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
        state = BattleState.BattleOver;
        teamOne.ForEach(chr => chr.ClearStatChanges());
        OnExitBattle(won);
    }


    // Switch the charcter
    public IEnumerator SwitchCharacter(Character nextChr, CharacterUI characterUI)
    {
        state = BattleState.Loading;

        // Old Character Exits the Scene
        characterUI.PlayExitAnimation();        
        yield return new WaitForSeconds(1f);

        // New Character Enters the Scene
        int direction = characterUI.direction;
        characterUI.PlaceCharacterUI(nextChr, direction);
        battleUI.battleDialogue.SetSkillNames(nextChr.ChrBase.Skills);

        // UI Dialogue Update
        yield return StartCoroutine(battleUI.battleDialogue.TypeDialogue($"{nextChr.ChrBase.Name} approaches.", 30));
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

        // Get Skill
        currentAction = Mathf.Clamp(currentAction, 0, battleUI.battleDialogue.actionTexts.Count - 1);
        SkillInstance selectedSkill = activeChr.ChrBase.Skills[currentAction];

        // Update UI
        battleUI.battleDialogue.UpdateSkillSelection(currentAction);
        battleUI.battleDialogue.SetDialogue(selectedSkill.SkillBase.Description);

        // Check for a selection
        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(SkillAction(activeChr, battleUI.enemyChrUI.Chr, selectedSkill));
        else if (Input.GetKeyDown(KeyCode.X))
            SwitchToActionSelection();
            
    }

    // Move to a different UI Screen
    private void SwitchToActionSelection()
    {
        state = BattleState.ActionSelection;
        battleUI.battleDialogue.EnableSkillSelector(false);
        battleUI.battleDialogue.SetDialogue($"{activeChr.ChrBase.Name}'s turn, Choose an action");
        battleUI.battleDialogue.EnableActionSelector(true);
    }

    private void SwitchToSkillSelection()
    {
        state = BattleState.SkillSelection;
        battleUI.battleDialogue.EnableActionSelector(false);
        battleUI.battleDialogue.SetDialogue("");
        battleUI.battleDialogue.EnableSkillSelector(true);
    }

    // Handle any effects that a skill
    void RunEffects(Character source, Character target, SkillInstance skill)
    {
        foreach (BaseStatModifier bsm in skill.SkillBase.Effects.StatChanges)
        {
            if (bsm.target == EffectTarget.Self)
                source.ApplyStatChanges(bsm);
            else
                target.ApplyStatChanges(bsm);
        }

        foreach (StatusInstance statusInstance in skill.SkillBase.Effects.Statuses)
        {
            if (statusInstance.Target == EffectTarget.Self)
                source.AddStatus(statusInstance);
            else
                target.AddStatus(statusInstance);
        }
    }

    // Action Types
    public IEnumerator AttackAction(Character attacker, Character target)
    {
        state = BattleState.PerformMove;

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
        yield return battleUI.battleDialogue.TypeDialogue($"{attacker.ChrBase.Name} attacks {target.ChrBase.Name}", 30);
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
            state = BattleState.NextTurn;
        }        
    }

    public IEnumerator SkillAction(Character attacker, Character target, SkillInstance skill)
    {
        state = BattleState.PerformMove;

        // Get Characters UI
        CharacterUI attackerUI = battleUI.enemyChrUI;
        CharacterUI targetUI = battleUI.playerChrUI;

        // Check the skill category
        if (skill.SkillBase.Category == SkillCategory.Buff || skill.SkillBase.Category == SkillCategory.Debuff)
        {
            RunEffects(attacker, target, skill);
        }

        if (teamOne.Contains(attacker))
        {
            attackerUI = battleUI.playerChrUI;
            targetUI = battleUI.enemyChrUI;
        }

        // Update Dialogue and Character UI
        battleUI.battleDialogue.EnableSkillSelector(false);
        yield return battleUI.battleDialogue.TypeDialogue($"{attacker.ChrBase.Name} used {skill.SkillBase.Name}", 30);
        yield return new WaitForSeconds(1f);    

        // Check if target is dead
        if (!target.isAlive())
        {
            yield return CharacterDies(target, targetUI);
        }
        else
        {
            state = BattleState.NextTurn;
        }        
    }
}