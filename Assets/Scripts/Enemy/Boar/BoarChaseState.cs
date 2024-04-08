using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        //播放动画
        currentEnemy.animator.SetBool("isRunning", true);
    }
    public override void PhysicFixedUpdate()
    {
     
    }
    public override void LogicUpdate()
    {
        //撞墙或悬崖不等待，直接转向
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
        }
        //丢失目标后，持续追逐状态一段时间，然后切换为巡逻状态
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
    }
    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isRunning", false);
    }
}
