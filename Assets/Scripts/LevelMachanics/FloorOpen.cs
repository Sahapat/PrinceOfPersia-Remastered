using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorOpen : MonoBehaviour
{
	[Header("Reference")]
	[SerializeField] private Gate gateReference;
	[SerializeField] AudioClip button;
	private BoxCollider2D colliderChecker;
	private Animator floorAnim;
	private bool soundTrigger;
	private AudioSource mAudiosource;
	private void Awake()
	{
		floorAnim = GetComponentInChildren<Animator>();
		colliderChecker = GetComponent<BoxCollider2D>();
		mAudiosource = GetComponent<AudioSource>();
		soundTrigger = true;
	}
	private void Update()
	{
		var checkPosition = new Vector3(transform.position.x+colliderChecker.offset.x,transform.position.y+colliderChecker.offset.y);
		var tempHit = Physics2D.OverlapBox(checkPosition,colliderChecker.size,0f,LayerMask.GetMask("Player"));
		if(tempHit)
		{
			floorAnim.SetBool("Active",true);
			gateReference.Open();
			if(soundTrigger)
			{
				mAudiosource.PlayOneShot(button);
				soundTrigger = false;
			}
		}
		else
		{
			floorAnim.SetBool("Active",false);
			gateReference.ForClose();
			soundTrigger = true;
		}
	}
}
