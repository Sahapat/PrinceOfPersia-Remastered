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
    [SerializeField] private float moveBackSpeed;
    [Header("EnemyProperty")]
    [SerializeField] private float savePosition;
    [SerializeField] private float attackPosition;
    [SerializeField] private bool haveAttackPredict;
    [SerializeField, Range(0, 100)] private byte fightAttackChance;
    [SerializeField] private float fightAttackDelay;
    [SerializeField, Range(0, 100)] private byte fightParryChance;
    [SerializeField] private float fightParryDelay;
    [SerializeField] private float destroyAfterDeadDuration;
    [SerializeField] private float moveBackScale;
    [Header("Other")]
    [SerializeField] private Animator enemyAnim;
    [SerializeField] private GameObject particle;
    //Counter Variable
    private float fightParryDelayCounter;
    private float fightAttackDelayCounter;

    private string currentAnimationClip;
    private bool deadTriggerSet;
    private bool isMoveAndAttack;
    private bool isMoveBack;
    private AIActionPhase aIActionPhase;
    private WaitForSeconds waitForSworddraw;
    private ParticleSystem m_particleSystem;
    private EnemyRayChecker m_enemyRayChecker;
    private bool isReayAttack;
    private float distanceBetweenPlayer;
    public bool canCombat;

    protected override void OnAwake()
    {
        base.OnAwake();
        waitForSworddraw = new WaitForSeconds(0.1f);
        m_particleSystem = particle.GetComponent<ParticleSystem>();
        m_enemyRayChecker = GetComponent<EnemyRayChecker>();
        particle.SetActive(false);
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
            if (GameCore.combatController.currentEnemy)
            {
                if (GameCore.combatController.canCombat && GameCore.combatController.currentEnemy.name == this.name && m_enemyRayChecker.canCombat)
                {
                    StartCoroutine(ToCombat());
                }
            }
        }
    }
    public void ParticlePlay()
    {
        particle.SetActive(true);
        m_particleSystem.Play();
        Invoke("ParticleStop", 0.6f);
    }
    public void ParticleStop()
    {
        particle.SetActive(false);
    }
    protected override void OnCombat()
    {
        base.OnCombat();
        GameCore.combatController.isEnemyAttacking = isAttacking;
        GameCore.combatController.isEnemyParrying = isParring;
        if (controlable)
        {
            if (!m_enemyRayChecker.canCombat || GameCore.combatController.isPlayerDead)
            {
                StartCoroutine(ToNormal());
                return;
            }
            if (GameCore.combatController.canCombat || m_enemyRayChecker.canCombat)
            {
                switch (aIActionPhase)
                {
                    case AIActionPhase.MOVETOPLAYER:
                        if (!isMoveAndAttack)
                        {
                            if (distanceBetweenPlayer > savePosition)
                            {
                                isMoving = true;
                                var direction = (isInvertFacing) ? -1 : 1;
                                FightStep((sbyte)direction);
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
                                var direction = (isInvertFacing) ? -1 : 1;
                                FightStep((sbyte)direction);
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
                        if (distanceBetweenPlayer < attackPosition)
                        {
                            aIActionPhase = AIActionPhase.MOVETOPLAYER;
                        }
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
                            isMoveBack = false;
                        }
                        else if (!isMoving)
                        {
                            aIActionPhase = AIActionPhase.NONE;
                        }
                        break;
                    case AIActionPhase.NONE:
                        if (!GameCore.combatController.isPlayerAttacking)
                        {
                            Invoke("ToMovePhase", 1f);
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
    public override void OnTakeParry()
    {
        enemyAnim.SetTrigger("TakeParry");
        StartCoroutine(TakeParryDuration());
    }
    protected override void OnTakeDamage()
    {
        enemyAnim.SetTrigger("TakeDamage");
        float decreastPosition = 0;
        if (isInvertFacing)
        {
            decreastPosition = (currentFacing) ? -moveBackScale : moveBackScale;
        }
        else
        {
            decreastPosition = (currentFacing) ? moveBackScale : -moveBackScale;
        }
        predictPosition = new Vector3(transform.position.x + decreastPosition, transform.position.y, transform.position.z);
        settingMoveSpeed = moveBackSpeed;
        isMoveBack = true;
        aIActionPhase = AIActionPhase.MOVEBACK;
        enemyAnim.SetBool("MoveForward", false);
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
        if (fightAttackDelayCounter <= Time.time && !isReayAttack)
        {
            isReayAttack = true;
        }
        canCombat = m_enemyRayChecker.canCombat;
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
        enemyAnim.SetBool("MoveForward", false);
        yield return waitForSworddraw;
        controlable = true;
        actionState = CharacterState.NORMAL;
        aIActionPhase = AIActionPhase.NONE;
    }
    private IEnumerator TakeParryDuration()
    {
        controlable = false;
        yield return new WaitForSeconds(0.5f);
        controlable = true;
    }
    private void ToMovePhase()
    {
        aIActionPhase = AIActionPhase.MOVETOPLAYER;
    }
}
