using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public bool canCombat;
    public bool isPlayerAttacking;
    public bool isPlayerParring;
    public GameObject targetPlayer;
    public GameObject currentEnemy;

    private Prince prince;
    private Guard guard;    
    void Update()
    {
        if(targetPlayer)
        {
            if(!prince)
            {
                prince = targetPlayer.GetComponent<Prince>();
            }
            GameCore.uIHandler.SetPrinceHealthText(prince.health);
        }
        if(currentEnemy)
        {
            if(!guard)
            {
                guard = currentEnemy.GetComponent<Guard>();
            }
            GameCore.uIHandler.SetEnemyHealthText(guard.health);
        }
    }
}
