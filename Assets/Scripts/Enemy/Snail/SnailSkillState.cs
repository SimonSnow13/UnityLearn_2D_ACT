using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailSkillState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = 0f;

        currentEnemy.animator.SetTrigger("skill");
        currentEnemy.animator.SetBool("isHiding",true);

        currentEnemy.lostTimeCounter = currentEnemy.lostTimeDuration;

        currentEnemy.GetComponent<Character>().invulnerable = true;
        currentEnemy.GetComponent<Character>().invulnerableCounter = currentEnemy.lostTimeCounter;
    }
    public override void PhysicFixedUpdate()
    {

    }
    public override void LogicUpdate()
    {
        //����ʧĿ��һ��ʱ����л���Ѳ��״̬
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        currentEnemy.GetComponent<Character>().invulnerableCounter = currentEnemy.lostTimeCounter;
    }
    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isHiding", false);
        currentEnemy.GetComponent<Character>().invulnerable = false;
    }
}
