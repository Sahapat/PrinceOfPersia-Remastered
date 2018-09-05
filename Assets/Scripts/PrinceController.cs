using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceController : MonoBehaviour
{
    public enum SideFacing
    {
        Left,
        Right
    };
    public enum ActionState
    {
        IDLE,
        COMMON_ACTION,
        COMBAT,
        OTHER
    };

    private ActionState actionState;
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
        isReadyToTurn = true;
    }
    void Update()
    {
        var animationStateInfo = princeAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = animationStateInfo[0].clip.name;
    }
    public void SetAnimationRunning()
    {
        princeAnimator.SetBool("isRun",true);
    }
    public void SetAnimationRunStop()
    {
        princeAnimator.SetBool("isRun",false);
    }
    public void FlipFacing()
    {
        if(isReadyToTurn)
        StartCoroutine(Fliping());
    }
    public SideFacing GetCurrentFacing()
    {
        return (princeSpriteRenderer.flipX) ? SideFacing.Left : SideFacing.Right;
    }
    public ActionState GetCurrentActionState()
    {
        return actionState;
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
    }
}