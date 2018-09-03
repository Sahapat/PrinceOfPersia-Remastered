using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController:MonoBehaviour
{
    [Serializable]
    private struct Status
    {
        public byte health;
        public float stalkScale;
        public float runningScale;
        public float jumpScale;
    }
    enum ActionState
    {
        IDLE,
        STALK,
        RUNING,
    }
    [SerializeField]private Status status;
    private ActionState actionState;
    private bool sideFacing = false; // true = right,false = left

    void Awake()
    {

    }
    void Update()
    {
        
    }

}