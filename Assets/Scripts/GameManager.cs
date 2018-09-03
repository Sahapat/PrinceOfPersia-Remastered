using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCore
{
    public static GameManager gameManager;
}
public class GameManager : MonoBehaviour
{
    void Awake()
    {
		GameCore.gameManager = this;
    }
}
