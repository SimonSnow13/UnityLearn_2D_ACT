using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Enemy
{
    public float patrolRadius;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new BeePatrolState();
        chaseState = new BeeChaseState();
    }
    /// <summary>
    /// �۷����м����ҵķ���
    /// </summary>
    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position,checkDistance,attackerLayer);
        if (obj )
        {
            attackerTransform = obj.transform;
        }
        return obj;
    }
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position,checkDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint,patrolRadius);
    }
    /// <summary>
    /// �������ƶ�Ŀ���
    /// </summary>
    public override Vector3 GetNewPoint()
    {
        var targetX = Random.Range(-patrolRadius,patrolRadius);
        var targetY = Random.Range(-patrolRadius, patrolRadius);

        return spawnPoint + new Vector3(targetX, targetY);
    }
    /// <summary>
    /// �۷������ƶ��������ƶ�����ִ�и�״̬�д���
    /// </summary>
    public override void Move()
    {
    
    }
}
