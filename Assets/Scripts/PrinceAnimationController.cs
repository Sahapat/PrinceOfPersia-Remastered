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
        STALK,
        CROUCH,
        COMBAT
    };

    [SerializeField] private Animator princeAnimator;
    [SerializeField] private SpriteRenderer princeSpriteRenderer;
    [SerializeField] private float turnFlipDelay;
    public bool isFliping = false;
    private PrinceAction_CommonAction princeAction_CommonAction;
    private PrinceAction_Combat princeAction_Combat;
    private PrinceAction_Other princeAction_Other;
    private ActionState currentState;
    private string currentAnimationClip;
    private void Awake()
    {
        princeAction_CommonAction = GetComponent<PrinceAction_CommonAction>();
        princeAction_Combat = GetComponent<PrinceAction_Combat>();
        princeAction_Other =GetComponent<PrinceAction_Other>();
    }
    public void IdleFlipFacing()
    {
        isFliping = true;
        StartCoroutine(IdleFlip());
    }
    public void TakeDamage()
    {
        princeAnimator.SetTrigger("TakeDamage");
    }
    public void Attack()
    {
        princeAnimator.SetTrigger("Attack1");
    }
    public void CombatMoveLeft()
    {
        princeAnimator.SetTrigger("FightMoveLeft");
    }
    public void CombatMoveRight()
    {
        princeAnimator.SetTrigger("FightMoveRight");
    }
    public void SetAnimationRuning(bool status)
    {
        princeAnimator.SetBool("isRun",status);
    }
    public void ToCombat()
    {
        princeAnimator.SetTrigger("ToCombat");
        currentState = ActionState.COMBAT;
    }
    private IEnumerator IdleFlip()
    {
        princeAnimator.SetTrigger("Turn");
        yield return new WaitForSeconds(turnFlipDelay);
        princeSpriteRenderer.flipX = !princeSpriteRenderer.flipX;
        isFliping = false;
    }

    ///////////////////////////// Get property ///////////////////////////////////
    public SideFacing GetCurrentFacing()
    {
        return (princeSpriteRenderer.flipX) ? SideFacing.Left : SideFacing.Right;
    }
    public ActionState GetCurrentActionState()
    {
        return currentState;
    }
    public string GetCurrentAnimationClip()
    {
        var animationStateInfo = princeAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = animationStateInfo[0].clip.name;
        return currentAnimationClip;
    }
}