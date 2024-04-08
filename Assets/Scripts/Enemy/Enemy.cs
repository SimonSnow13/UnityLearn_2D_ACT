using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    //���
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PhysicsCheck physicsCheck;
    
    [Header("��������")]
    public float walkSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public Vector3 faceDir;
    [HideInInspector] public Transform attackerTransform;
    public float hurtForce;
    [HideInInspector] public Vector3 spawnPoint;

    [Header("״̬")]
    [HideInInspector] public bool isHurt;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isWait;
    private EnemyBaseState currentState;
    protected EnemyBaseState patrolState;
    protected EnemyBaseState chaseState;
    protected EnemyBaseState skillState;

    [Header("��ʱ��")]
    public float waitTimeDuration;
    [HideInInspector] public float waitTimeCounter;
    public float lostTimeDuration;
    [HideInInspector] public float lostTimeCounter;

    [Header("������")]
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
        //��ţ��PreMove��Recover�׶Σ��������ƶ�
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SnailPreWalk") && !animator.GetCurrentAnimatorStateInfo(0).IsName("SnailRecover"))
        {
            rb.velocity = new Vector2(faceDir.x * currentSpeed * Time.deltaTime, rb.velocity.y);
        }
    }
    /// <summary>
    /// ��ʱ��
    /// </summary>
    void TimeCounter()
    {
        //�ȴ�״̬��ʱ
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
        //׷��״̬��ʧĿ���ʱ
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
    /// �۷��������ƶ�Ŀ���
    /// </summary>
    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }
    
    /// <summary>
    /// �ú������߼����ң�����ֵΪ����ֵ
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
    /// �л�״̬
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

    #region ״̬��ط���
    /// <summary>
    /// �������˵ķ������������򹥻��ߣ����˱�����
    /// </summary>
    public void OnHurt(Transform attackTrans)
    {
        //�ܵ��������򹥻���
        attackerTransform = attackTrans;
        if (attackerTransform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackerTransform.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
        //�������˶���
        isHurt = true;
        animator.SetTrigger("hurt");
        //���˻���
        Vector2 faceDir = new Vector2(transform.position.x - attackerTransform.position.x,0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(HurtWait(faceDir));
    }
    //Э�̣����˵ȴ�
    IEnumerator HurtWait(Vector2 faceDir)
    {
        rb.AddForce(faceDir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }
    /// <summary>
    /// ��������
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
