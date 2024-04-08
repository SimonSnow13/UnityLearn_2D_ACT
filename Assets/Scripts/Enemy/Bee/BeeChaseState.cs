using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeChaseState : EnemyBaseState
{
    private Attack attack;
    public Vector3 target;
    private Vector3 moveDir;
    public Attack attacker;

    private bool isAttacking;
    private float attackRateCounter = 0;

    public override void OnEnter(Enemy enemy)
    {
        attack = enemy.GetComponent<Attack>();

        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.lostTimeCounter = currentEnemy.lostTimeDuration;

        currentEnemy.animator.SetBool("isRunning",true);
    }
    public override void PhysicFixedUpdate()
    {
        //移动相关
        if ( !currentEnemy.isHurt && !currentEnemy.isDead && !isAttacking)
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

        //丢失目标后，持续追逐状态一段时间，然后切换为巡逻状态
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        target = new Vector3(currentEnemy.attackerTransform.position.x, currentEnemy.attackerTransform.position.y + 1.5f, 0);
        //切换面朝方向
        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (moveDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
        //攻击频率计时
        attackRateCounter -= Time.deltaTime;
        //敌人处于蜜蜂攻击范围内
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            isAttacking = true;
            //进入攻击范围后停下
            if (!currentEnemy.isHurt)
            {
                currentEnemy.rb.velocity = Vector2.zero;
            }
            //当攻击计时器为0时，进攻攻击
            if (attackRateCounter <= 0)
            {
                currentEnemy.animator.SetTrigger("attack");
                attackRateCounter = attack.attackRate;
            }
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isRunning", false);
    }
}
