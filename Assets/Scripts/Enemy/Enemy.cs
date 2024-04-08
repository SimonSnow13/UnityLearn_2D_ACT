using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    //组件
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PhysicsCheck physicsCheck;
    
    [Header("基本参数")]
    public float walkSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public Vector3 faceDir;
    [HideInInspector] public Transform attackerTransform;
    public float hurtForce;
    [HideInInspector] public Vector3 spawnPoint;

    [Header("状态")]
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isWait;
    private EnemyBaseState currentState;
    protected EnemyBaseState patrolState;
    protected EnemyBaseState chaseState;
    protected EnemyBaseState skillState;

    [Header("计时器")]
    public float waitTimeDuration;
    [HideInInspector] public float waitTimeCounter;
    public float lostTimeDuration;
    [HideInInspector] public float lostTimeCounter;

    [Header("检测参数")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackerLayer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = walkSpeed;
        //waitTimeCounter = waitTimeDuration;
        lostTimeCounter = lostTimeDuration;
        spawnPoint = transform.position;
    }
    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void FixedUpdate()
    {
        currentState.PhysicFixedUpdate();
        if(!isHurt && !isDead && !isWait)
        {
            Move();
        }
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        currentState.LogicUpdate();
        TimeCounter();
    }

    private void OnDisable()
    {
        currentState.OnExit();    
    }

    public virtual void Move()
    {
        //蜗牛在PreMove和Recover阶段，不进行移动
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SnailPreWalk") && !animator.GetCurrentAnimatorStateInfo(0).IsName("SnailRecover"))
        {
            rb.velocity = new Vector2(faceDir.x * currentSpeed * Time.deltaTime, rb.velocity.y);
        }
    }
    /// <summary>
    /// 计时器
    /// </summary>
    void TimeCounter()
    {
        //等待状态计时
        if (isWait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                isWait = false;
                waitTimeCounter = waitTimeDuration;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }
        //追逐状态丢失目标计时
        if (!FoundPlayer() && lostTimeCounter > 0) 
        { 
            lostTimeCounter -= Time.deltaTime;
        }
        else if (FoundPlayer())
        {
            lostTimeCounter = lostTimeDuration;
        }
    }
    /// <summary>
    /// 蜜蜂随机获得移动目标点
    /// </summary>
    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }
    
    /// <summary>
    /// 用盒体射线检测玩家，返回值为布尔值
    /// </summary>
    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackerLayer);
    }
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x,0,0), 0.2f);
    }
    /// <summary>
    /// 切换状态
    /// </summary>
    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        } ;
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    #region 状态相关方法
    /// <summary>
    /// 敌人受伤的方法，包括朝向攻击者，受伤被击退
    /// </summary>
    public void OnHurt(Transform attackTrans)
    {
        //受到攻击朝向攻击者
        attackerTransform = attackTrans;
        if (attackerTransform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackerTransform.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        //播放受伤动画
        isHurt = true;
        animator.SetTrigger("hurt");
        //受伤击退
        Vector2 faceDir = new Vector2(transform.position.x - attackerTransform.position.x,0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(HurtWait(faceDir));
    }
    //协程：受伤等待
    IEnumerator HurtWait(Vector2 faceDir)
    {
        rb.AddForce(faceDir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }
    /// <summary>
    /// 敌人死亡
    /// </summary>
    public void OnDeath()
    {
        gameObject.layer = 2;
        isDead = true;
        animator.SetBool("isDead", true);
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
