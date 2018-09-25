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
    void Awake()
    {
		GameCore.gameManager = this;
        GameCore.timeCounter = GetComponent<TimeCounter>();
        GameCore.combatController = GetComponent<CombatController>();
    }
}
