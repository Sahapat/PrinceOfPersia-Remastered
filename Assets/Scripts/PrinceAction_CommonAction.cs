using UnityEngine;

public class PrinceAction_CommonAction : MonoBehaviour
{
    [SerializeField] private PrinceAnimationController princeAnimationController;
    [SerializeField] private float runInputDelay;
    [SerializeField] private float runStepScale;
    [SerializeField] private float runSpeed;
    [SerializeField] private float crouchStepScale;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpDelay;
    [SerializeField] private float jumpScale;
    private float predictPositionX;
    private float countSelectTimeForMove;
    private float countSelectTimeForJump;
    private bool isRunning;
    private bool isStalking;
    private bool isChargeJump;
    private bool isRuningTurn;
    private AnimatorClipInfo[] animatorClipInfo;
    private string currentAnimationClip;

    private Rigidbody2D prince_rigidbody;

    void Awake()
    {
        prince_rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (InputManager.getInputKeyDown_Right())
        {
            if (isRunning && princeAnimationController.GetCurrentFacing() != PrinceAnimationController.SideFacing.Right && !isStalking)
            {
                isRuningTurn = true;
                predictPositionX = transform.position.x;
                princeAnimationController.FlipFacing();
            }
            if (princeAnimationController.GetCurrentActionState() != PrinceAnimationController.ActionState.FLIPING)
            {
                if (princeAnimationController.GetCurrentActionState() == PrinceAnimationController.ActionState.CROUCH)
                {
                    if (princeAnimationController.GetCurrentFacing() == PrinceAnimationController.SideFacing.Right)
                    {
                        princeAnimationController.CrouchMove();
                        predictPositionX = transform.position.x + crouchStepScale;
                        isRunning = false;
                        isStalking = true;
                    }
                }
                else
                {
                    if (princeAnimationController.GetCurrentFacing() != PrinceAnimationController.SideFacing.Right)
                    {
                        princeAnimationController.FlipFacing();
                        return;
                    }
                    countSelectTimeForMove = Time.time + runInputDelay;
                    predictPositionX = transform.position.x + runStepScale;
                    isRunning = true;
                    isStalking = false;
                }
            }
        }
        if (InputManager.getInputKey_Right() && isRunning && countSelectTimeForMove <= Time.time)
        {
            if (princeAnimationController.GetCurrentActionState() != PrinceAnimationController.ActionState.FLIPING)
            {
                countSelectTimeForMove = Time.time + runInputDelay;
                predictPositionX = transform.position.x + runStepScale;
                isRunning = true;
                isStalking = false;
            }
        }
        if (InputManager.getInputKey_Left() && isRunning && countSelectTimeForMove <= Time.time)
        {
            if (princeAnimationController.GetCurrentActionState() != PrinceAnimationController.ActionState.FLIPING)
            {
                countSelectTimeForMove = Time.time + runInputDelay;
                predictPositionX = transform.position.x - runStepScale;
                isRunning = true;
                isStalking = false;
            }
        }
        if (InputManager.getInputKeyDown_Left())
        {
            if (princeAnimationController.GetCurrentActionState() != PrinceAnimationController.ActionState.FLIPING)
            {
                if (princeAnimationController.GetCurrentActionState() == PrinceAnimationController.ActionState.CROUCH)
                {
                    if (princeAnimationController.GetCurrentFacing() == PrinceAnimationController.SideFacing.Left)
                    {
                        princeAnimationController.CrouchMove();
                        predictPositionX = transform.position.x - crouchStepScale;
                        isRunning = false;
                        isStalking = true;
                    }
                }
                else
                {
                    if (princeAnimationController.GetCurrentFacing() != PrinceAnimationController.SideFacing.Left)
                    {
                        princeAnimationController.FlipFacing();
                        return;
                    }
                    countSelectTimeForMove = Time.time + runInputDelay;
                    predictPositionX = transform.position.x - runStepScale;
                    isRunning = true;
                    isStalking = false;
                }
            }
        }
        if (InputManager.getInputKey_Down())
        {
            princeAnimationController.SetAnimationCrouch();
        }
        if (InputManager.getInputKeyUp_Down())
        {
            princeAnimationController.SetAnimationCrouchStop();
        }
        DoJump();
        DoMovement();
    }
    private void DoJump()
    {
        if (InputManager.getInputKeyDown_Up())
        {
            princeAnimationController.Jump();
            isChargeJump = true;
            countSelectTimeForJump = Time.time + jumpDelay;
        }
        if (InputManager.getInputKey_Up() && isChargeJump && countSelectTimeForJump <= Time.time)
        {
            prince_rigidbody.velocity = new Vector2(prince_rigidbody.velocity.x, jumpScale);
            isChargeJump = false;
        }
        if (InputManager.getInputKeyUp_Up())
        {
            isChargeJump = false;
            princeAnimationController.CancelJump();
        }
    }
    private void DoMovement()
    {
        if (isRunning)
        {
            if (Mathf.Abs(transform.position.x - predictPositionX) >= 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(predictPositionX, transform.position.y, transform.position.z), Time.deltaTime * runSpeed);
                princeAnimationController.SetAnimationRunning();
            }
            else
            {
                transform.position = new Vector3(predictPositionX, transform.position.y, transform.position.z);
                isRunning = false;
                princeAnimationController.SetAnimationRunStop();
            }
        }
        else if (isStalking)
        {
            if (Mathf.Abs(transform.position.x - predictPositionX) >= 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(predictPositionX, transform.position.y, transform.position.z), Time.deltaTime * crouchSpeed);
            }
            else
            {
                transform.position = new Vector3(predictPositionX, transform.position.y, transform.position.z);
                isStalking = false;
            }
        }
    }
}