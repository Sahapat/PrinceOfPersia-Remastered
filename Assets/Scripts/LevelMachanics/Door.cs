using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
	[Header("Reference")]
	[SerializeField] public string sceneToLoad;
	[SerializeField] private float delayBeforeLoad;
	[SerializeField] private GameObject princeIntoDoor;
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
	void Start()
	{
		princeIntoDoor.SetActive(false);
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
					isIntoDoor = true;
					Destroy(playerChecker.gameObject);
					princeIntoDoor.SetActive(true);
					var animatorPrinceIntoDoor = princeIntoDoor.GetComponent<Animator>();
					animatorPrinceIntoDoor.SetTrigger("Play");
					edgeDoorSprite.enabled = true;
					StartCoroutine(IntoDoor());
				}
			}
		}
	}
	private IEnumerator IntoDoor()
	{
		yield return new WaitForSeconds(0.6f);
		GameCore.gameManager.SuccessSoundPlay();
		yield return loadWait;
		if(sceneToLoad != string.Empty)
		{
			SceneManager.LoadScene(sceneToLoad);
		}
	}
}
