﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceAnimationEventHandler : MonoBehaviour
{
	[Header("AnimationEvent property")]
	[SerializeField] private float crouchOutmoveScale;
	private GameObject objectParent;
	private Prince prince;
	public Transform intoDoorPos;
	private void Awake()
	{
		objectParent = transform.parent.gameObject;
		prince = GetComponentInParent<Prince>();
	}
	public void CrouchOutMove()
	{
		var direction = (prince.currentFacing)?1:-1;
		var moveX = objectParent.transform.position.x + (crouchOutmoveScale*direction);
		objectParent.transform.position = new Vector3(moveX,objectParent.transform.position.y,objectParent.transform.position.z);
	}
	public void TurnInRunning()
	{
		prince.FlipSprite();
	}
	public void SetControlable(string msg)
	{
		bool status = bool.Parse(msg);
		prince.SetControlable(status);
	}
	public void SetPositionToIntoDoorPos()
	{
		transform.parent.transform.position = intoDoorPos.position;
	}
}