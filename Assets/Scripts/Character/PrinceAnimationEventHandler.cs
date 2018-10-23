using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinceAnimationEventHandler : MonoBehaviour
{
	[Header("AnimationEvent property")]
	[SerializeField] private float crouchOutmoveScale;
	private GameObject objectParent;
	private Prince prince;
	private void Awake()
	{
		objectParent = transform.parent.gameObject;
		prince = GetComponentInParent<Prince>();
	}
	public void OnJump()
	{
		prince.canInteruptJump = false;
	}
	public void JumpEnd()
	{
		prince.EndJump();
	}
	public void ClimbUpEnd()
	{
		prince.ClimbUpEnd();
	}
	public void StartRunJump()
	{
		prince.StartRunJump();
	}
	public void StartIdleJump()
	{
		prince.StartIdleJump();
	}
	public void StartJump()
	{
		prince.StartJump();
	}
	public void StartRun()
	{
		prince.StartRunCycle();
	}
	public void RunTurnOut()
	{
		prince.RunTurnOut();
	}
	public void CrouchStepMove()
	{
		prince.CrouchStep();
	}
	public void IdleStepMove()
	{
		prince.IdleStep();
	}
	public void CrouchOutMove()
	{
		var direction = (prince.currentFacing)?1:-1;
		var moveX = objectParent.transform.position.x + (crouchOutmoveScale*direction);
		objectParent.transform.position = new Vector3(moveX,objectParent.transform.position.y,objectParent.transform.position.z);
	}
	public void Turning()
	{
		prince.FlipSprite();
	}
	public void SetControlable(string msg)
	{
		bool status = bool.Parse(msg);
		prince.SetControlable(status);
	}
	public void SetFloorCheck(string msg)
	{
		bool status = bool.Parse(msg);
		prince.SetFloorCheck(status);
	}
}
