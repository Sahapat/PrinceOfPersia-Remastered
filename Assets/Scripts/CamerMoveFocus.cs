using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMoveFocus : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform moveFocus;

    private BoxCollider2D moveFocusChecker;
    void Awake()
    {
        moveFocusChecker = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        var checkPosition = new Vector2(transform.position.x + moveFocusChecker.offset.x, transform.position.y + moveFocusChecker.offset.y);
        var hitCheck = Physics2D.OverlapBox(checkPosition, moveFocusChecker.size, 0f, LayerMask.GetMask("Player"));
        if (hitCheck)
        {
            GameCore.cameraController.SetMoveFocus(moveFocus.position);
        }
    }
}
