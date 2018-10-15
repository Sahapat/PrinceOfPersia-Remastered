using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	[SerializeField] private Gate[] gateDoor;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Player"))
		{
			for(int i =0;i<gateDoor.Length;i++)
			{
				if(!gateDoor[i].isOpen)
				{
					gateDoor[i].Open();
				}
			}
		}
	}
}
