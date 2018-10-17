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
    private bool isJump;
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
    private BoxCollider2D princeColider;
    private FloorProperty upInteractFloor;
    private FloorProperty downInteractFloor;
    public void StartJump()
    {
        characterRigid.velocity = new Vector2(0, jumpScale);
        isJump = true;
    }
    public void DropEnd()
    {
        isCheckingFall = true;
        characterRigid.isKinematic = false;
        princeAnimator.SetBool("Drop", false);
        var decreastPosition = (currentFacing) ? 0.2f : -0.2f;
        transform.position = new Vector3(transform.position.x + decreastPosition, transform.position.y, transform.position.z);
        canHang = false;
    }
    public void DropHang()
    {
        var decreastPosition = (currentFacing) ? -0.4f : 0.4f;
        transform.position = new Vector3(transform.position.x + decreastPosition, transform.position.y, transform.position.z);
        canHang = true;
        StartCoroutine(dropDown());
    }
    public void StartRunCycle()
    {
        isRunning = true;
        runStartCounter = Time.time + 0.2f;
    }
    public void RunTurnOut()
    {
        characterRigid.isKinematic = false;
        isCheckingFall = true;
        isRunning = true;
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
    public void SetFloorCheck(bool status)
    {
        isCheckingFall = status;
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
        if (isMoving)
        {
            var predictMoveCheckPos = new Vector2(predictPosition.x + princeColider.offset.x, predictPosition.y + princeColider.offset.y);
            var predictWallHit = Physics2D.OverlapBox(predictMoveCheckPos, princeColider.size, 0f, LayerMask.GetMask("Wall"));
            var predictFloorHit = Physics2D.OverlapBox(predictMoveCheckPos, princeColider.size, 0f, LayerMask.GetMask("Floor"));
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
                        health += 1;
                    }
                    else if (interactObject.CompareTag("PotionLarge"))
                    {
                        transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, transform.position.z);
                        Destroy(interactObject);
                        princeAnimator.SetTrigger("Heal");
                        MaxHealth += 1;
                        health = MaxHealth;
                    }
                    else if (interactObject.CompareTag("Sword"))
                    {
                        transform.position = new Vector3(interactObject.transform.position.x, transform.position.y, transform.position.z);
                        isHaveSword = true;
                        Destroy(interactObject);
                        princeAnimator.SetTrigger("GetSword");
                    }
                }
            }
            #endregion
            #region Keydown Implements
            if (InputManager.GetKey_Down())
            {
                if (!isCrouch)
                {
                    if (downInteractFloor.GetRightSideInteract() && !currentFacing)
                    {
                        if (Mathf.Abs(transform.position.x - downInteractFloor.GetRightSideEdge().x) < 0.3f)
                        {
                            princeAnimator.SetTrigger("Drop");
                            isCheckingFall = false;
                            characterRigid.isKinematic = true;
                        }
                        else
                        {
                            isCrouch = true;
                            princeAnimator.SetBool("Crouch", true);
                        }
                    }
                    else
                    {
                        isCrouch = true;
                        princeAnimator.SetBool("Crouch", true);
                    }
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
            if (InputManager.GetKey_Up())
            {
                princeAnimator.SetTrigger("Jump");
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
                        princeAnimator.SetTrigger("Turn");
                        isInAction = true;
                    }
                }
            }
            if (InputManager.GetKey_Left() && !isCrouch && !isInAction)
            {
                if (isRunning)
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
                if (runStartCounter <= Time.time)
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
                if (isRunning)
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
        if (!isJump)
        {
            princeAnimator.SetTrigger("Fall");
            princeAnimator.SetBool("Running", false);
            characterRigid.velocity = Vector2.zero;
            var fallPosIncreast = (currentFacing) ? 0.35f : -0.35f;
            if (!isMoving)
            {
                fallPosIncreast = 0;
            }
            stepBlockCount = 0;
            transform.position = new Vector3(transform.position.x + fallPosIncreast, transform.position.y, transform.position.z);
            isMoving = false;
            isRunning = false;
        }
    }
    protected override void OnStopFall()
    {
        base.OnStopFall();
        if (health != 0)
        {
            if (fallDamageTaken <= 1)
            {
                princeAnimator.SetTrigger("FallNotTakeDamage");
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
    public override void SetControlable(bool status)
    {
        base.SetControlable(status);
        if (controlable)
        {
            isInAction = false;
        }
    }
    private IEnumerator dropDown()
    {
        for (int i = 0; i < 10; i++)
        {
            transform.Translate(new Vector3(0, -0.18f, 0));
            yield return new WaitForSeconds(0.03f);
        }
    }
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
    private IEnumerator FallToStun()
    {
        princeAnimator.SetTrigger("FallTakeDamage");
        yield return waitForFallStun;
        princeAnimator.SetTrigger("CrouchOut");
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
