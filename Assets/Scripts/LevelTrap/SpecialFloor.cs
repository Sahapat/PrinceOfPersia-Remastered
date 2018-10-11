using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFloor : MonoBehaviour
{
	private BoxCollider2D boxCollider2D;

	private void Awake()
	{
		boxCollider2D = GetComponent<BoxCollider2D>();
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			print(GetClimbDownPosition());
		}
	}
	public Vector3 GetClimbDownPosition()
	{
		return boxCollider2D.bounds.max;
	}
}
