using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { World, Battle }
public class GameController : MonoBehaviour
{   
    // References
    [SerializeField] BattleHandler battleHandler;
    [SerializeField] PlayerController playerController;
    [SerializeField] Camera worldCamera;

    GameState state;

    private void Start()
    {
        playerController.OnEncountered += EnterBattle;
        battleHandler.OnExitBattle += ExitBattle;
    }

    private void Update()
    {
        if (state == GameState.World)
        {
            playerController.RunUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleHandler.RunUpdate();
        }
    }

    // Move to the Battle State
    void EnterBattle()
    {
        state = GameState.Battle;
        battleHandler.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerTeam = playerController.GetComponent<CharacterTeam>().characters;
        var enemyTeam = FindObjectOfType<WorldMapArea>().GetComponent<CharacterTeam>().characters;

        battleHandler.StartBattle(playerTeam, enemyTeam);
    }

    void ExitBattle(bool won)
    {
        state = GameState.World;
        battleHandler.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

    }
}
