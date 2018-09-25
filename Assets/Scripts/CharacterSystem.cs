using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSystem : MonoBehaviour
{
    public enum CharacterState
    {
        NORMAL,
        COMBAT,
        DIE
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
    public bool currentFacing { get; protected set; }
    protected CharacterState actionState;
    [Header("CharacterProperty")]
    [SerializeField] private sbyte MaxHealth;

    [Header("FightProperty")]
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
    protected bool isMoving = false;
    protected bool isAttacking = false;
    protected bool isParring = false;
    protected bool controlable = false;
    private bool isTakeDamage = false;
    //Other
    protected Vector3 predictPosition;
    private float settingMoveSpeed;

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
                break;
            case CharacterState.COMBAT:
                OnCombat();
                break;
            case CharacterState.DIE:
                OnDie();
                break;
        }
        MoveToPosition();
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
            var moveScale = (direction > 0) ? fightStepScale : fightStepScale * (-0.6f);
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
    protected virtual void OnAwake()
    {
        return;
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
    }
}
