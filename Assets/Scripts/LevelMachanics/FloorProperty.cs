using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorProperty : MonoBehaviour
{
	[Header("Property")]
	[SerializeField]private bool leftSideInteract;
	[SerializeField]private bool rightSideInteract;
	private BoxCollider2D floorColider;

	void Awake()
	{
		floorColider = GetComponent<BoxCollider2D>();
	}
	public Vector2 GetLeftSideEdge()
	{
		Vector2 temp = Vector2.zero;
		var xMin = Mathf.Min(floorColider.bounds.max.x,floorColider.bounds.min.x);
		var yPosition = Mathf.Max(floorColider.bounds.max.y,floorColider.bounds.min.y);
		temp = new Vector2(xMin,yPosition);
		return temp;
	}
	public Vector2 GetRightSideEdge()
	{
		Vector2 temp = Vector2.zero;
		var xMax = Mathf.Max(floorColider.bounds.max.x,floorColider.bounds.min.x);
		var yPosition = Mathf.Max(floorColider.bounds.max.y,floorColider.bounds.min.y);
		temp = new Vector2(xMax,yPosition);
		return temp;
	}
	public bool GetLeftSideInteract()
	{
		return leftSideInteract;
	}
	public bool GetRightSideInteract()
	{
		return rightSideInteract;
	}
}
