using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMoveFocus : MonoBehaviour
{
    [SerializeField] private bool isInputVertical;
    [SerializeField] private Transform upInFocus;
    [SerializeField] private Transform downInFocus;
    [SerializeField] private bool isInputHorizontal;
    [SerializeField] private Transform LeftInFocus;
    [SerializeField] private Transform RightInFocus;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isInputVertical)
            {
                switch ((int)other.contacts[0].normal.y)
                {
                    case -1:
                        GameCore.cameraController.SetMoveFocus(downInFocus.position);
                        break;
                    case 1:
                        GameCore.cameraController.SetMoveFocus(upInFocus.position);
                        break;
                }
            }
            else
            {
                switch ((int)other.contacts[0].normal.x)
                {
                    case -1:
                        GameCore.cameraController.SetMoveFocus(LeftInFocus.position);
                        break;
                    case 1:
                        GameCore.cameraController.SetMoveFocus(RightInFocus.position);
                        break;
                }
            }
            GetComponent<BoxCollider2D>().isTrigger = true;
            Invoke("BackToColider", 3f);
        }
    }
    private void BackToColider()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
