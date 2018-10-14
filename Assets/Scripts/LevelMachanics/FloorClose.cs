using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorClose : MonoBehaviour
{
    [Header("Reference")]
	[SerializeField] private Gate gateReference;

	private BoxCollider2D colliderChecker;
	private Animator floorAnim;

	private void Awake()
	{
		floorAnim = GetComponentInChildren<Animator>();
		colliderChecker = GetComponent<BoxCollider2D>();
	}
	private void Update()
	{
		var checkPosition = new Vector3(transform.position.x+colliderChecker.offset.x,transform.position.y+colliderChecker.offset.y);
		var tempHit = Physics2D.OverlapBox(checkPosition,colliderChecker.size,0f,LayerMask.GetMask("Player"));
		if(tempHit)
		{
			floorAnim.SetBool("Active",true);
			gateReference.Close();
		}
		else
		{
			floorAnim.SetBool("Active",false);
		}
	}
}
