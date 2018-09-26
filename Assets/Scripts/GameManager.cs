using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCore
{
    public static TimeCounter timeCounter;
    public static GameManager gameManager;
    public static CombatController combatController;
}
public class GameManager : MonoBehaviour
{
    public bool isGameEnd{get;private set;}
    void Awake()
    {
		GameCore.gameManager = this;
        GameCore.timeCounter = GetComponent<TimeCounter>();
        GameCore.combatController = GetComponent<CombatController>();
    }
    public void GameEnd()
    {
        isGameEnd = true;
        GameCore.combatController.canCombat = false;
    }
}
