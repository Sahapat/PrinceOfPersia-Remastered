using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BetweenScene : MonoBehaviour
{
	[SerializeField]
	private string sceneToLoad;
	[SerializeField]
	private GameObject loadingSprite;

	void Awake()
	{
		loadingSprite.SetActive(false);
	}

	void Update()
	{
		if(InputManager.GetKeyDown_Interact())
		{
			StartCoroutine(LoadNewScene());
		}
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
