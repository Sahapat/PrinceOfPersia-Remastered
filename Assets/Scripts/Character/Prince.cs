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
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float jumpUpScale;
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
    private float runCycleInputCount;
    //Other
    private bool isCrouch;
    private bool isRunning;
    private bool isJump;
    [HideInInspector] public bool isRunningStop;
    private bool isInteractSomething;
    private bool isDeadFromFall;
    private bool canClimpDown;
    private bool deadTriggerSet;
    private bool forwardBlock;
    private string currentAnimationClip;
    private WaitForSeconds waitForStartStep;
    private WaitForSeconds waitForCrouch;
    private WaitForSeconds waitForOutCrouch;
    private WaitForSeconds waitForShealth;
    private WaitForSeconds waitForDrawSword;
    private WaitForSeconds waitForStep;
    private WaitForSeconds waitForStartRun;
    private WaitForSeconds waitForRunTurn;
    private WaitForSeconds waitForStopRun;
    private WaitForSeconds waitForFallStun;
    private WaitForSeconds waitForStartJump;
    private WaitForSeconds waitForJump;
    private WaitForSeconds waitForclimbUp;
    private WaitForSeconds waitForclimDown;
    
    private PrinceAnimationEventHandler princeAnimationEventHandler;
    public void DieSprike()
    {
        health = 0;
        controlable = false;
        deadTriggerSet = false;
        isCheckingFall = false;
        characterRigid.bodyType = RigidbodyType2D.Static;
        var spriteChild = transform.Find("PrinceSprite");
        spriteChild.transform.localPosition = Vector3.zero;
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
        waitForStartStep = new WaitForSeconds(normalStartStepDuration);
        waitForStep = new WaitForSeconds(normalStepDuration - normalStartStepDuration);
        waitForStartRun = new WaitForSeconds(runStartDuration);
        waitForStopRun = new WaitForSeconds(runStopDuration);
        waitForRunTurn = new WaitForSeconds(runTurnDuration);
        waitForFallStun = new WaitForSeconds(fallTakeDamageStunDuration);
        waitForStartJump = new WaitForSeconds(jumpStartDuration);
        waitForJump = new WaitForSeconds(jumpDuration);
        waitForclimbUp = new WaitForSeconds(climbUpDuration);
        waitForclimDown = new WaitForSeconds(climbDownDuration);
        princeAnimationEventHandler = GetComponentInChildren<PrinceAnimationEventHandler>();
    }
    protected override void OnUpdate()
    {
        var clips = princeAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = clips[0].clip.name;
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
            #region Keydown Implements
            if (InputManager.GetKey_Down())
            {
                if (!isCrouch)
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
            #region KeyUp Implement
            if(InputManager.GetKey_Up())
            {
                isJump = true;
            }
            else if (!InputManager.GetKey_Up())
            {
                isJump = false;
            }
            if (InputManager.GetKeyDown_Up())
            {
                isJump = true;
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
                else if (isJump)
                {
                    if (!currentFacing)
                    {
                        predictPosition = new Vector3(transform.position.x - jumpScale, transform.position.y, transform.position.z);
                        settingMoveSpeed = jumpSpeed;
                        StartCoroutine(JumpIdle());
                    }
                }
                else
                {

                    if (currentFacing)
                    {
                        if (isRunningStop)
                        {
                            isRunningStop = false;
                            StopAllCoroutines();
                            StartCoroutine(RuningTurn());
                        }

                        else
                        {
                            if (currentAnimationClip == "Idle")
                            {
                                princeAnimator.SetTrigger("Turn");
                                controlable = false;
                            }
                        }
                    }
                    else if (!isRunningStop)
                    {
                        if (isInteractSomething && !isMoving)
                        {
                            predictPosition = new Vector3(transform.position.x - normalStepScale, transform.position.y, transform.position.z);
                            settingMoveSpeed = normalStepSpeed;
                            StartCoroutine(Step());
                        }
                        else if (runCycleInputCount <= Time.time && !isRunning)
                        {
                            predictPosition = new Vector3(transform.position.x - runStepScale, transform.position.y, transform.position.z);
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
                    predictPosition = new Vector3(transform.position.x - runStopScale, predictPosition.y, predictPosition.z);
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
                        if (isRunningStop)
                        {
                            isRunningStop = false;
                            StopAllCoroutines();
                            StartCoroutine(RuningTurn());
                        }
                        else
                        {
                            if (currentAnimationClip == "Idle")
                            {
                                princeAnimator.SetTrigger("Turn");
                                controlable = false;
                            }
                        }
                    }
                    else if (isJump)
                    {
                        if (currentFacing)
                        {
                            predictPosition = new Vector3(transform.position.x + jumpScale, transform.position.y, transform.position.z);
                            settingMoveSpeed = jumpSpeed;
                            StartCoroutine(JumpIdle());
                        }
                    }
                    else
                    {
                        if (isInteractSomething && !isMoving)
                        {
                            predictPosition = new Vector3(transform.position.x + normalStepScale, transform.position.y, transform.position.z);
                            settingMoveSpeed = normalStepSpeed;
                            StartCoroutine(Step());
                        }
                        else if (runCycleInputCount <= Time.time && !isRunning)
                        {
                            predictPosition = new Vector3(transform.position.x + runStepScale, transform.position.y, transform.position.z);
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
                    predictPosition = new Vector3(transform.position.x + runStopScale, predictPosition.y, predictPosition.z);
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
    /* private IEnumerator ClimbDown()
    {
        controlable = false;
        isCheckingFall = false;
        princeAnimator.SetTrigger("Drop");
        yield return waitForclimDown;
        isCheckingFall = true;
        controlable = true;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
    } */
    private IEnumerator JumpIdle()
    {
        controlable = false;
        isCheckingFall = false;
        isMoving = false;
        princeAnimator.SetTrigger("IdleJump");
        yield return waitForStartJump;
        isMoving = true;
        controlable = true;
    }
    private IEnumerator StartRun()
    {
        isRunning = true;
        runCycleInputCount = Time.time + runCycleInputDelay + runStartDuration;
        princeAnimator.SetBool("Running", true);
        yield return waitForStartRun;
        isMoving = true;
    }
    private IEnumerator StopRun()
    {
        princeAnimator.SetBool("Running", false);
        princeAnimator.SetBool("ForwardBlock", forwardBlock);
        isRunning = false;
        isMoving = true;
        isRunningStop = true;
        yield return waitForStopRun;
        isRunningStop = false;
    }
    private IEnumerator RuningTurn()
    {
        controlable = false;
        isMoving = true;
        isCheckingFall = false;
        characterRigid.isKinematic = true;
        princeAnimator.SetTrigger("RunTurn");
        princeAnimator.SetBool("Running", true);
        yield return waitForRunTurn;
        isRunning = true;
        controlable = true;
        isCheckingFall = true;
        characterRigid.isKinematic = false;
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
