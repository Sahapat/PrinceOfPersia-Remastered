using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnStableFloor : MonoBehaviour
{
	[SerializeField]private Rigidbody2D floorRigid;
	[SerializeField]private Animator floorAnim;
	[SerializeField]private float beforeFallDuration;
	private bool isSet;
	private void Start()
	{
		floorRigid.isKinematic = true;
	}
	private void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			if(!isSet)
			{
				StartCoroutine(StartFall());
			}
		}
	}
	private IEnumerator StartFall()
	{
		isSet = true;
		floorAnim.SetBool("unStable",true);
		yield return new WaitForSeconds(beforeFallDuration);
		this.gameObject.layer = 14;
		floorAnim.SetBool("unStable",false);
		floorRigid.isKinematic = false;
	}
}
