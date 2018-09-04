using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController:MonoBehaviour
{
    enum ActionState
    {
        IDLE,
        COMMON_ACTION,
        COMBAT,
        OTHER
    };

    private ActionState actionState;
    
    void Awake()
    {

    }
    void Update()
    {
    }
}