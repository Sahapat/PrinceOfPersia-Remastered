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
        private set
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
    [SerializeField] private sbyte MaxHealth;
    [SerializeField] private bool isInvertFacing;
    [SerializeField] private float fallTakeDamageDistance;
    [SerializeField] private float fallDieDistance;

    [Header("FightPhaseProperty")]
    [SerializeField] private float fightStepScale;
    [SerializeField] private float fightStartAttackDuration;
    [SerializeField] private float fightAttackDuration;
    [SerializeField] private float fightParryDuration;
    [SerializeField] private float fightStepDuration;
    [SerializeField] private float fightStepMoveSpeed;
    [SerializeField] private float takeDamageDuration;
    [SerializeField] private BoxCollider2D attackColider;
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
    private sbyte characterHealth;
    private bool isDead;
    private bool isFall;
    private bool isFallTakeDamage;
    private bool isFallDie;
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool isParring = false;
    protected bool controlable = false;
    private bool isTakeDamage = false;
    //Other
    protected Vector3 predictPosition;
    protected float settingMoveSpeed;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D characterRigid;
    private BoxCollider2D characterColider;

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
    }
    private void MoveToPosition()
    {
        if (isMoving && !isDead)
        {
            if (transform.position == predictPosition)
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
        return;
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
        if (isParring)
        {
            FightParry();
        }
        else if (isAttacking)
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
    protected virtual void OnFall()
    {
    }
    protected virtual void OnStartFall()
    {

    }
    protected virtual void OnStopFall()
    {

    }
    protected virtual void FallTakeDamage()
    {

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
    }
    protected virtual void OnUpdate()
    {
        if (takeDamageCounter < Time.time && isTakeDamage)
        {
            controlable = true;
            isTakeDamage = false;
        }
        UpdateFacing();
        if (!characterRigid.isKinematic)
        {
            FallingCheck();
        }
    }
    private void FallingCheck()
    {
        var overlapBoxPos = new Vector2(transform.position.x + characterColider.offset.x, transform.position.y + characterColider.offset.y);
        var groundCheck = Physics2D.OverlapBox(overlapBoxPos, characterColider.size, 0, LayerMask.GetMask("Floor"));

        if (!groundCheck)
        {
            if (!isFall)
            {
                OnStartFall();
                isFall = true;
                actionState = CharacterState.FALL;
            }
        }
        else
        {
            if (isFall)
            {
                isFall = false;
                OnStopFall();
            }
        }
    }
    private void UpdateFacing()
    {
        currentFacing = (isInvertFacing) ? spriteRenderer.flipX : !spriteRenderer.flipX;
    }
}
