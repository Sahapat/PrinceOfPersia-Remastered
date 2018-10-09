using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : CharacterSystem
{
    protected enum AIActionPhase
    {
        NONE,
        MOVETOPLAYER,
        MOVEBACK,
        ATTACK
    };
    [Header("EnemyProperty")]
    [SerializeField] private float savePosition;
    [SerializeField] private float attackPosition;
    [SerializeField] private bool haveAttackPredict;
    [SerializeField, Range(0, 100)] private byte fightAttackChance;
    [SerializeField] private float fightAttackDelay;
    [SerializeField, Range(0, 100)] private byte fightParryChance;
    [SerializeField] private float fightParryDelay;
    [SerializeField] private float destroyAfterDeadDuration;
    [Header("Other")]
    [SerializeField] private Animator enemyAnim;
    //Counter Variable
    private float fightParryDelayCounter;
    private float fightAttackDelayCounter;

    private string currentAnimationClip;
    private bool deadTriggerSet;
    private bool isMoveAndAttack;
    private bool isMoveBack;
    private AIActionPhase aIActionPhase;
    private WaitForSeconds waitForSworddraw;
    private bool isReayAttack;
    private float distanceBetweenPlayer;

    protected override void OnAwake()
    {
        base.OnAwake();
        waitForSworddraw = new WaitForSeconds(0.1f);
    }
    protected override void OnStart()
    {
        base.OnStart();
        aIActionPhase = AIActionPhase.NONE;
        isReayAttack = true;
    }
    protected override void OnNormal()
    {
        if (controlable)
        {
            if (GameCore.combatController.canCombat)
            {
                StartCoroutine(ToCombat());
            }
        }
    }
    protected override void OnCombat()
    {
        base.OnCombat();
        if (controlable)
        {
            if (GameCore.combatController.canCombat)
            {
                switch (aIActionPhase)
                {
                    case AIActionPhase.MOVETOPLAYER:
                        if (!isMoveAndAttack)
                        {
                            if (distanceBetweenPlayer > savePosition)
                            {
                                isMoving = true;
                                FightStep(-1);
                                enemyAnim.SetBool("MoveForward", true);
                            }
                            else
                            {
                                enemyAnim.SetBool("MoveForward", false);
                                if (haveAttackPredict)
                                {
                                    if (!GameCore.combatController.isPlayerAttacking)
                                    {
                                        isMoveAndAttack = true;
                                    }
                                }
                                else
                                {
                                    isMoveAndAttack = true;
                                }
                            }
                        }
                        else
                        {
                            if (distanceBetweenPlayer > attackPosition)
                            {
                                isMoving = true;
                                FightStep(-1);
                                enemyAnim.SetBool("MoveForward", true);
                            }
                            else
                            {
                                enemyAnim.SetBool("MoveForward", false);
                                aIActionPhase = AIActionPhase.ATTACK;
                            }
                        }
                        break;
                    case AIActionPhase.ATTACK:
                        if (GameCore.combatController.isPlayerAttacking && fightParryDelayCounter <= Time.time && isAttacking)
                        {
                            fightParryDelayCounter = Time.time + fightParryDelay;
                            if (Random.value <= fightParryChance)
                            {
                                FightParry();
                            }
                        }
                        else if (isReayAttack)
                        {
                            if (Random.value <= fightAttackChance && fightAttackDelayCounter <= Time.time)
                            {
                                fightAttackDelayCounter = Time.time + fightAttackDelay;
                                isAttacking = true;
                                enemyAnim.SetTrigger("Attack");
                                isReayAttack = false;
                            }
                        }
                        break;
                    case AIActionPhase.MOVEBACK:
                        if (isMoveBack)
                        {
                            isMoving = true;
                            enemyAnim.SetBool("MoveBack", true);
                        }
                        else
                        {
                            enemyAnim.SetBool("MoveBack", false);
                            aIActionPhase = AIActionPhase.MOVETOPLAYER;
                        }
                        break;
                }
            }
            else
            {
                StartCoroutine(ToNormal());
            }
        }
    }
    protected override void OnDie()
    {
        if (currentAnimationClip != "Enemy_die" && !deadTriggerSet)
        {
            enemyAnim.SetTrigger("Dead");
            deadTriggerSet = true;
            gameObject.layer = LayerMask.GetMask("Default");
            GameCore.combatController.canCombat = false;
            Destroy(this.gameObject, destroyAfterDeadDuration);
        }
    }
    protected override void OnParry()
    {
        enemyAnim.SetTrigger("Parry");
        isReayAttack = true;
    }
    protected override void OnTakeDamage()
    {
        enemyAnim.SetTrigger("TakeDamage");
        /*predictPosition = new Vector3(transform.position.x + 0.6f,transform.position.y,transform.position.z);
        isMoveBack = true;
        aIActionPhase = AIActionPhase.MOVEBACK;*/
    }
    protected override void OnStopFall()
    {
        controlable = true;
    }
    protected override void OnUpdate()
    {
        var clipInfo = enemyAnim.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = clipInfo[0].clip.name;

        if (GameCore.combatController.canCombat)
        {
            distanceBetweenPlayer = Mathf.Abs(transform.position.x - GameCore.combatController.targetPlayer.transform.position.x);
        }
        if(fightAttackDelayCounter <= Time.time&&!isReayAttack)
        {
            isReayAttack = true;
        }
    }
    private void PredictNextState()
    {
    }
    private IEnumerator ToCombat()
    {
        controlable = false;
        enemyAnim.SetBool("Combat", true);
        yield return waitForSworddraw;
        controlable = true;
        actionState = CharacterState.COMBAT;
        aIActionPhase = AIActionPhase.MOVETOPLAYER;
    }
    private IEnumerator ToNormal()
    {
        controlable = false;
        enemyAnim.SetBool("Combat", false);
        yield return waitForSworddraw;
        controlable = true;
        actionState = CharacterState.NORMAL;
        aIActionPhase = AIActionPhase.NONE;
    }
}
