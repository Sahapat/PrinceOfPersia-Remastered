using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RayChecker : MonoBehaviour
{
    [Header("CombatChecker")]
    [SerializeField] private bool flipDirection;
    [SerializeField] private float EnemyCheckerRayDistance;
    [SerializeField] private LayerMask EnemyLayer;
    private CharacterSystem character;
    private Vector3 direction;

    private void OnEnable()
    {
        character = GetComponentInParent<CharacterSystem>();
    }
    private void Update()
    {
        if (!GameCore.gameManager.isGameEnd)
        {
            FacingCheck();
            Vector3 rayDirection = transform.TransformDirection(direction) * EnemyCheckerRayDistance;
            RaycastHit2D EnemyHit = Physics2D.Raycast(transform.position, rayDirection, EnemyCheckerRayDistance, EnemyLayer);
            if (EnemyHit)
            {
                GameCore.combatController.canCombat = true;
            }
            else
            {
                GameCore.combatController.canCombat= false;
            }
            Debug.DrawRay(transform.position, direction*EnemyCheckerRayDistance, Color.green);
        }
    }
    private void FacingCheck()
    {
        if (flipDirection)
        {
            direction = (character.currentFacing) ? Vector3.right : Vector3.left;
        }
        else
        {
            direction = (character.currentFacing) ? Vector3.left : Vector3.right;
        }
    }
}
