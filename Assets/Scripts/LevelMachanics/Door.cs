using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
	[Header("Reference")]
	[SerializeField] private string sceneToLoad;
	[SerializeField] private float delayBeforeLoad;
	[SerializeField] private Transform runInPosition;
	[SerializeField] private SpriteRenderer edgeDoorSprite;
	private BoxCollider2D coliderChecker;
	private Gate gateScript;
	private WaitForSeconds loadWait;
	private bool isIntoDoor;

	void Awake()
	{
		loadWait = new WaitForSeconds(delayBeforeLoad);
		coliderChecker = GetComponent<BoxCollider2D>();
		gateScript = GetComponent<Gate>();
		edgeDoorSprite.enabled =false;
	}
	void Update()
	{
		if(gateScript.isOpen)
		{
			var checkPosition = new Vector2(transform.position.x+coliderChecker.offset.x,transform.position.y+coliderChecker.offset.y);
			var playerChecker = Physics2D.OverlapBox(checkPosition,coliderChecker.size,0f,LayerMask.GetMask("Player"));

			if(playerChecker)
			{
				if(InputManager.GetKey_Up() && !isIntoDoor)
				{
					var princeScript = playerChecker.GetComponent<Prince>();
					isIntoDoor = true;
					edgeDoorSprite.enabled = true;
					princeScript.IntoDoor(runInPosition);
					StartCoroutine(IntoDoor(playerChecker.gameObject));
				}
			}
		}
	}
	private IEnumerator IntoDoor(GameObject prince)
	{
		yield return loadWait;
		print("into");
		Destroy(prince);
	}
}
