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
	[SerializeField] private GameObject loadingSprite;
	private BoxCollider2D coliderChecker;
	private Gate gateScript;
	private WaitForSeconds loadWait;
	private bool isIntoDoor;
	bool isIn;
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
		loadingSprite.SetActive(false);
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
					isIn = true;
					StartCoroutine(IntoDoor());
				}
			}
		}
		if(isIn&&InputManager.GetKeyDown_Interact())
		{
			StartCoroutine(LoadNewScene());
		}
	}
	private IEnumerator IntoDoor()
	{
		yield return new WaitForSeconds(0.6f);
		GameCore.gameManager.SuccessSoundPlay();
	}
	private IEnumerator LoadNewScene()
    {
        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
			loadingSprite.SetActive(true);
            yield return null;
        }

    }
}
