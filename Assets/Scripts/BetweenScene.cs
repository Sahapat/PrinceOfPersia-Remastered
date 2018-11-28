using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetweenScene : MonoBehaviour
{
	[SerializeField]
	private string sceneToLoad;

	void Update()
	{
		if(InputManager.GetKeyDown_Interact())
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
		}
	}
}
