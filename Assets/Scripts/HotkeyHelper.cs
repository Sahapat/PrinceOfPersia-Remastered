using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HotkeyHelper : MonoBehaviour
{
	[SerializeField]
	private GameObject targetPlayer;
	[SerializeField]
	private Vector3 F3Cheater;
	[SerializeField]
	private Vector3 F4Cheater;
    void Update()
    {
		if(Input.GetKeyDown(KeyCode.F1))
        {
			SceneManager.LoadScene("Level1");
        }
		else if(Input.GetKeyDown(KeyCode.F2))
		{
			var princeScript = targetPlayer.GetComponent<Prince>();
			princeScript.Reset();
		}
        else if(Input.GetKeyDown(KeyCode.F3))
        {
			targetPlayer.transform.position = F3Cheater;
        }
        else if(Input.GetKeyDown(KeyCode.F4))
        {
            var temp = targetPlayer.GetComponent<Prince>();
            targetPlayer.transform.position = F4Cheater;
            temp.isHaveSword = true;
        }
    }
}
