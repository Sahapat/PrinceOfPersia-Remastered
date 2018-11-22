using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public bool canCombat;
    public bool isPlayerAttacking;
    public bool isPlayerParring;
    public bool isEnemyAttacking;
    public bool isEnemyParrying;
    public GameObject targetPlayer;
    public GameObject currentEnemy;

    private Prince prince;
    private Guard guard;

    private bool parringTrigger;
    void Update()
    {
        Guard guard = null;
        if (currentEnemy)
        {
            guard = currentEnemy.GetComponent<Guard>();
        }
        if (targetPlayer)
        {
            if (!prince)
            {
                prince = targetPlayer.GetComponent<Prince>();
            }
            else
            {
                GameCore.uIHandler.UpdateUIPrince(prince);
            }
        }
        if (guard)
        {
            GameCore.uIHandler.UpdateUIEnemy(guard);
            if (targetPlayer && guard.canCombat)
            {
                canCombat = true;
            }
        }
        else
        {
            canCombat = false;
        }

        if (targetPlayer && currentEnemy)
        {
            if ((isEnemyAttacking && isPlayerParring) && !parringTrigger)
            {
                guard.OnTakeParry();
                parringTrigger = true;
                Invoke("ResetParry", 0.5f);
            }
            else if ((isEnemyParrying && isPlayerAttacking) && !parringTrigger)
            {
                prince.OnTakeParry();
                parringTrigger = true;
                Invoke("ResetParry", 0.5f);
            }
        }
    }
    private void ResetParry()
    {
        parringTrigger = false;
    }
}
