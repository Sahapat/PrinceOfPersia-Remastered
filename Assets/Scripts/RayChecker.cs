using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RayChecker : MonoBehaviour
{
    [Header("CombatChecker")]
    [SerializeField] private bool flipDirection;
    [SerializeField] private float EnemyCheckerRayDistance;
    [SerializeField] private LayerMask EnemyLayer;
    private CharacterSystem character;
    private Vector3 direction;
    void Awake()
    {
        character = GetComponentInParent<CharacterSystem>();
    }
    private void Update()
    {
        FacingCheck();
        Vector3 rayDirection = transform.TransformDirection(direction) * EnemyCheckerRayDistance;
        RaycastHit2D EnemyHit = Physics2D.Raycast(transform.position, rayDirection, EnemyCheckerRayDistance, EnemyLayer);
        if (EnemyHit)
        {

        }
        Debug.DrawRay(transform.position, direction, Color.green);
    }
    private void FacingCheck()
    {
        if(flipDirection)
        {
            direction = (character.currentFacing) ? Vector3.right : Vector3.left;
        }
        else
        {
            direction = (character.currentFacing) ? Vector3.left : Vector3.right;
        }
    }
}
