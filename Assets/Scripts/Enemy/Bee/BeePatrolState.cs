using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeePatrolState : EnemyBaseState
{

    public Vector3 target;
    private Vector3 moveDir;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.walkSpeed;
        target = currentEnemy.GetNewPoint();
    }
    public override void PhysicFixedUpdate()
    {
        //移动相关
        if (!currentEnemy.isWait && !currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else
        {
            currentEnemy.rb.velocity = Vector3.zero;
        }
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        //判断蜜蜂是否到达目标地点
        //到达后等待一段时间，然后生成下一个目标地点
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) < 0.1f && Mathf.Abs(target.y - currentEnemy.transform.position.y) < 0.1f)
        {
            currentEnemy.isWait = true;
            target = currentEnemy.GetNewPoint();
        }
        //切换面朝方向
        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1,1,1);
        }
        if (moveDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public override void OnExit()
    {

    }
}
