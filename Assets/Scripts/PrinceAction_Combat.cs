using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PrinceAction_Combat : MonoBehaviour
{
    [SerializeField] private float moveScale;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject hitChecker;
    [SerializeField] private LayerMask hitLayer;
    private PrinceAnimationController princeAnimationController;
    private float predictPositionX;
    private bool moving;
    private bool side;

    private WaitForSeconds attackWaiting;
    private void Awake()
    {
        princeAnimationController = GetComponent<PrinceAnimationController>();
        attackWaiting = new WaitForSeconds(0.2f);
    }
    private void FixedUpdate()
    {
        if (princeAnimationController.GetCurrentActionState() != PrinceAnimationController.ActionState.COMBAT) return;

        if (princeAnimationController.GetCurrentAnimationClip() == "Fight_idle")
        {
            if (InputManager.getInputKeyDown_Right())
            {
                predictPositionX = transform.position.x + moveScale;
                princeAnimationController.CombatMoveRight();
                moving = true;
                side = true;
            }

            if (InputManager.getInputKeyDown_Left())
            {
                predictPositionX = transform.position.x - moveScale;
                princeAnimationController.CombatMoveLeft();
                moving = true;
                side = false;
            }
            if (InputManager.getInputKey_Interact())
            {
                StartCoroutine(Attack());
            }
        }
        if (moving)
        {
            Move();
        }
    }
    private void Move()
    {
        if (side)
        {
            if (transform.position.x > predictPositionX)
            {
                moving = false;
            }
        }
        else
        {
            if (transform.position.x < predictPositionX)
            {
                moving = false;
            }
        }
        Vector3 toPos = new Vector3(predictPositionX, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, toPos, moveSpeed * Time.deltaTime);
    }
    private IEnumerator Attack()
    {
        princeAnimationController.Attack();
        yield return attackWaiting;
        Collider2D hit2D =Physics2D.OverlapBox(hitChecker.transform.position
                            , GetComponent<BoxCollider2D>().size, 0, hitLayer);
        if (hit2D)
        {
            hit2D.gameObject.GetComponent<Enemy>().TakeDamage();
        }
    }
}