using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour 
{
    [SerializeField] private float activeDelay;
    [SerializeField] private AudioClip activeSound;
    [SerializeField] private AudioClip killSound;
    private float activeDelayCount;

    private bool isActive;
    private Animator m_animator;
    private BoxCollider2D m_coliderChecker;
    private AudioSource m_audiosource;
    void Awake()
    {
        m_coliderChecker = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();
        m_audiosource = GetComponent<AudioSource>();
    }
    void Update()
    {
        var distance = Vector3.Distance(transform.position,Camera.main.transform.position);
        if(distance < 12.5f)
        {
            m_audiosource.volume = 1;
        }
        else 
        {
            m_audiosource.volume = 0;
        }
        if(activeDelayCount <= Time.time)
        {
            activeDelayCount = Time.time + activeDelay;
            m_animator.SetTrigger("Active");
            m_audiosource.PlayOneShot(activeSound);
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
                    temp.SetDead(transform.position);
                    isActive = false;
                    m_audiosource.PlayOneShot(killSound);
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
