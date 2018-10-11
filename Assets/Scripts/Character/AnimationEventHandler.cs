using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
	[Header("AnimationEvent property")]
	[SerializeField] private float crouchOutmoveScale;
	private GameObject objectParent;
	private CharacterSystem character;
	private void Awake()
	{
		objectParent = transform.parent.gameObject;
		character = GetComponentInParent<CharacterSystem>();
	}
	public void CrouchOutMove()
	{
		var direction = (character.currentFacing)?1:-1;
		var moveX = objectParent.transform.position.x + (crouchOutmoveScale*direction);
		objectParent.transform.position = new Vector3(moveX,objectParent.transform.position.y,objectParent.transform.position.z);
	}
	public void TurnInRunning()
	{
		character.FlipSprite();
	}
}
