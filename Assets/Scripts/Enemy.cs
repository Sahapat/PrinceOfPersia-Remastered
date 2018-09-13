using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject hitChecker;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float stopDistance;
    [SerializeField] private float moveScale;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDelay;
    [SerializeField] private float attackDelay;
    [SerializeField] [Range(0, 100)] private float attackChance;

    private bool facing;
    private bool moving;
    private bool onFightState;

    private bool toMoveForward;

    private float predictPosX;
    private float countDelayMove;
    private float countDelayAttack;
    private bool isStopAndAttack;
    private bool isAttack;
    private bool setAttack;
    private string animationCurrentState;
    private Transform playerTarget;
    private SpriteRenderer spriteRenderer;
    private Animator enemyAnimator;
    private WaitForSeconds second;

    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = !facing;
        onFightState = true;
        enemyAnimator.SetTrigger("ToFight");
        toMoveForward = true;
        second = new WaitForSeconds(0.3f);
        setAttack = true;
    }

    private void Update()
    {
        var animationState = enemyAnimator.GetCurrentAnimatorClipInfo(0);
        animationCurrentState = animationState[0].clip.name;
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.left, 10, hitLayer);
        if (hit2D)
        {
            onFightState = true;
            playerTarget = hit2D.transform;
        }
    }
    public void TakeDamage()
    {
        enemyAnimator.SetTrigger("TakeDamage");
        toMoveForward = false;
        isStopAndAttack = false;
        MoveBackward();
    }
    private void FixedUpdate()
    {
        if (!onFightState) return;
        if (playerTarget == null) return;
        if (isAttack) return;

        if (isStopAndAttack && Time.time > countDelayAttack)
        {
            if (Random.value <= (attackChance * 0.01))
            {
                GoAttack();
            }

            countDelayAttack = Time.time + attackDelay;
        }
        float distanceBetweenTarget = Vector3.Distance(transform.position, playerTarget.position);
        if (distanceBetweenTarget > stopDistance && Time.time > countDelayMove && toMoveForward)
        {
            MoveForward();
        }
        else if (distanceBetweenTarget > stopDistance && Time.time > countDelayMove && !toMoveForward)
        {
            MoveBackward();
        }
        else if (distanceBetweenTarget <= stopDistance)
        {
            if (setAttack)
            {
                isStopAndAttack = true;
                setAttack = false;
            }
        }
        if (moving)
        {
            Move();
        }
    }
    private void GoAttack()
    {
        StartCoroutine(Attack());
    }
    private void MoveBackward()
    {
        if (facing)
        {
            predictPosX = transform.position.x - moveScale;
        }
        else
        {
            predictPosX = transform.position.x + moveScale;
        }
        enemyAnimator.SetTrigger("MoveLeft");
        countDelayMove = Time.time + moveDelay;
        toMoveForward = true;
        moving = true;
        isStopAndAttack = true;
        setAttack = true;
    }
    private void MoveForward()
    {
        if (facing)
        {
            predictPosX = transform.position.x + moveScale;
        }
        else
        {
            predictPosX = transform.position.x - moveScale;
        }
        enemyAnimator.SetTrigger("MoveRight");
        countDelayMove = Time.time + moveDelay;
        moving = true;
    }
    private void Move()
    {
        if (facing)
        {
            if (transform.position.x > predictPosX)
            {
                moving = false;
            }
        }
        else
        {
            if (transform.position.x < predictPosX)
            {
                moving = false;
            }
        }
        Vector3 toPos = new Vector3(predictPosX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, toPos, Time.deltaTime * moveSpeed);
    }
    private IEnumerator Attack()
    {
        isAttack = true;
        enemyAnimator.SetTrigger("Attack");
        yield return second;
        Collider2D hit2D = Physics2D.OverlapBox(hitChecker.transform.position
                            , GetComponent<BoxCollider2D>().size, 0, hitLayer);
        if (hit2D)
        {
			hit2D.gameObject.GetComponent<PrinceAnimationController>().TakeDamage();
			print("playerHit");
        }
        isAttack = false;
    }
}
