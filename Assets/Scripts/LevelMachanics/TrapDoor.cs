using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour 
{
    [SerializeField] private float activeDelay;
    private float activeDelayCount;

    private bool isActive;
    private Animator m_animator;
    private BoxCollider2D m_coliderChecker;
    void Awake()
    {
        m_coliderChecker = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(activeDelayCount <= Time.time)
        {
            activeDelayCount = Time.time + activeDelay;
            m_animator.SetTrigger("Active");
        }
        if(isActive)
        {
            var startPoint = new Vector2(transform.position.x+m_coliderChecker.offset.x,transform.position.y+m_coliderChecker.offset.y);
            var hit = Physics2D.OverlapBox(startPoint,m_coliderChecker.size,0f,LayerMask.GetMask("Player","Enemy"));
            if(hit)
            {
                CharacterSystem temp = hit.GetComponent<CharacterSystem>();
                if(temp)
                {
                    temp.SetDead();
                    isActive = false;
                }
            }
        }
    }
	public void ActiveTrap()
    {
        isActive = true;
    }
    public void DeActiveTrap()
    {
        isActive = false;
    }
}
