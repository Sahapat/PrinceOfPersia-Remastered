using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prince : CharacterSystem
{
    [Header("StepProperty")]
    [SerializeField] private float normalStepScale;
    [SerializeField] private float normalStepSpeed;
    [Header("CrouchProperty")]
    [SerializeField] private float crouchStepScale;
    [SerializeField] private float crouchStepSpeed;
    [Header("RunProperty")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float runStopScale;
    [SerializeField] private float runStopSpeed;
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
    [SerializeField] private BoxCollider2D upFloorChecker;
    [SerializeField] private BoxCollider2D downFloorChecker;
    //Counter variable
    private float runStartCounter;
    private float runTurnCounter;
    //Other
    private bool isCrouch;
    private bool isRunning;
    private bool isInAction;
    public bool isJump;
    private bool isRunJump;
    private bool isTurning;
    private bool isFromHang;

    [HideInInspector] public bool canInteruptJump;
    private bool isDeadFromFall;
    private bool deadTriggerSet;
    private bool forwardBlock;
    private bool isHaveSword;
    private bool isDrop;
    private bool canHang;
    private bool isHang;
    private byte stepBlockCount;
    private string currentAnimationClip;
    private GameObject interactObject;
    private WaitForSeconds waitForShealth;
    private WaitForSeconds waitForDrawSword;
    private WaitForSeconds waitForFallStun;
    private WaitForSeconds waitForStartJump;
    private WaitForSeconds waitForJump;
    private WaitForSeconds waitForclimbUp;
    private WaitForSeconds waitForclimDown;

    private PrinceAnimationEventHandler princeAnimationEventHandler;
    private PrinceSoundHandler princeSoundHandler;
    private BoxCollider2D princeColider;
    private FloorProperty upInteractFloor;
    private FloorProperty downInteractFloor;
    private Vector3 hangPosition;
    private Vector3 lastPosition;
    private Vector3 nextPosition;
    private bool screamSetTrigger;
    private bool startTrigger;
    private bool isDeadOnFight;
    public void DieSpike()
    {
        GameCore.gameManager.GameEnd();
        Destroy(this.gameObject);
    }
    public void StartRunJump()
    {
        isRunJump = false;
        var increastPosition = (currentFacing) ? jumpScale + 0.5f : -(jumpScale + 0.5f);
        isMoving = true;
        predictPosition = new Vector3(transform.position.x + increastPosition, transform.position.y, transform.position.z);
        settingMoveSpeed = jumpSpeed;
        characterRigid.simulated = false;
        isCheckingFall = false;
        isJump = true;
        princeAnimator.SetBool("isJump", false);
    }
    public void ClimbUpEnd()
    {
        transform.position = nextPosition;
        princeAnimator.SetBool("isHang", false);
        princeAnimator.SetBool("isJump", false);
        controlable = true;
        characterRigid.simulated = true;
        characterRigid.velocity = Vector2.zero;
        isHang = false;
        isFromHang = true;
    }
    public void StartIdleJump()
    {
        var increastPosition = (currentFacing) ? jumpScale : -jumpScale;
        isMoving = true;
        predictPosition = new Vector3(transform.position.x + increastPosition, transform.position.y, transform.position.z);
        settingMoveSpeed = jumpSpeed;
        characterRigid.simulated = false;
        isCheckingFall = false;
        isJump = true;
        princeAnimator.SetBool("isJump", false);
    }
    public void EndJump()
    {
        if (canHang)
        {
            transform.position = hangPosition;
            characterRigid.simulated = false;
            princeAnimator.SetBool("isHang", true);
            controlable = true;
            canHang = false;
            isHang = true;
        }
    }
    public void StartJump()
    {
        fallVelocity = 0;
        princeAnimator.SetFloat("fallVelocity", fallVelocity);
        characterRigid.velocity = new Vector2(0, jumpUpScale);
    }
    public void StartRunCycle()
    {
        isRunning = true;
        runStartCounter = Time.time + 0.2f;
    }
    public void RunTurnOut()
    {
        characterRigid.simulated = true;
        isCheckingFall = true;
        isRunning = true;
        controlable = true;
        isInAction = false;
        isTurning = false;
        princeAnimator.SetBool("RunTurn", false);
    }
    public void IdleStep()
    {
        isMoving = true;
        settingMoveSpeed = normalStepSpeed;
        /* var movingIncreast = (currentFacing) ? normalStepScale : -normalStepScale;
        predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z); */
    }
    public void CrouchStep()
    {
        isMoving = true;
        var movingIncreast = (currentFacing) ? crouchStepScale : -crouchStepScale;
        settingMoveSpeed = crouchStepSpeed;
        predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
    }
    public void SetFloorCheck(bool status)
    {
        isCheckingFall = status;
        characterRigid.simulated = status;
        isJump = false;
    }
    public void SetKinematic(bool status)
    {
        characterRigid.isKinematic = status;
    }
    public void Dead()
    {
        health = 0;
        controlable = false;
        deadTriggerSet = false;
        princeAnimator.SetTrigger("DeadFall");
        GameCore.gameManager.GameEnd();
    }
    protected override void OnAwake()
    {
        base.OnAwake();
        waitForShealth = new WaitForSeconds(fightShealthSwordDuration);
        waitForDrawSword = new WaitForSeconds(fightDrawSwordDuration);
        waitForFallStun = new WaitForSeconds(fallTakeDamageStunDuration);
        waitForStartJump = new WaitForSeconds(jumpStartDuration);
        waitForJump = new WaitForSeconds(jumpDuration);
        waitForclimbUp = new WaitForSeconds(climbUpDuration);
        waitForclimDown = new WaitForSeconds(climbDownDuration);
        princeAnimationEventHandler = GetComponentInChildren<PrinceAnimationEventHandler>();
        princeSoundHandler = GetComponentInChildren<PrinceSoundHandler>();
        princeColider = GetComponent<BoxCollider2D>();
    }
    protected override void OnUpdate()
    {
        var itemHitChecker = Physics2D.OverlapBox(new Vector2(transform.position.x + princeColider.offset.x, transform.position.y + princeColider.offset.y
                                                    ), princeColider.size, 0f, LayerMask.GetMask("Interact"));
        if (itemHitChecker)
        {
            interactObject = itemHitChecker.gameObject;
        }
        else
        {
            interactObject = null;
        }
        var upFloorColiderCheckPos = new Vector2(upFloorChecker.transform.position.x + upFloorChecker.offset.x, upFloorChecker.transform.position.y + upFloorChecker.offset.y);
        var upFloorHit = Physics2D.OverlapBox(upFloorColiderCheckPos, upFloorChecker.size, 0f, LayerMask.GetMask("Floor"));
        if (upFloorHit)
        {
            var temp = upFloorHit.GetComponent<FloorProperty>();
            upInteractFloor = (temp) ? temp : null;
        }
        else
        {
            upInteractFloor = null;
        }
        var downFloorColiderCheckPos = new Vector2(downFloorChecker.transform.position.x + downFloorChecker.offset.x, downFloorChecker.transform.position.y + downFloorChecker.offset.y);
        var downFloorHit = Physics2D.OverlapBox(downFloorColiderCheckPos, downFloorChecker.size, 0f, LayerMask.GetMask("Floor"));
        if (downFloorHit)
        {
            var temp = downFloorHit.GetComponent<FloorProperty>();
            downInteractFloor = (temp) ? temp : null;
        }
        else
        {
            downInteractFloor = null;
        }
        var coliderPos = new Vector2(transform.position.x + princeColider.offset.x, transform.position.y + princeColider.offset.y);
        var wallHit = Physics2D.OverlapBox(coliderPos, princeColider.size, 0f, LayerMask.GetMask("Wall"));
        if (wallHit)
        {
            characterRigid.simulated = true;
            isCheckingFall = true;
            predictPosition = transform.position;
            isMoving = false;
        }
        if (isMoving)
        {
            var predictMoveCheckPos = new Vector2(predictPosition.x + princeColider.offset.x, predictPosition.y + princeColider.offset.y);
            var predictWallHit = Physics2D.OverlapBox(predictMoveCheckPos, princeColider.size, 0f, LayerMask.GetMask("Wall"));
        }
        if (isRunJump)
        {
            var movingIncreast = (currentFacing) ? runSpeed : -runSpeed;
            characterRigid.velocity = new Vector2(movingIncreast, characterRigid.velocity.y);
        }
        var clips = princeAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = clips[0].clip.name;
        GameCore.combatController.isPlayerAttacking = isAttacking;
        GameCore.combatController.isPlayerParring = isParring;
        GameCore.combatController.targetPlayer = this.gameObject;
    }
    protected override void OnTakeDamage()
    {
        print("takeDamage");
        if (actionState != CharacterState.COMBAT)
        {
            health -= 1;
            isDeadOnFight = false;
        }
        else
        {
            isDeadOnFight = true;
        }
        princeAnimator.SetTrigger("TakeDamage");
        isMoving = false;
        isRunning = false;
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
            if (InputManager.GetKeyDown_Interact() && GameCore.combatController.canCombat && isHaveSword)
            {
                StartCoroutine(IdleToCombat());
            }
            else if (InputManager.GetKeyDown_Interact())
            {
                if (interactObject)
                {
                    if (interactObject.CompareTag("PotionSmall"))
                    {
                        transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, transform.position.z);
                        Destroy(interactObject);
                        princeAnimator.SetTrigger("Heal");
                        StartCoroutine(potionSmallPlay());
                    }
                    else if (interactObject.CompareTag("PotionLarge"))
                    {
                        transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, transform.position.z);
                        Destroy(interactObject);
                        princeAnimator.SetTrigger("Heal");
                        StartCoroutine(potionLargePlay());
                    }
                    else if (interactObject.CompareTag("Sword"))
                    {
                        transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, transform.position.z);
                        isHaveSword = true;
                        Destroy(interactObject);
                        princeAnimator.SetTrigger("GetSword");
                        princeSoundHandler.swordGetPlay();
                    }
                }
            }
            else if (!InputManager.GetKey_Interact())
            {
                if (isHang && !InputManager.GetKey_Up())
                {
                    princeAnimator.SetBool("isHang", false);
                    characterRigid.simulated = true;
                    isCheckingFall = true;
                    isHang = false;
                    isFromHang = true;
                    transform.position = lastPosition;
                }
                else if (isHang && InputManager.GetKey_Up())
                {
                    princeAnimator.SetTrigger("ClimbUp");
                    controlable = false;
                }
            }
            #endregion
            #region Keydown Implements
            if (InputManager.GetKey_Down())
            {
                if (!isCrouch)
                {
                    ClimbDownChecking();
                }
            }
            else if (!InputManager.GetKey_Down())
            {
                if (isCrouch)
                {
                    isCrouch = false;
                    princeAnimator.SetBool("Crouch", false);
                }
            }
            #endregion
            #region KeyUp Implement
            if (InputManager.GetKey_Up() && !isHang)
            {
                if (upInteractFloor)
                {
                    ClimbUpChecking();
                }
                else if (currentAnimationClip == "Idle")
                {
                    princeAnimator.SetBool("isJump", true);
                    canInteruptJump = true;
                }
            }
            else if (!InputManager.GetKey_Up())
            {
            }
            if (InputManager.GetKeyDown_Up())
            {
            }
            #endregion
            #region KeyLeft Implement
            if (InputManager.GetKeyDown_Left())
            {
                if (isCrouch)
                {
                    if (!currentFacing)
                    {
                        princeAnimator.SetTrigger("CrouchStep");
                        stepBlockCount = 0;
                    }
                }
                else
                {
                    if (!currentFacing && InputManager.GetKey_Interact() && !isMoving)
                    {
                        var movingIncreast = (currentFacing) ? normalStepScale : -normalStepScale;
                        predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
                        if (downInteractFloor && stepBlockCount < 2)
                        {
                            if (downInteractFloor.GetLeftSideInteract())
                            {
                                if (predictPosition.x < downInteractFloor.GetLeftSideEdge().x + 0.3f)
                                {
                                    predictPosition = new Vector3(downInteractFloor.GetLeftSideEdge().x + 0.3f, predictPosition.y, predictPosition.z);
                                    if ((int)transform.position.x == (int)(downInteractFloor.GetLeftSideEdge().x + 0.3f))
                                    {
                                        princeAnimator.SetTrigger("StepBlock");
                                        stepBlockCount++;
                                    }
                                    else
                                    {
                                        princeAnimator.SetTrigger("Step");
                                    }
                                    isInAction = true;
                                }
                                else
                                {
                                    princeAnimator.SetTrigger("Step");
                                    isInAction = true;
                                }
                            }
                            else if (downInteractFloor.GetRightSideInteract())
                            {
                                if (predictPosition.x > downInteractFloor.GetRightSideEdge().x - 0.3f)
                                {
                                    predictPosition = new Vector3(downInteractFloor.GetRightSideEdge().x - 0.3f, predictPosition.y, predictPosition.z);
                                    if ((int)transform.position.x == (int)(downInteractFloor.GetLeftSideEdge().x - 0.3f))
                                    {
                                        princeAnimator.SetTrigger("StepBlock");
                                        stepBlockCount++;
                                    }
                                    else
                                    {
                                        princeAnimator.SetTrigger("Step");
                                    }
                                    isInAction = true;
                                }
                                else
                                {
                                    princeAnimator.SetTrigger("Step");
                                    isInAction = true;
                                }
                            }
                        }
                        else
                        {
                            princeAnimator.SetTrigger("Step");
                            isInAction = true;
                        }
                    }
                    else if (currentFacing)
                    {
                        if (isTurning)
                        {
                            princeAnimator.SetTrigger("Turn");
                            isInAction = true;
                        }
                    }
                }
            }
            if (InputManager.GetKey_Left() && !isCrouch && !isInAction)
            {
                if (canInteruptJump && InputManager.GetKey_Up() && !currentFacing && currentAnimationClip == "Jump")
                {
                    princeAnimator.SetTrigger("IdleJump");
                    princeAnimator.SetBool("isJump", false);
                    controlable = false;
                    canInteruptJump = false;
                }
                else if (isRunning && InputManager.GetKey_Up() && !currentFacing)
                {
                    princeAnimator.SetTrigger("RunJump");
                    controlable = false;
                    isRunJump = true;
                }

                else if (isRunning)
                {
                    var movingIncreast = (currentFacing) ? runSpeed : -runSpeed;
                    characterRigid.velocity = new Vector2(movingIncreast, characterRigid.velocity.y);
                }
                else
                {
                    if (!currentFacing)
                    {
                        princeAnimator.SetBool("Running", true);
                        stepBlockCount = 0;
                    }
                    else if (currentFacing)
                    {
                        princeAnimator.SetTrigger("Turn");
                        isInAction = true;
                    }
                }
            }
            else if (!InputManager.GetKey_Left() && isRunning && !currentFacing)
            {
                if (InputManager.GetKey_Right())
                {
                    isMoving = true;
                    isTurning = true;
                    settingMoveSpeed = runStopSpeed - 0.6f;
                    characterRigid.simulated = false;
                    isCheckingFall = false;
                    princeAnimator.SetBool("RunTurn", true);
                    characterRigid.velocity = Vector2.zero;
                    var movingIncreast = (currentFacing) ? runStopScale + 0.3f : -(runStopScale + 0.3f);
                    predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
                }
                else if (runStartCounter <= Time.time)
                {
                    isRunning = false;
                    isMoving = true;
                    settingMoveSpeed = runStopSpeed;
                    characterRigid.velocity = Vector2.zero;
                    princeAnimator.SetBool("Running", false);
                    princeAnimator.SetBool("ForwardBlock", forwardBlock);
                    var movingIncreast = (currentFacing) ? runStopScale : -runStopScale;
                    predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
                }
                else
                {
                    var movingIncreast = (currentFacing) ? runSpeed : -runSpeed;
                    characterRigid.velocity = new Vector2(movingIncreast, characterRigid.velocity.y);
                    runStartCounter = Time.time;
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
                        princeAnimator.SetTrigger("CrouchStep");
                        stepBlockCount = 0;
                    }
                }
                else
                {
                    if (currentFacing && InputManager.GetKey_Interact())
                    {
                        var movingIncreast = (currentFacing) ? normalStepScale : -normalStepScale;
                        predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
                        if (downInteractFloor && stepBlockCount < 2)
                        {
                            if (downInteractFloor.GetLeftSideInteract())
                            {
                                if (predictPosition.x < downInteractFloor.GetLeftSideEdge().x + 0.3f)
                                {
                                    predictPosition = new Vector3(downInteractFloor.GetLeftSideEdge().x + 0.3f, predictPosition.y, predictPosition.z);
                                    if ((int)transform.position.x == (int)(downInteractFloor.GetLeftSideEdge().x + 0.3f))
                                    {
                                        princeAnimator.SetTrigger("StepBlock");
                                        stepBlockCount++;
                                    }
                                    else
                                    {
                                        princeAnimator.SetTrigger("Step");
                                    }
                                    isInAction = true;
                                }
                                else
                                {
                                    princeAnimator.SetTrigger("Step");
                                    isInAction = true;
                                }
                            }
                            else if (downInteractFloor.GetRightSideInteract())
                            {
                                if (predictPosition.x > downInteractFloor.GetRightSideEdge().x - 0.3f)
                                {
                                    predictPosition = new Vector3(downInteractFloor.GetRightSideEdge().x - 0.3f, predictPosition.y, predictPosition.z);
                                    if ((int)transform.position.x == (int)(downInteractFloor.GetRightSideEdge().x - 0.3f))
                                    {
                                        princeAnimator.SetTrigger("StepBlock");
                                        stepBlockCount++;
                                    }
                                    else
                                    {
                                        princeAnimator.SetTrigger("Step");
                                    }
                                    isInAction = true;
                                }
                                else
                                {
                                    princeAnimator.SetTrigger("Step");
                                    isInAction = true;
                                }
                            }
                        }
                        else
                        {
                            princeAnimator.SetTrigger("Step");
                            isInAction = true;
                        }
                    }
                    else if (!currentFacing)
                    {
                        princeAnimator.SetTrigger("Turn");
                        isInAction = true;
                    }
                }
            }
            if (InputManager.GetKey_Right() && !isCrouch && !isInAction)
            {
                if (canInteruptJump && InputManager.GetKey_Up() && currentFacing && currentAnimationClip == "Jump")
                {
                    princeAnimator.SetTrigger("IdleJump");
                    princeAnimator.SetBool("isJump", false);
                    controlable = false;
                    canInteruptJump = false;
                }
                else if (isRunning && InputManager.GetKey_Up() && currentFacing)
                {
                    princeAnimator.SetTrigger("RunJump");
                    controlable = false;
                    isRunJump = true;
                }
                else if (isRunning)
                {
                    var movingIncreast = (currentFacing) ? runSpeed : -runSpeed;
                    characterRigid.velocity = new Vector2(movingIncreast, characterRigid.velocity.y);
                }
                else
                {
                    if (currentFacing)
                    {
                        princeAnimator.SetBool("Running", true);
                        stepBlockCount = 0;
                    }
                    else if (!currentFacing)
                    {
                        princeAnimator.SetTrigger("Turn");
                        isInAction = true;
                    }
                }
            }
            else if (!InputManager.GetKey_Right() && isRunning && currentFacing)
            {
                if (InputManager.GetKey_Left())
                {
                    isMoving = true;
                    isTurning = true;
                    settingMoveSpeed = runStopSpeed - 0.6f;
                    characterRigid.simulated = false;
                    isCheckingFall = false;
                    princeAnimator.SetBool("RunTurn", true);
                    characterRigid.velocity = Vector2.zero;
                    var movingIncreast = (currentFacing) ? runStopScale + 0.3f : -(runStopScale + 0.3f);
                    predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
                }
                if (!isTurning)
                {
                    if (runStartCounter <= Time.time)
                    {
                        isRunning = false;
                        isMoving = true;
                        settingMoveSpeed = runStopSpeed;
                        princeAnimator.SetBool("Running", false);
                        princeAnimator.SetBool("ForwardBlock", forwardBlock);
                        characterRigid.velocity = Vector2.zero;
                        var movingIncreast = (currentFacing) ? runStopScale : -runStopScale;
                        predictPosition = new Vector3(transform.position.x + movingIncreast, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        var movingIncreast = (currentFacing) ? runSpeed : -runSpeed;
                        characterRigid.velocity = new Vector2(movingIncreast, characterRigid.velocity.y);
                        runStartCounter = Time.time;
                    }
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
            if (isDeadOnFight)
            {
                StartCoroutine(deathFightSoundPlay());
            }
            else
            {
                StartCoroutine(deathSoundPlay());
            }
        }
        GameCore.gameManager.GameEnd();
    }
    protected override void OnStartFall()
    {
        base.OnStartFall();
        princeAnimator.SetTrigger("Fall");
        princeAnimator.SetFloat("distanceBetweenFloor", distanceBetweenPoint);
        princeAnimator.SetBool("Running", false);
        characterRigid.velocity = new Vector2(0, characterRigid.velocity.y);
        var fallPosIncreast = (currentFacing) ? 0.35f : -0.35f;
        if (!isMoving)
        {
            fallPosIncreast = 0;
        }
        stepBlockCount = 0;
        transform.position = new Vector3(transform.position.x + fallPosIncreast, transform.position.y, transform.position.z);
        isMoving = false;
        isRunning = false;
        isRunJump = false;
    }
    protected override void OnFall()
    {
        fallVelocity = characterRigid.velocity.y;
        if (fallVelocity < -14f && screamSetTrigger)
        {
            princeSoundHandler.screamPlay();
            screamSetTrigger = false;
        }
    }
    protected override void OnStopFall()
    {
        base.OnStopFall();
        if (health != 0)
        {
            princeAnimator.SetFloat("distanceBetweenFloor", distanceBetweenPoint);
            princeAnimator.SetTrigger("toFloor");
            princeAnimator.SetFloat("fallVelocity", fallVelocity);
            if (fallVelocity < -9.8f)
            {
                var damageTake = Mathf.Clamp(fallDamageTaken - 1, 0, 10);
                if (damageTake <= 0)
                {
                    princeSoundHandler.landSoftPlay();
                    if (startTrigger)
                    {
                        StartCoroutine(startSoundPlay());
                        startTrigger = false;
                    }
                }
                else
                {
                    princeSoundHandler.landharmPlay();
                }
                StartCoroutine(FallToStun());
            }
            else if (distanceBetweenPoint < 1 && !isFromHang)
            {
                var increastPosition = (currentFacing) ? 0.3f : -0.3f;
                transform.Translate(new Vector3(increastPosition, 0, 0));
            }
            else if (isFromHang)
            {
                isFromHang = false;
            }
            else
            {
                princeSoundHandler.landSoftPlay();
            }
        }
        else
        {
            isDeadFromFall = true;
        }
        princeAnimator.SetBool("isJump", false);
        isMoving = false;
        screamSetTrigger = true;
    }
    protected override void OnStart()
    {
        base.OnStart();
        deadTriggerSet = true;
        screamSetTrigger = true;
        startTrigger = true;
    }
    public override void SetControlable(bool status)
    {
        base.SetControlable(status);
        if (controlable)
        {
            isInAction = false;
        }
    }
    private void ClimbDownChecking()
    {
        if (downInteractFloor.GetRightSideInteract() && !currentFacing)
        {
            /* if (Mathf.Abs(transform.position.x - downInteractFloor.GetRightSideEdge().x) < 0.3f)
            {
                princeAnimator.SetTrigger("Drop");
                isCheckingFall = false;
                characterRigid.isKinematic = true;
            }
            else
            { */
            isCrouch = true;
            princeAnimator.SetBool("Crouch", true);
            /* } */
        }
        else if (downInteractFloor.GetLeftSideInteract() && currentFacing)
        {
            /* if (Mathf.Abs(transform.position.x - downInteractFloor.GetLeftSideEdge().x) < 0.3f)
            {
                princeAnimator.SetTrigger("Drop");
                isCheckingFall = false;
                characterRigid.isKinematic = true;
            }
            else
            { */
            isCrouch = true;
            princeAnimator.SetBool("Crouch", true);
            /* } */
        }
        else
        {
            isCrouch = true;
            princeAnimator.SetBool("Crouch", true);
        }
    }
    private bool ClimbUpChecking()
    {
        if (upInteractFloor.GetLeftSideInteract() && currentFacing)
        {
            canHang = true;
            lastPosition = new Vector3(upInteractFloor.GetLeftSideEdge().x - 0.35f, transform.position.y, transform.position.z);
            nextPosition = new Vector3(upInteractFloor.GetLeftSideEdge().x + 0.4f, upInteractFloor.GetLeftSideEdge().y + 0.69f, transform.position.z);
            transform.position = new Vector3(upInteractFloor.GetLeftSideEdge().x - 0.35f, transform.position.y, transform.position.z);
            hangPosition = new Vector3(upInteractFloor.GetLeftSideEdge().x - 0.3f, upInteractFloor.GetLeftSideEdge().y - 1, transform.position.z);
            princeAnimator.SetBool("isJump", true);
            isCheckingFall = false;
            isMoving = false;
        }
        if (upInteractFloor.GetRightSideInteract() && !currentFacing)
        {
            canHang = true;
            lastPosition = new Vector3(upInteractFloor.GetRightSideEdge().x + 0.35f, transform.position.y, transform.position.z);
            nextPosition = new Vector3(upInteractFloor.GetRightSideEdge().x - 0.4f, upInteractFloor.GetRightSideEdge().y + 0.69f, transform.position.z);
            transform.position = new Vector3(upInteractFloor.GetRightSideEdge().x + 0.35f, transform.position.y, transform.position.z);
            hangPosition = new Vector3(upInteractFloor.GetRightSideEdge().x + 0.3f, upInteractFloor.GetRightSideEdge().y - 1, transform.position.z);
            princeAnimator.SetBool("isJump", true);
            isCheckingFall = false;
            isMoving = false;
        }
        return true;
    }
    private IEnumerator FallToStun()
    {
        yield return waitForFallStun;
        princeAnimator.SetTrigger("GetUp");
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
    private IEnumerator startSoundPlay()
    {
        startTrigger = false;
        yield return new WaitForSeconds(1.2f);
        GameCore.gameManager.startSoundPlay();
    }
    private IEnumerator deathSoundPlay()
    {
        deadTriggerSet = false;
        yield return new WaitForSeconds(1.2f);
        GameCore.gameManager.deathSoundPlay();
    }
    private IEnumerator deathFightSoundPlay()
    {
        deadTriggerSet = false;
        yield return new WaitForSeconds(1.2f);
        GameCore.gameManager.FightdeathSoundPlay();
    }
    private IEnumerator potionSmallPlay()
    {
        health += 1;
        yield return new WaitForSeconds(1.6f);
        princeSoundHandler.PotionSmallPlay();
    }
    private IEnumerator potionLargePlay()
    {
        health = MaxHealth;
        yield return new WaitForSeconds(1.6f);
        princeSoundHandler.PotionBigPlay();
    }
}
