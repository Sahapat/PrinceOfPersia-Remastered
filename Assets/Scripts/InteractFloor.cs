using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractFloor : MonoBehaviour
{
	public bool canClimbUp;
	[SerializeField] private Transform climbUpPosition;
	public bool canClimbDown;
	public bool climbdownSide;
	public Transform climbDownPosition;
	public bool getCanClimbDown(Vector3 position,bool side)
	{
		if(climbdownSide == side)
		{
			if(Mathf.Abs(climbDownPosition.position.x-position.x)<=0.3f)
			{
				return true;
			}
		}
		return false;
	}
}
