using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : CharacterSystem
{
    [Header("EnemyProperty")]
    [SerializeField] private float destroyAfterDeadDuration;
    [Header("Other")]
    [SerializeField] private Animator enemyAnim;

    private string currentAnimationClip;
    private bool deadTriggerSet;
    protected override void OnDie()
    {
        if(currentAnimationClip != "Enemy_die" && !deadTriggerSet)
        {
            enemyAnim.SetTrigger("Dead");
            deadTriggerSet = true;
            gameObject.layer = LayerMask.GetMask("Default");
            GameCore.combatController.canCombat = false;
            Destroy(this.gameObject,destroyAfterDeadDuration);
        }
    }
    protected override void OnTakeDamage()
    {
        enemyAnim.SetTrigger("TakeDamage");
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        var clipInfo = enemyAnim.GetCurrentAnimatorClipInfo(0);
        currentAnimationClip = clipInfo[0].clip.name;
    }
}
