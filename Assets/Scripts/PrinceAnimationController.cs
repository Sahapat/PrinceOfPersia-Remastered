using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceAnimationController : MonoBehaviour
{
    public enum SideFacing
    {
        Left,
        Right
    };
    public enum ActionState
    {
        IDLE,
        FLIPING,
        RUNNING,
        CROUCH,
        JUMP
    };

    [SerializeField]private ActionState currentState;
    private ActionState previousState;
    private PrinceAction_CommonAction commonAction;
    private PrinceAction_Combat combat;
    private PrinceAction_Other other;

    [SerializeField] private Animator princeAnimator;
    [SerializeField] private SpriteRenderer princeSpriteRenderer;

    private string currentAnimationClip;
    private bool isReadyToTurn;
    void Awake()
    {
        commonAction = GetComponent<PrinceAction_CommonAction>();
        combat = GetComponent<PrinceAction_Combat>();
        other = GetComponent<PrinceAction_Other>();
    }
    void Start()
    {
        previousState = currentState;
        currentState = ActionState.IDLE;
        isReadyToTurn = true;
    }
    void Update()
    {
        var animationStateInfo = princeAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = animationStateInfo[0].clip.name;
    }
    public void SetAnimationCrouch()
    {
        princeAnimator.SetBool("isCrouch",true);
        previousState = currentState;
        currentState = ActionState.CROUCH;
    }
    public void SetAnimationCrouchStop()
    {
        princeAnimator.SetBool("isCrouch",false);
        previousState = currentState;
        currentState = ActionState.IDLE;
    }
    public void SetAnimationRunning()
    {
        princeAnimator.SetBool("isRun",true);
        previousState = currentState;
        currentState = ActionState.RUNNING;
    }
    public void SetAnimationRunStop()
    {
        princeAnimator.SetBool("isRun",false);
        previousState = currentState;
        currentState = ActionState.IDLE;
    }
    public void Jump()
    {
        princeAnimator.SetBool("isJump",true);
        previousState = currentState;
        currentState = ActionState.JUMP;
    }
    public void CancelJump()
    {
        princeAnimator.SetBool("isJump",false);
        previousState = currentState;
        currentState = ActionState.IDLE;
    }
    public void CrouchMove()
    {
        princeAnimator.SetTrigger("CrouchMove");
    }
    public void FlipFacing()
    {
        previousState = currentState;
        currentState = ActionState.FLIPING;
        if(isReadyToTurn)
        StartCoroutine(Fliping());
    }
    public SideFacing GetCurrentFacing()
    {
        return (princeSpriteRenderer.flipX) ? SideFacing.Left : SideFacing.Right;
    }
    public ActionState GetPreviousActionState()
    {
        return previousState;
    }
    public ActionState GetCurrentActionState()
    {
        return currentState;
    }
    public string GetCurrentAnimationClip()
    {
        return currentAnimationClip;
    }
    private IEnumerator Fliping()
    {
        isReadyToTurn = false;
        princeAnimator.SetTrigger("Turn");
        yield return new WaitForSeconds(0.64f);
        princeSpriteRenderer.flipX = !princeSpriteRenderer.flipX;
        isReadyToTurn = true;
        currentState = previousState;
        previousState = ActionState.FLIPING;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(currentState == ActionState.JUMP)
        {
            CancelJump();
        }
    }
}