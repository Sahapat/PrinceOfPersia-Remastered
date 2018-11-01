using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private Vector3 moveLeft;
    [SerializeField] private Vector3 moveRight;
    [SerializeField] private Vector3 moveUp;
    [SerializeField] private Vector3 moveDown;
    [Header("Max&Min Position")]
    [SerializeField] private float max_X;
    [SerializeField] private float min_X;
    [SerializeField] private float max_y;
    [SerializeField] private float min_y;
    private void Update()
    {
        if (targetPlayer)
        {
            var temp = Camera.main.WorldToViewportPoint(targetPlayer.transform.position);
            if (temp.y < 0)
            {
                transform.position += moveDown;
            }
            else if (temp.y > 1)
            {
                transform.position += moveUp;
            }
            else if (temp.x < 0)
            {
                transform.position += moveLeft;
            }
            else if (temp.x > 1)
            {
                transform.position += moveRight;
            }
            var currentX = transform.position.x;
            var currentY = transform.position.y;
            transform.position = new Vector3(Mathf.Clamp(currentX,min_X,max_X),Mathf.Clamp(currentY,min_y,max_y),-10f);
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            targetPlayer.transform.position = new Vector3(-2.44f,-7.5f,targetPlayer.transform.position.z);
        }
        else if(Input.GetKeyDown(KeyCode.F4))
        {
            var temp = targetPlayer.GetComponent<Prince>();
            targetPlayer.transform.position = new Vector3(62.16f,-4.34f,targetPlayer.transform.position.z);
            temp.isHaveSword = true;
        }
        else if(Input.GetKeyDown(KeyCode.F1))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        }
    }
}