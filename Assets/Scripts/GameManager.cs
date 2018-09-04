using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCore
{
    public static TimeCounter timeCounter;
    public static GameManager gameManager;
}
public class GameManager : MonoBehaviour
{
    void Awake()
    {
		GameCore.gameManager = this;
        GameCore.timeCounter = GetComponent<TimeCounter>();
    }
}
