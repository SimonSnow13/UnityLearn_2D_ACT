using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        //���Ŷ���
        currentEnemy.animator.SetBool("isRunning", true);
    }
    public override void PhysicFixedUpdate()
    {
     
    }
    public override void LogicUpdate()
    {
        //ײǽ�����²��ȴ���ֱ��ת��
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1, 1);
        }
        //��ʧĿ��󣬳���׷��״̬һ��ʱ�䣬Ȼ���л�ΪѲ��״̬
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
