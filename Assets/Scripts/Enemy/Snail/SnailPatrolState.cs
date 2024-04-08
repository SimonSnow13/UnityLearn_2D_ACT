using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.walkSpeed;
    }
    public override void PhysicFixedUpdate()
    {

    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Skill);
        }
        //撞墙或面前为悬崖时，等待后转向
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.isWait = true;
            currentEnemy.animator.SetBool("isWalking", false);
        }
        else
        {
            currentEnemy.animator.SetBool("isWalking", true);
        }
    }
    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isWalking", false);
    }
}
