using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour 
{
	[SerializeField] Transform parent;
	[SerializeField] float xOffset;
	[SerializeField] float yOffset;
	void Update()
	{
		var hitFloorPoint = Physics2D.Raycast(parent.position,Vector2.down,Mathf.Infinity,LayerMask.GetMask("Floor"));
		if(hitFloorPoint)
		{
			transform.position = hitFloorPoint.point;
			transform.localPosition = new Vector3(xOffset,transform.localPosition.y+yOffset,transform.localPosition.z);
		}
	}
}
