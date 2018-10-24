using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour 
{
	float startPositionX;
	void Awake()
	{
		startPositionX = transform.localPosition.x;
	}
	void Start()
	{
		transform.localPosition = new Vector3(startPositionX,transform.localPosition.y,transform.localPosition.z);
	}
	void Update()
	{
		transform.localPosition = new Vector3(startPositionX,transform.localPosition.y,transform.localPosition.z);
	}
}
