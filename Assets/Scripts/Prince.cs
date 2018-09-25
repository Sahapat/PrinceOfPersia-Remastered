using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prince : CharacterSystem
{
    [Header("Other")]
    [SerializeField] private float fightDrawSwordDuration;
    [Header("Reference")]
    [SerializeField] private Animator princeAnimator;

    //Counter Variable
    private float fightDrawSwordCounter;

    //Other
    private bool isCrouch;
    private bool isInteractSomething;
    protected override void OnTakeDamage()
    {
        princeAnimator.SetTrigger("TakeDamage");
    }
    protected override void OnNormal()
    {
        if (controlable)
        {
            if (InputManager.GetKeyDown_Interact())
            {
                fightDrawSwordCounter = Time.time + fightDrawSwordDuration;
                actionState = CharacterState.COMBAT;
				princeAnimator.SetBool("Combat",true);
            }
            if(InputManager.GetKey_Interact())
            {
                isInteractSomething = true;
            }
            else if(InputManager.GetKeyUp_Interact())
            {
                isInteractSomething = false;
            }
            if(InputManager.GetKey_Down())
            {
                isCrouch = true;
            }
            else if(InputManager.GetKeyUp_Down())
            {
                isCrouch = false;
            }
            if(isCrouch)
            {
                if(InputManager.GetKeyDown_Right())
                {

                }
                else if(InputManager.GetKeyDown_Left())
                {

                }
            }
            else
            {
                if(InputManager.GetKeyDown_Right())
                {

                }
                else if(InputManager.GetKeyDown_Left())
                {

                }
            }
        }
    }
    protected override void OnCombat()
    {
        base.OnCombat();
        if (controlable && !isMoving)
        {
            if (InputManager.GetKeyDown_Interact() && fightDrawSwordCounter <= Time.time)
            {
				isAttacking = true;
				princeAnimator.SetTrigger("Attack");
            }
			if(InputManager.GetKeyDown_Right())
			{
				FightStep(1);
				princeAnimator.SetTrigger("FightStepForward");
			}
            else if(InputManager.GetKeyDown_Left())
            {
                FightStep(-1);
                princeAnimator.SetTrigger("FightStepBack");
            }
        }
    }
}
