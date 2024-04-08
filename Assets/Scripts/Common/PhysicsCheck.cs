using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    //public CapsuleCollider2D bodyCollider;
    private PlayerController playerController;
    private Rigidbody2D rb;

    [Header("������")]
    public float checkRadius;
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    //�Ƿ��ֶ�������ײ�����λ��
    //public bool manual;

    [Header("״̬")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool isOnWall;
    public bool isPlayer;

    private void Awake()
    {
        //bodyCollider = GetComponent<CapsuleCollider2D>();

        //if (!manual)
        //{
        //    rightOffset = new Vector2(bodyCollider.bounds.size.x / 2 + bodyCollider.offset.x, bodyCollider.size.y / 2);
        //    leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        //}

        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Check();
    }

    void Check()
    {
        //����ɫ�Ƿ��ڵ���
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x , bottomOffset.y) , checkRadius , groundLayer);
        //����ɫ�Ƿ�ײǽ
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
        //�������Ƿ�λ��ǽ����
        if (isPlayer)
        {
            isOnWall = (touchLeftWall && playerController.inputDirection.x < 0f || touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y < 0;
        }
    }
    //�ڶ���ѡ��ʱ������ײ������Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}
