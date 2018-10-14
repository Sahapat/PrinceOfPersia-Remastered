using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFloor : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject rubbleObj;
	[SerializeField] private BoxCollider2D floorColliderChecker;
    [SerializeField] private float fallingWaitDuration;
    [SerializeField] private bool isFalling;
    private bool isTouchFloor;
    private WaitForSeconds waitForFalling;
    private Animator fallingFloorAnim;
    private Rigidbody2D floorRigidbody;
    private BoxCollider2D colliderChecker;

    void Awake()
    {
        waitForFalling = new WaitForSeconds(fallingWaitDuration);
        fallingFloorAnim = GetComponentInChildren<Animator>();
        floorRigidbody = GetComponent<Rigidbody2D>();
        colliderChecker = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        floorRigidbody.isKinematic = true;
    }
    void Update()
    {
        var checkPosition = new Vector2(transform.position.x + colliderChecker.offset.x, transform.position.y+colliderChecker.offset.y);
		var floorCheckPosition = new Vector2(floorColliderChecker.gameObject.transform.position.x+floorColliderChecker.offset.x,floorColliderChecker.gameObject.transform.position.y+floorColliderChecker.offset.y);
        var playerChecker = Physics2D.OverlapBox(checkPosition, colliderChecker.size, 0f, LayerMask.GetMask("Player"));
        var floorChecker = Physics2D.OverlapBox(floorCheckPosition, floorColliderChecker.size, 0f, LayerMask.GetMask("Floor"));
        if (playerChecker)
        {
            if (!isFalling)
            {
                StartCoroutine(Falling());
            }
        }
        if (floorChecker)
        {
            if (!floorRigidbody.isKinematic&&isFalling)
            {
                var temp = Instantiate(rubbleObj, transform.position, Quaternion.identity);
                temp.transform.position = new Vector3(temp.transform.position.x+0.1f, floorChecker.GetComponent<BoxCollider2D>().bounds.max.y-0.05f);
                Destroy(this.gameObject);
            }
        }
    }
    private IEnumerator Falling()
    {
        isFalling = true;
		yield return waitForFalling;
		this.gameObject.layer = 0;
		fallingFloorAnim.SetTrigger("Fall");
		floorRigidbody.isKinematic = false;
    }
}
