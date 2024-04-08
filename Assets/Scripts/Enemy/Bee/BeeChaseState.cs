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
        //�ƶ����
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

        //��ʧĿ��󣬳���׷��״̬һ��ʱ�䣬Ȼ���л�ΪѲ��״̬
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        target = new Vector3(currentEnemy.attackerTransform.position.x, currentEnemy.attackerTransform.position.y + 1.5f, 0);
        //�л��泯����
        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (moveDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
        //����Ƶ�ʼ�ʱ
        attackRateCounter -= Time.deltaTime;
        //���˴����۷乥����Χ��
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            isAttacking = true;
            //���빥����Χ��ͣ��
            if (!currentEnemy.isHurt)
            {
                currentEnemy.rb.velocity = Vector2.zero;
            }
            //��������ʱ��Ϊ0ʱ����������
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
