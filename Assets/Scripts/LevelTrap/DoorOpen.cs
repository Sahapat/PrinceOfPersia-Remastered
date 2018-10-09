using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
	[SerializeField] private GameObject doorObj;
	[SerializeField] private Animator doorAnim;

	private void Awake()
	{
		doorObj.SetActive(true);
	}
	private void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			doorObj.SetActive(false);
			doorAnim.SetBool("Active",true);
		}
	}
	private void OnCollisionExit2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			doorObj.SetActive(true);
			doorAnim.SetBool("Active",false);
		}
	}
}
