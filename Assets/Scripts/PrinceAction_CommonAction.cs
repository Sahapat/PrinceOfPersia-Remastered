using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PrinceAction_CommonAction : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float runDrag;
    [SerializeField] private float runStartDelay;
    [SerializeField] private float crouchStepScale;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpDelay;
    [SerializeField] private float jumpScale;
    private PrinceAnimationController princeAnimationController;
    private float countDelayTimeForMove;
    private float countDelayTimeForJump;
    private bool isRunning = false;
    private Rigidbody2D prince_rigidbody;
    private WaitForSeconds second;

    void Awake()
    {
        princeAnimationController = GetComponent<PrinceAnimationController>();
        prince_rigidbody = GetComponent<Rigidbody2D>();
        second = new WaitForSeconds(1f);
    }
    void FixedUpdate()
    {
        if (princeAnimationController.GetCurrentActionState() == PrinceAnimationController.ActionState.COMBAT) return; //if on combat action return this function
        else if (princeAnimationController.isFliping) return; //if on fliping action return this functon
        if(InputManager.getInputKey_Interact())
        {
            princeAnimationController.ToCombat();
        } 
        if (InputManager.getInputKeyDown_Right())
        {
            switch (princeAnimationController.GetCurrentActionState())
            {
                case PrinceAnimationController.ActionState.IDLE:
                    if (princeAnimationController.isFliping) return;

                    if (princeAnimationController.GetCurrentFacing() == PrinceAnimationController.SideFacing.Right)
                    {
                        countDelayTimeForMove = Time.time + runStartDelay;
                        princeAnimationController.SetAnimationRuning(true);
                        isRunning = true;
                    }
                    else
                    {
                        princeAnimationController.IdleFlipFacing();
                    }
                    break;
                case PrinceAnimationController.ActionState.CROUCH:
                    break;
                case PrinceAnimationController.ActionState.STALK:
                    break;
            }
        }
        if (InputManager.getInputKey_Right())
        {
            if (princeAnimationController.GetCurrentFacing() != PrinceAnimationController.SideFacing.Right) return;
            if (isRunning && countDelayTimeForMove < Time.time)
            {
                prince_rigidbody.velocity = new Vector2(runSpeed, prince_rigidbody.velocity.y);
            }
        }

        if (InputManager.getInputKeyUp_Right())
        {
            princeAnimationController.SetAnimationRuning(false);
        }

        if (InputManager.getInputKeyDown_Left())
        {
            switch (princeAnimationController.GetCurrentActionState())
            {
                case PrinceAnimationController.ActionState.IDLE:
                    if (princeAnimationController.isFliping) return;

                    if (princeAnimationController.GetCurrentFacing() == PrinceAnimationController.SideFacing.Left)
                    {
                        countDelayTimeForMove = Time.time + runStartDelay;
                        isRunning = true;
                        princeAnimationController.SetAnimationRuning(true);
                    }
                    else
                    {
                        princeAnimationController.IdleFlipFacing();
                    }
                    break;
                case PrinceAnimationController.ActionState.CROUCH:
                    break;
                case PrinceAnimationController.ActionState.STALK:
                    break;
            }
        }
        if (InputManager.getInputKey_Left())
        {
            if (princeAnimationController.GetCurrentFacing() != PrinceAnimationController.SideFacing.Left) return;
            if (isRunning && countDelayTimeForMove < Time.time)
            {
                prince_rigidbody.velocity = new Vector2(-runSpeed, prince_rigidbody.velocity.y);
            }
        }
        if (InputManager.getInputKeyUp_Left())
        {
            princeAnimationController.SetAnimationRuning(false);
        }
    }
}