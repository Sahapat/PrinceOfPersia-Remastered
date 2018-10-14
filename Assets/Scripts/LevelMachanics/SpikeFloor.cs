﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFloor : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private BoxCollider2D activeChecker;
    [SerializeField] private float activeDuration;
	[SerializeField] private SpriteRenderer frontSpike;
    [SerializeField] private Transform diePosition;

    public bool isActive;
    private bool spikeTrigger;
    private float activeCount;
    private BoxCollider2D spikeChecker;
	private Animator spikeAnim;
    void Awake()
    {
        spikeChecker = GetComponent<BoxCollider2D>();
		spikeAnim = GetComponent<Animator>();
    }
	void Start()
	{
		frontSpike.enabled = false;
	}
    void Update()
    {
        var activeSpikeChecker = new Vector2(activeChecker.gameObject.transform.position.x + activeChecker.offset.x, activeChecker.gameObject.transform.position.y + activeChecker.offset.y);
        var activeSpikeHit = Physics2D.OverlapBox(activeSpikeChecker, activeChecker.size, 0f, LayerMask.GetMask("Player"));
        if (activeSpikeHit)
        {
            if (!isActive)
            {
                activeCount = Time.time + activeDuration;
				spikeAnim.SetBool("Active",true);
                isActive = true;
            }
        }
		else
		{
			if(activeCount <= Time.time)
			{
				isActive = false;
				spikeAnim.SetBool("Active",false);
			}
		}
        if (isActive)
        {
            var spikeCheckPosition = new Vector2(transform.position.x + spikeChecker.offset.x, transform.position.y + spikeChecker.offset.y);
            var playerChecker = Physics2D.OverlapBox(spikeCheckPosition, spikeChecker.size, 0f, LayerMask.GetMask("Player"));
            if (playerChecker)
            {
                if (!spikeTrigger)
                {
					spikeTrigger =true;
                    var princeScript = playerChecker.GetComponent<Prince>();
					frontSpike.enabled =true;
                    princeScript.transform.parent = this.transform;
                    princeScript.transform.localPosition = diePosition.localPosition;
                    princeScript.DieSprike();
                }
            }
        }
    }
}