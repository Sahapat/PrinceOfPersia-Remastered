using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController:MonoBehaviour
{
    private Vector3 moveFocus = new Vector3(0,0,-10f);

    private void Update()
    {
        transform.position = moveFocus;
    }
    public void SetMoveFocus(Vector3 position)
    {
        moveFocus = new Vector3(position.x,position.y,moveFocus.z);
    }
}