using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSystem : MonoBehaviour
{
    public enum CharacterState
    {
        NORMAL,
        COMBAT,
        DIE,
        FALL
    };
    public sbyte health
    {
        get
        {
            return characterHealth;
        }
        protected set
        {
            characterHealth = value;
            characterHealth = (sbyte)Mathf.Clamp(characterHealth, 0, MaxHealth);
            if (characterHealth <= 0)
            {
                isDead = true;
                actionState = CharacterState.DIE;
            }
        }
    }
    public bool currentFacing { get; protected set; } //true:right false:left
    protected CharacterState actionState;
    [Header("CharacterProperty")]
    [SerializeField] protected sbyte MaxHealth;
    [SerializeField] private float fallDistancePerFloor;
    [SerializeField] private bool isInvertFacing;

    [Header("FightPhaseProperty")]
    [SerializeField] private float fightStepScale;
    [SerializeField] private float fightStartAttackDuration;
    [SerializeField] private float fightAttackDuration;
    [SerializeField] private float fightParryDuration;
    [SerializeField] private float fightStepDuration;
    [SerializeField] private float fightStepMoveSpeed;
    [SerializeField] private float takeDamageDuration;
    [SerializeField] private BoxCollider2D attackColider;
    [SerializeField] protected BoxCollider2D floorColider;
    [SerializeField] private LayerMask attackLayer;

    //Counter Variable
    private float fightParryCounter;
    private float fightStartAttackCounter;
    private float fightAttackCounter;
    private float fightStepCounter;
    private float takeDamageCounter;
    private bool isHitSomething;
    protected bool attackTrigger;
    //Character property
    protected sbyte characterHealth;
    private bool isDead;
    private bool isFall;
    protected byte fallDamageTaken;
    protected float fallVelocity;
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool isParring = false;
    protected bool controlable = false;
    protected bool isCheckingFall = false;
    protected float distanceBetweenPoint;
    private bool isTakeDamage = false;
    //Other
    protected Vector3 predictPosition;
    protected float settingMoveSpeed;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D characterRigid;
    protected BoxCollider2D characterColider;
    public void FlipSprite()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    public virtual void SetControlable(bool status)
    {
        controlable = status;
    }

    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        OnUpdate();
    }
    private void FixedUpdate()
    {
        if (takeDamageCounter < Time.time && isTakeDamage)
        {
            controlable = true;
            isTakeDamage = false;
        }
        if (fightParryCounter < Time.time && isParring)
        {
            controlable = true;
            isParring = false;
        }
        UpdateFacing();
        switch (actionState)
        {
            case CharacterState.NORMAL:
                OnNormal();
                MoveToPosition();
                break;
            case CharacterState.COMBAT:
                OnCombat();
                MoveToPosition();
                break;
            case CharacterState.DIE:
                OnDie();
                break;
            case CharacterState.FALL:
                OnFall();
                break;
        }
        if (actionState != CharacterState.DIE)
        {
            FallingCheck();
        }
    }
    private void MoveToPosition()
    {
        if (isMoving && !isDead)
        {
            if (transform.position.x == predictPosition.x)
            {
                isMoving = false;
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, predictPosition, settingMoveSpeed * Time.deltaTime);
        }
    }
    public void TakeDamage(sbyte damage)
    {
        if (!isTakeDamage)
        {
            isTakeDamage = true;
            if (takeDamageCounter <= Time.time && !isDead)
            {
                takeDamageCounter = Time.time + takeDamageDuration;
                health = (sbyte)(health - damage);
                OnTakeDamage();
            }
        }
    }
    protected virtual void FightStep(sbyte direction)
    {
        if (fightStepCounter <= Time.time)
        {
            fightStepCounter = Time.time + fightStepDuration;
            settingMoveSpeed = fightStepMoveSpeed;
            float moveScale;
            if (currentFacing)
            {
                moveScale = (direction > 0) ? fightStepScale : fightStepScale * (-0.8f);
            }
            else
            {
                moveScale = (direction > 0) ? fightStepScale * (0.8f) : -fightStepScale;
            }
            predictPosition = new Vector3(transform.position.x + moveScale, transform.position.y, transform.position.z);
            isMoving = true;
        }
    }
    protected virtual void FightAttack()
    {
        controlable = false;
        if (attackTrigger)
        {
            fightStartAttackCounter = Time.time + fightStartAttackDuration;
            fightAttackCounter = fightStartAttackCounter + fightAttackDuration;
            attackTrigger = false;
        }

        if (!attackTrigger && !isHitSomething && fightStartAttackCounter <= Time.time)
        {
            var hit2D = Physics2D.OverlapBox(attackColider.transform.position, attackColider.size, 0f, attackLayer);
            if (hit2D)
            {
                var character = hit2D.GetComponent<CharacterSystem>();
                isHitSomething = true;
                if (character.isParring)
                {
                    OnTakeParry();
                }
                else
                {
                    character.TakeDamage(1);
                }
            }
        }
        if (fightAttackCounter <= Time.time && fightStartAttackCounter <= Time.time && !attackTrigger)
        {
            isAttacking = false;
            attackTrigger = true;
            controlable = true;
            isHitSomething = false;
        }
    }
    protected virtual void FightParry()
    {
        controlable = false;
        if (fightParryCounter <= Time.time)
        {
            isParring = true;
            fightParryCounter = Time.time + fightParryDuration;
            OnParry();
        }
    }
    protected virtual void OnTakeParry()
    {
        return;
    }
    protected virtual void OnParry()
    {
        return;
    }
    protected virtual void OnTakeDamage()
    {
        return;
    }
    protected virtual void OnCombat()
    {
        if (isAttacking)
        {
            FightAttack();
        }
    }
    protected virtual void OnNormal()
    {
        return;
    }
    protected virtual void OnDie()
    {
        return;
    }
    protected virtual void OnAwake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        characterColider = GetComponent<BoxCollider2D>();
        characterRigid = GetComponent<Rigidbody2D>();
    }
    protected virtual void OnStart()
    {
        attackColider.enabled = false;
        health = MaxHealth;
        actionState = CharacterState.NORMAL;
        controlable = true;
        isAttacking = false;
        attackTrigger = true;
        isCheckingFall = true;
    }
    protected virtual void OnUpdate()
    {
        return;
    }
    private void FallingCheck()
    {
        var overlapBoxPos = new Vector2(transform.position.x + floorColider.offset.x, transform.position.y + floorColider.offset.y);
        var groundCheck = Physics2D.OverlapBox(overlapBoxPos, floorColider.size, 0, LayerMask.GetMask("Floor"));

        if (!groundCheck)
        {
            if (!isFall)
            {
                OnStartFall();
                isFall = true;
                controlable = false;
                actionState = CharacterState.FALL;
            }
        }
        else
        {
            if (isFall)
            {
                isFall = false;
                actionState = CharacterState.NORMAL;
                OnStopFall();
            }
        }
    }
    protected virtual void OnFall()
    {
        return;
    }
    protected virtual void OnStartFall()
    {
        if (!isFall)
        {
            var nextGroundCheck = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Floor"));
            if (nextGroundCheck)
            {
                distanceBetweenPoint = Mathf.Abs(transform.position.y - nextGroundCheck.point.y);
                fallDamageTaken = (byte)(distanceBetweenPoint / fallDistancePerFloor);
            }
            predictPosition = transform.position;
        }
    }
    protected virtual void OnStopFall()
    {
        var damageTake = Mathf.Clamp(fallDamageTaken - 1, 0, 10);
        health -= (sbyte)damageTake;
    }
    private void UpdateFacing()
    {
        currentFacing = (isInvertFacing) ? spriteRenderer.flipX : !spriteRenderer.flipX;
    }
}
