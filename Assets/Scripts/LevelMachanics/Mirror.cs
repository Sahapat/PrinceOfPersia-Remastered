using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    private BoxCollider2D m_boxColider;
    [SerializeField] private BoxCollider2D m_checker;
    [SerializeField] private AudioClip mirrorSpawnSound;
    DoppleGanger m_doppleganger;
    private AudioSource m_audiosource;
    private bool triggerSound;
    void Awake()
    {
        m_boxColider = GetComponent<BoxCollider2D>();
        m_doppleganger = GetComponentInChildren<DoppleGanger>();
        m_audiosource = GetComponent<AudioSource>();
    }
    void Update()
    {
        var checkPosition = new Vector2(transform.position.x + m_checker.offset.x, transform.position.y + m_checker.offset.y);
        var hit = Physics2D.OverlapBox(checkPosition, m_checker.size, 0f, LayerMask.GetMask("Player"));
        if (hit)
        {
            var prince = hit.GetComponent<Prince>();
            if (prince.currentAnimationClip == "Run_Jump")
            {
                m_boxColider.enabled = false;
                Invoke("ShowDopple", 0.2f);
            }
        }
    }
    void ShowDopple()
    {
        m_doppleganger.MoveToDestination();
        if (!triggerSound)
        {
            m_audiosource.PlayOneShot(mirrorSpawnSound);
            triggerSound = true;
        }
    }
}
