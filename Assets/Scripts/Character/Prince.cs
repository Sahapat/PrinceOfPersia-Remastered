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
    [SerializeField] private float runStopSpeed;
    [SerializeField] private float runStartDuration;
    [SerializeField] private float runStopDuration;
    [SerializeField] private float runTurnDuration;
    [SerializeField] private float runCycleInputDelay;
    [Header("SpecialActionProperty")]
    [SerializeField] private float fallTakeDamageStunDuration;
    [SerializeField] private float jumpStartDuration;
    [SerializeField] private float jumpScale;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float climbUpDuration;
    [SerializeField] private float climbDownDuration;
    [Header("Other")]
    [SerializeField] private float fightDrawSwordDuration;
    [SerializeField] private float fightShealthSwordDuration;
    [Header("Reference")]
    [SerializeField] private Animator princeAnimator;
    //Counter Variable
    private float crouchStepCount;
    private float runTurnCount;
    private float runCycleInputCount;
    private float climbUpDurationCount;
    //Other
    private bool isCrouch;
    private bool isRunning;
    private bool isInteractSomething;
    private bool isDeadFromFall;
    private bool canClimbUp;
    private bool canClimpDown;
    private bool deadTriggerSet;
    private bool forwardBlock;

    private WaitForSeconds waitForStartStep;
    private WaitForSeconds waitForCrouch;
    private WaitForSeconds waitForOutCrouch;
    private WaitForSeconds waitForShealth;
    private WaitForSeconds waitForDrawSword;
    private WaitForSeconds waitForNormalTurn;
    private WaitForSeconds waitForStep;
    private WaitForSeconds waitForStartRun;
    private WaitForSeconds waitForStopRun;
    private WaitForSeconds waitForFallStun;
    private WaitForSeconds waitForStartJump;
    private WaitForSeconds waitForJump;
    private WaitForSeconds waitForclimbUp;
    private WaitForSeconds waitForclimDown;

    private InteractFloor currentInteractFloor;
    public void DieSprike()
    {
        health = 0;
        controlable = false;
        deadTriggerSet = false;
        princeAnimator.SetTrigger("DeadSpike");
    }
    public void Dead()
    {
        health = 0;
        controlable = false;
        deadTriggerSet = false;
        princeAnimator.SetTrigger("DeadFall");
    }
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
        waitForStartRun = new WaitForSeconds(runStartDuration);
        waitForStopRun = new WaitForSeconds(runStopDuration);
        waitForFallStun = new WaitForSeconds(fallTakeDamageStunDuration);
        waitForStartJump = new WaitForSeconds(jumpStartDuration);
        waitForJump = new WaitForSeconds(jumpDuration);
        waitForclimbUp = new WaitForSeconds(climbUpDuration);
        waitForclimDown = new WaitForSeconds(climbDownDuration);
    }
    protected override void OnUpdate()
    {
        var floorCheckPos = new Vector2(transform.position.x + floorColider.offset.x, transform.position.y + floorColider.offset.y);
        var floorCheckResult = Physics2D.OverlapBox(floorCheckPos, floorColider.size, 0, LayerMask.GetMask("Floor"));
        if (floorCheckResult)
        {
            var interactFloorScript = floorCheckResult.GetComponent<InteractFloor>();
            if (interactFloorScript)
            {
                currentInteractFloor = interactFloorScript;
                if (interactFloorScript.canClimbDown)
                {
                    canClimpDown = interactFloorScript.getCanClimbDown(transform.position, currentFacing);
                }
            }
        }
        else
        {
            canClimpDown = false;
            currentInteractFloor = null;
        }
        GameCore.combatController.isPlayerAttacking = isAttacking;
        GameCore.combatController.isPlayerParring = isParring;
        GameCore.combatController.targetPlayer = this.gameObject;
    }
    protected override void OnTakeDamage()
    {
        print("takeDamage");
        princeAnimator.SetTrigger("TakeDamage");
    }
    protected override void OnParry()
    {
        princeAnimator.SetTrigger("Parry");
    }
    protected override void OnNormal()
    {
        if (controlable)
        {
            #region InteractKey Implement
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
            #endregion
            #region Keydown Implement
            if (InputManager.GetKey_Down())
            {
                if (canClimpDown)
                {
                    StartCoroutine(ClimbDown());
                }
                else if (!isCrouch)
                {
                    isCrouch = true;
                    StartCoroutine(ToCrouch());
                }
            }
            else if (!InputManager.GetKey_Down())
            {
                if (isCrouch)
                {
                    isCrouch = false;
                    StartCoroutine(OutCrouch());
                }
            }
            #endregion
            #region KeyLeft Implement
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
                        if (isRunning)
                        {

                        }

                        else
                        {
                            StartCoroutine(IdleTurn());
                        }
                    }
                    else
                    {
                        if (isInteractSomething && !isMoving)
                        {
                            predictPosition = new Vector3(transform.position.x - normalStepScale, transform.position.y, transform.position.z);
                            settingMoveSpeed = normalStepSpeed;
                            if (currentInteractFloor)
                            {
                                var distance = Mathf.Abs(currentInteractFloor.climbDownPosition.position.x) - Mathf.Abs(predictPosition.x);
                                if (distance > 0)
                                {
                                    predictPosition = new Vector3(currentInteractFloor.climbDownPosition.position.x, transform.position.y, transform.position.z);
                                    StartCoroutine(Step());
                                }
                                else if (distance < 0)
                                {
                                    StartCoroutine(Step());

                                }
                                else
                                {
                                    StartCoroutine(StepBlock());
                                }
                            }
                            else
                            {
                                StartCoroutine(Step());
                            }
                        }
                        else if (runCycleInputCount <= Time.time && !isRunning)
                        {
                            predictPosition = new Vector3(transform.position.x - runStepScale, transform.position.y, transform.position.z);
                            runCycleInputCount = Time.time + runCycleInputDelay;
                            settingMoveSpeed = runSpeed;
                            StartCoroutine(StartRun());
                        }
                    }
                }
            }
            if (InputManager.GetKey_Left())
            {
                if (isRunning && runCycleInputCount <= Time.time)
                {
                    predictPosition = new Vector3(predictPosition.x - runStepScale, predictPosition.y, predictPosition.z);
                    runCycleInputCount = Time.time + runCycleInputDelay;
                    settingMoveSpeed = runSpeed;
                    isMoving = true;
                }
            }
            else if (!InputManager.GetKey_Left())
            {
                if (isRunning && !currentFacing)
                {
                    predictPosition = new Vector3(predictPosition.x - runStopScale, predictPosition.y, predictPosition.z);
                    settingMoveSpeed = runStopSpeed;
                    StartCoroutine(StopRun());
                }
            }
            #endregion
            #region KeyRight Implement
            if (InputManager.GetKeyDown_Right())
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
                        if (isRunning)
                        {

                        }
                        else
                        {
                            StartCoroutine(IdleTurn());
                        }
                    }
                    else
                    {
                        if (isInteractSomething && !isMoving)
                        {
                            predictPosition = new Vector3(transform.position.x + normalStepScale, transform.position.y, transform.position.z);
                            settingMoveSpeed = normalStepSpeed;
                            if (currentInteractFloor)
                            {
                                var distance = Mathf.Abs(currentInteractFloor.climbDownPosition.position.x) - Mathf.Abs(predictPosition.x);
                                if (distance < 0)
                                {
                                    predictPosition = new Vector3(currentInteractFloor.climbDownPosition.position.x, transform.position.y, transform.position.z);
                                    StartCoroutine(Step());
                                }
                                else if (distance > 0)
                                {
                                    StartCoroutine(Step());

                                }
                                else
                                {
                                    StartCoroutine(StepBlock());
                                }
                            }
                            else
                            {
                                StartCoroutine(Step());
                            }
                        }
                        else if (runCycleInputCount <= Time.time && !isRunning)
                        {
                            predictPosition = new Vector3(transform.position.x + runStepScale, transform.position.y, transform.position.z);
                            runCycleInputCount = Time.time + runCycleInputDelay;
                            settingMoveSpeed = runSpeed;
                            StartCoroutine(StartRun());
                        }
                    }
                }
            }
            if (InputManager.GetKey_Right())
            {
                if (isRunning && runCycleInputCount <= Time.time)
                {
                    predictPosition = new Vector3(predictPosition.x + runStepScale, predictPosition.y, predictPosition.z);
                    runCycleInputCount = Time.time + runCycleInputDelay;
                    settingMoveSpeed = runSpeed;
                    isMoving = true;
                }
            }
            else if (!InputManager.GetKey_Right())
            {
                if (isRunning && currentFacing)
                {
                    predictPosition = new Vector3(predictPosition.x + runStopScale, predictPosition.y, predictPosition.z);
                    settingMoveSpeed = runStopSpeed;
                    StartCoroutine(StopRun());
                }
            }
            #endregion
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
        if (deadTriggerSet)
        {
            if (isDeadFromFall)
            {
                princeAnimator.SetTrigger("DeadFall");
            }
            else
            {
                princeAnimator.SetTrigger("Dead");
            }
            deadTriggerSet = false;
        }
        GameCore.gameManager.GameEnd();
    }
    protected override void OnStartFall()
    {
        base.OnStartFall();
        princeAnimator.SetTrigger("Fall");
        princeAnimator.SetBool("Running", false);
        isMoving = false;
        isRunning = false;
    }
    protected override void OnStopFall()
    {
        base.OnStopFall();
        if (health != 0)
        {
            if (fallDamageTaken <= 1)
            {
                StartCoroutine(FallToOutCrouch());
            }
            else
            {
                StartCoroutine(FallToStun());
            }
        }
        else
        {
            isDeadFromFall = true;
        }
    }
    protected override void OnStart()
    {
        base.OnStart();
        deadTriggerSet = true;
    }
    private IEnumerator StopRun()
    {
        princeAnimator.SetBool("Running", false);
        princeAnimator.SetBool("ForwardBlock", forwardBlock);
        isRunning = false;
        isMoving = true;
        controlable = false;
        yield return waitForStopRun;
        controlable = true;
    }
    private IEnumerator ClimbDown()
    {
        controlable = false;
        isCheckingFall = false;
        princeAnimator.SetTrigger("Drop");
        yield return waitForclimDown;
        isCheckingFall = true;
        controlable = true;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
    }
    private IEnumerator StartRun()
    {
        isRunning = true;
        princeAnimator.SetBool("Running", true);
        yield return waitForStartRun;
        isMoving = true;
    }
    private IEnumerator FallToStun()
    {
        princeAnimator.SetTrigger("FallTakeDamage");
        yield return waitForFallStun;
        princeAnimator.SetTrigger("CrouchOut");
        yield return waitForOutCrouch;
        controlable = true;
    }
    private IEnumerator FallToOutCrouch()
    {
        princeAnimator.SetTrigger("FallNotTakeDamage");
        yield return outCrouchDuration;
        controlable = true;
    }
    private IEnumerator Step()
    {
        controlable = false;
        princeAnimator.SetTrigger("Step");
        yield return waitForStartStep;
        isMoving = true;
        yield return waitForStep;
        controlable = true;
    }
    private IEnumerator StepBlock()
    {
        controlable = false;
        princeAnimator.SetTrigger("StepBlock");
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
