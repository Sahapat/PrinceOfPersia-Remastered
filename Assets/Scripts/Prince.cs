using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prince : CharacterSystem
{
    [Header("StepProperty")]
    [SerializeField] private float normalStepScale;
    [SerializeField] private float normalStepSpeed;
    [SerializeField] private float normalStartStepDuration;
    [SerializeField] private float normalStepDuration;
    [SerializeField] private float normalTurnDuration;
    [Header("CrouchProperty")]
    [SerializeField] private float crouchStepScale;
    [SerializeField] private float crouchStepSpeed;
    [SerializeField] private float crouchStepDuration;
    [SerializeField] private float toCrouchDuration;
    [SerializeField] private float outCrouchDuration;
    [Header("RunProperty")]
    [SerializeField] private float runStepScale;
    [SerializeField] private float runStopScale;
    [SerializeField] private float runSpeed;
    [SerializeField] private float runStartDuration;
    [SerializeField] private float runStopDuration;
    [SerializeField] private float runTurnDuration;
    [SerializeField] private float runCycleInputDelay;
    [Header("SpecialActionProperty")]
    [SerializeField] private float jumpStartDuration;
    [Header("Other")]
    [SerializeField] private float fightDrawSwordDuration;
    [SerializeField] private float fightShealthSwordDuration;
    [Header("Reference")]
    [SerializeField] private Animator princeAnimator;
    //Counter Variable
    private float normalStepCount;
    private float crouchStepCount;
    private float runStartCount;
    private float runStopCount;
    private float runTurnCount;
    private float runCycleInputCount;
    //Other
    private bool isCrouch;
    private bool isRunning;
    private bool isInteractSomething;
    [HideInInspector] public bool canJumpUp;
    [HideInInspector] public bool canClimbDown;
    [HideInInspector] public bool canClimbUp;

    private WaitForSeconds waitForStartStep;
    private WaitForSeconds waitForCrouch;
    private WaitForSeconds waitForOutCrouch;
    private WaitForSeconds waitForShealth;
    private WaitForSeconds waitForDrawSword;
    private WaitForSeconds waitForNormalTurn;
    private WaitForSeconds waitForStep;
    protected override void OnAwake()
    {
        base.OnAwake();
        waitForShealth = new WaitForSeconds(fightShealthSwordDuration);
        waitForDrawSword = new WaitForSeconds(fightDrawSwordDuration);
        waitForCrouch = new WaitForSeconds(toCrouchDuration);
        waitForOutCrouch = new WaitForSeconds(outCrouchDuration);
        waitForNormalTurn = new WaitForSeconds(normalTurnDuration);
        waitForStartStep = new WaitForSeconds(normalStartStepDuration);
        waitForStep = new WaitForSeconds(normalStepDuration - normalStartStepDuration);
    }
    protected override void OnTakeDamage()
    {
        princeAnimator.SetTrigger("TakeDamage");
    }
    protected override void OnNormal()
    {
        if (controlable)
        {
            //Key Interact Implement
            if (InputManager.GetKeyDown_Interact() && GameCore.combatController.canCombat)
            {
                StartCoroutine(IdleToCombat());
            }
            if (InputManager.GetKey_Interact())
            {
                isInteractSomething = true;
            }
            else if (InputManager.GetKeyUp_Interact())
            {
                isInteractSomething = false;
            }
            else if (!InputManager.GetKey_Interact())
            {
                isInteractSomething = false;
            }
            //Key down Implement
            if (InputManager.GetKeyDown_Down())
            {
                if (canClimbDown)
                {

                }
                else
                {
                    isCrouch = true;
                    StartCoroutine(ToCrouch());
                }
            }
            else if (InputManager.GetKey_Down())
            {

            }
            else if (!InputManager.GetKey_Down())
            {
                if (isCrouch)
                {
                    isCrouch = false;
                    StartCoroutine(OutCrouch());
                }
            }
            //Key Left Implement
            if (InputManager.GetKeyDown_Left())
            {
                if (isCrouch)
                {
                    if (!currentFacing)
                    {
                        if (crouchStepCount <= Time.time)
                        {
                            isMoving = true;
                            crouchStepCount = Time.time + crouchStepDuration;
                            settingMoveSpeed = crouchStepSpeed;
                            predictPosition = new Vector3(transform.position.x - crouchStepScale, transform.position.y, transform.position.z);
                            princeAnimator.SetTrigger("CrouchStep");
                        }
                    }
                }
                else
                {
                    if (currentFacing)
                    {
                        StartCoroutine(IdleTurn());
                    }
                    else if (isInteractSomething && normalStepCount <= Time.time && !isMoving)
                    {
                        predictPosition = new Vector3(transform.position.x - normalStepScale, transform.position.y, transform.position.z);
                        settingMoveSpeed = normalStepSpeed;
                        StartCoroutine(Step());
                    }
                    else if (!isInteractSomething && !isMoving && !isRunning)
                    {
                        isRunning = true;
                    }
                }
            }
            //Key Right Implement
            else if (InputManager.GetKeyDown_Right())
            {
                if (isCrouch)
                {
                    if (currentFacing)
                    {
                        if (crouchStepCount <= Time.time)
                        {
                            isMoving = true;
                            crouchStepCount = Time.time + crouchStepDuration;
                            settingMoveSpeed = crouchStepSpeed;
                            predictPosition = new Vector3(transform.position.x + crouchStepScale, transform.position.y, transform.position.z);
                            princeAnimator.SetTrigger("CrouchStep");
                        }
                    }
                }
                else
                {
                    if (!currentFacing)
                    {
                        StartCoroutine(IdleTurn());
                    }
                    else if (isInteractSomething && normalStepCount <= Time.time && !isMoving)
                    {
                        predictPosition = new Vector3(transform.position.x + normalStepScale, transform.position.y, transform.position.z);
                        settingMoveSpeed = normalStepSpeed;
                        StartCoroutine(Step());
                    }
                    else if (!isInteractSomething && !isMoving && !isRunning)
                    {
                        isRunning = true;
                    }
                }
            }
        }
    }
    protected override void OnCombat()
    {
        base.OnCombat();
        if (controlable && !isMoving)
        {
            //Fight Combat implement
            if (InputManager.GetKeyDown_Interact())
            {
                if (GameCore.combatController.canCombat)
                {
                    isAttacking = true;
                    princeAnimator.SetTrigger("Attack");
                }
                else
                {
                    StartCoroutine(CombatToIdle());
                }
            }
            if (InputManager.GetKeyDown_Up())
            {
                FightParry();
            }
            //Fight moving implement
            if (InputManager.GetKeyDown_Right())
            {
                if (currentFacing)
                {
                    FightStep(1);
                    princeAnimator.SetTrigger("FightStepForward");
                }
                else
                {
                    FightStep(1);
                    princeAnimator.SetTrigger("FightStepBack");
                }
            }
            else if (InputManager.GetKeyDown_Left())
            {
                if (!currentFacing)
                {
                    FightStep(-1);
                    princeAnimator.SetTrigger("FightStepForward");
                }
                else
                {
                    FightStep(-1);
                    princeAnimator.SetTrigger("FightStepBack");
                }
            }
        }
    }
    protected override void OnDie()
    {
        base.OnDie();
        GameCore.gameManager.GameEnd();
    }
    private IEnumerator Step()
    {
        controlable = false;
        normalStepCount = Time.time + normalStepDuration;
        princeAnimator.SetTrigger("Step");
        yield return waitForStartStep;
        isMoving = true;
        yield return waitForStep;
        controlable = true;
    }
    private IEnumerator ToCrouch()
    {
        princeAnimator.SetBool("Crouch", true);
        controlable = false;
        yield return waitForCrouch;
        controlable = true;
    }
    private IEnumerator OutCrouch()
    {
        princeAnimator.SetBool("Crouch", false);
        controlable = false;
        yield return waitForOutCrouch;
        controlable = true;
    }
    private IEnumerator IdleTurn()
    {
        princeAnimator.SetTrigger("Turn");
        controlable = false;
        yield return waitForNormalTurn;
        spriteRenderer.flipX = !spriteRenderer.flipX;
        controlable = true;
    }
    private IEnumerator CombatToIdle()
    {
        princeAnimator.SetBool("Combat", false);
        controlable = false;
        yield return waitForShealth;
        actionState = CharacterState.NORMAL;
        controlable = true;
    }
    private IEnumerator IdleToCombat()
    {
        princeAnimator.SetBool("Combat", true);
        controlable = false;
        yield return waitForDrawSword;
        actionState = CharacterState.COMBAT;
        controlable = true;
    }
}
