using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorActive : MonoBehaviour
{
	[SerializeField]
	private FloorOpen openRef;
	[SerializeField]
	private GameObject mirrorObj;
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			if(openRef.isOpen)
			{
				mirrorObj.SetActive(true);
			}
		}
	}
}
