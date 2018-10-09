using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFloor : MonoBehaviour
{
	[SerializeField] private Animator sprikeAnim;
	[SerializeField] private Transform diePos;
	[SerializeField] private BoxCollider2D activeChecker;
	private bool isActive;
	void Update()
	{
		var activeCheckPos = new Vector2(transform.position.x+activeChecker.offset.x,transform.position.y+activeChecker.offset.y);

		var rayHit = Physics2D.OverlapBox(activeCheckPos,activeChecker.size,0,LayerMask.GetMask("Player"));
		if(rayHit)
		{
			sprikeAnim.SetBool("Active",true);
			isActive = true;
		}
		else
		{
			sprikeAnim.SetBool("Active",false);
			isActive = false;
		}
	}
	private void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			sprikeAnim.SetBool("Active",true);
			other.gameObject.GetComponent<Prince>().DieSprike();
			other.gameObject.transform.position = diePos.position;
		}
	}
}
