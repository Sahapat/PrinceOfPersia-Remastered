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
    [SerializeField] AudioClip breakSound;
    [SerializeField] AudioClip beforeFall;
    private bool isTouchFloor;
    private bool OnFall;
    private float onFallSoundCount;
    private WaitForSeconds waitForFalling;
    private Animator fallingFloorAnim;
    private Rigidbody2D floorRigidbody;
    private BoxCollider2D colliderChecker;
    private AudioSource floorAudioSource;
    private SpriteRenderer spriteRenderer;
    private float breakCount;
    private GameObject player;
    private Vector3 playerLastPos;
    void Awake()
    {
        waitForFalling = new WaitForSeconds(fallingWaitDuration);
        fallingFloorAnim = GetComponentInChildren<Animator>();
        floorRigidbody = GetComponent<Rigidbody2D>();
        colliderChecker = GetComponent<BoxCollider2D>();
        floorAudioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    void Start()
    {
        floorRigidbody.isKinematic = true;
    }
    void Update()
    {
        var checkPosition = new Vector2(transform.position.x + colliderChecker.offset.x, transform.position.y + colliderChecker.offset.y);
        var floorCheckPosition = new Vector2(floorColliderChecker.gameObject.transform.position.x + floorColliderChecker.offset.x, floorColliderChecker.gameObject.transform.position.y + floorColliderChecker.offset.y);
        var playerChecker = Physics2D.OverlapBox(checkPosition, colliderChecker.size, 0f, LayerMask.GetMask("Player"));
        var floorChecker = Physics2D.OverlapBox(floorCheckPosition, floorColliderChecker.size, 0f, LayerMask.GetMask("Floor"));
        var breakCheckerLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1), Vector2.left, Mathf.Infinity, LayerMask.GetMask("Player"));
        var breakCheckerRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1), Vector2.right, Mathf.Infinity, LayerMask.GetMask("Player"));

        if (breakCheckerLeft)
        {
            if (!player)
            {
                player = breakCheckerLeft.collider.gameObject;
                if (breakCount <= Time.time)
                {
                    fallingFloorAnim.SetTrigger("Active");
                    breakCount = Time.time + 2f;
                    playerLastPos = player.transform.position;
                }
            }
            else
            {
                if (playerLastPos != player.transform.position)
                {
                    if (breakCount <= Time.time)
                    {
                        fallingFloorAnim.SetTrigger("Active");
                        breakCount = Time.time + 2f;
                        playerLastPos = player.transform.position;
                    }
                }
            }
        }
        else if(breakCheckerRight)
        {
            if (!player)
            {
                player = breakCheckerRight.collider.gameObject;
                if (breakCount <= Time.time)
                {
                    fallingFloorAnim.SetTrigger("Active");
                    breakCount = Time.time + 2f;
                    playerLastPos = player.transform.position;
                }
            }
            else
            {
                if (playerLastPos != player.transform.position)
                {
                    if (breakCount <= Time.time)
                    {
                        fallingFloorAnim.SetTrigger("Active");
                        breakCount = Time.time + 2f;
                        playerLastPos = player.transform.position;
                    }
                }
            }
        }
        if (playerChecker)
        {
            if (!isFalling)
            {
                StartCoroutine(Falling());
            }
            if (!OnFall && onFallSoundCount <= Time.time)
            {
                onFallSoundCount = Time.time + 1.5f;
                floorAudioSource.PlayOneShot(beforeFall);
            }
        }
        if (floorChecker)
        {
            if (!floorRigidbody.isKinematic && isFalling && spriteRenderer.enabled)
            {
                var temp = Instantiate(rubbleObj, transform.position, Quaternion.identity);
                temp.transform.position = new Vector3(temp.transform.position.x + 0.1f, floorChecker.GetComponent<BoxCollider2D>().bounds.max.y - 0.05f);
                floorAudioSource.PlayOneShot(breakSound);
                spriteRenderer.enabled = false;
                colliderChecker.enabled = false;
                Destroy(this.gameObject, 5f);
            }
        }
    }
    private IEnumerator Falling()
    {
        isFalling = true;
        yield return waitForFalling;
        this.gameObject.layer = 0;
        OnFall = true;
        fallingFloorAnim.SetTrigger("Fall");
        floorRigidbody.isKinematic = false;
        colliderChecker.isTrigger = true;
    }
}
