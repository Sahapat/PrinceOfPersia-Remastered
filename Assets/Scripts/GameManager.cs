﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCore
{
    public static TimeCounter timeCounter;
    public static GameManager gameManager;
    public static CombatController combatController;
    public static CameraController cameraController;
    public static UIHandler uIHandler;
    public static ThemeSoundHandler themeSoundHandler;
    public static NormalSoundHandler normalSoundHandler;
}
public class GameManager : MonoBehaviour
{
    public bool isGameEnd{get;private set;}
    void Awake()
    {
		GameCore.gameManager = this;
        GameCore.timeCounter = GetComponent<TimeCounter>();
        GameCore.combatController = GetComponent<CombatController>();
        GameCore.cameraController = Camera.main.GetComponent<CameraController>();
        GameCore.uIHandler = GetComponent<UIHandler>();
        GameCore.themeSoundHandler = GetComponent<ThemeSoundHandler>();
        GameCore.normalSoundHandler = GetComponent<NormalSoundHandler>();
    }
    public void GameEnd()
    {
        isGameEnd = true;
        GameCore.combatController.canCombat = false;
    }
}
