using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //������������ȡ�û�����ű����
    public PlayerInputControl inputControl;

    [Header("����")]
    public PhysicsMaterial2D normalMaterial;
    public PhysicsMaterial2D wallMaterial;

    [Header("��������")]
    public float speed;
    private float walkSpeed => speed / 2.5f;
    private float runSpeed;
    public float jumpForce;
    public float hurtForce;
    public float wallJumpForce;
    public float wallJumpHeight;
    public float slideDistance;
    public float slideSpeed;
    public int slideStaminaCost;

    //������λ���������Դ洢����ķ���
    public Vector2 inputDirection;
    private Vector2 originalColliderOffset;
    private Vector2 originalColliderSize;
    
    [Header("״̬")]
    public bool isDead;
    public bool isHurt;
    public bool isCrouch;
    public bool isAttack;   
    public bool isOnWallJumping;
    public bool isSliding;

    [Header("�¼�����")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    //�������
    private SpriteRenderer spriteRender;
    private new Rigidbody2D rigidbody;
    private PhysicsCheck physicsCheck;
    private CapsuleCollider2D capsuleCollider;
    private PlayerAnimation playerAnimation;
    private Character character;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        physicsCheck = GetComponent<PhysicsCheck>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();


        runSpeed = speed;

        //�����ʵ����Ϊ����
        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Slide.started += Slide;

        originalColliderOffset = capsuleCollider.offset; 
        originalColliderSize = capsuleCollider.size;

        #region ��ס������·���ɿ�ʱ����
        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            }
        };
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        #endregion

        inputControl.Enable();
    }



    private void OnEnable()
    {
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }


    private void FixedUpdate()
    {
        if (!isHurt && !isAttack)
        {
            Move();
        }
    }

    private void Update()
    {
        //���÷�����InputSystem�������.ActionMap.Action.ֵ/����
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();
    }
    
    /// <summary>
    /// ��ɫ�ƶ�����ת���¶�
    /// </summary>
    void Move() 
    {
        //��ɫ�ƶ�
        if (!isCrouch && !isOnWallJumping)
        {
            rigidbody.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rigidbody.velocity.y);
        }

        //��ɫ��ת
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        else if (inputDirection.x < 0)
        {
            faceDir = -1;
        }
        transform.localScale = new Vector3(faceDir, 1, 1);
        //�ж��¶�״̬
        isCrouch = inputDirection.y < -0.2f && physicsCheck.isGround;
        if (isCrouch )
        {
            //������ײ���С��λ��
            capsuleCollider.size = new Vector2(0.7f , 1.7f);
            capsuleCollider.offset = new Vector2(-0.05f , 0.85f);
        }
        else
        {
            //��ԭ��ײ�����
            capsuleCollider.size = originalColliderSize;
            capsuleCollider.offset = originalColliderOffset;
        }
    }
   
    //��ɫ��Ծ
    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (physicsCheck.isOnWall)
        {
            rigidbody.AddForce(new Vector2(-inputDirection.x,wallJumpHeight) * wallJumpForce ,ForceMode2D.Impulse);
            isOnWallJumping = true;
            //��Ծ�жϻ���
            isSliding = false;
            StopAllCoroutines();
        }
    }
    //��ɫ����
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        isAttack = true;
        playerAnimation.PlayerAttack();
    }
    //����
    private void Slide(InputAction.CallbackContext context)
    {
        if (!isSliding && physicsCheck.isGround && character.currentStamina >= slideStaminaCost)
        {
            isSliding = true;
            character.OnSlide(slideStaminaCost);
        }
        //Ŀ��ص�
        var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
        //������ҽ�ɫͼ�㣬ʵ�ֻ������޵�
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        StartCoroutine(TriggerSlide(targetPos));
    }
    private IEnumerator TriggerSlide(Vector3 targetPos)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)
                break;
            if (physicsCheck.touchLeftWall && transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x >0f)
            {
                isSliding = false;
                break;
            }
            rigidbody.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (Mathf.Abs(targetPos.x - transform.position.x) > 0.1f);
        isSliding = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #region ��ɫ״̬��ط���
    /// <summary>
    /// �����ҽ�ɫ״̬
    /// </summary>
    void CheckState()
    {
        capsuleCollider.sharedMaterial = physicsCheck.isGround ?  normalMaterial : wallMaterial;
        //������ǽʱ�����ٶ�
        if (physicsCheck.isOnWall)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x,rigidbody.velocity.y/2.0f);
        }
        else
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
        }
        if (isOnWallJumping && rigidbody.velocity.y < 0f)
        {
            isOnWallJumping = false;
        }
        //��������״̬�µ��˹�����ҽ�ɫ
        if (isDead || isSliding)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }


    //��ɫ���˺󷴵�
    public void GetHurt(Transform attackerTransform)
    {
        isHurt = true;
        rigidbody.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attackerTransform.position.x ,0).normalized;
        rigidbody.AddForce(dir * hurtForce , ForceMode2D.Impulse);
    }
    //��ɫ����
    public void OnDeath()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
    #endregion

    #region �¼���ط���
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.Gameplay.Disable();
    }

    private void OnLoadDataEvent()
    {
        isDead = false;
    }


    private void OnAfterSceneLoadedEvent()
    {
        inputControl.Gameplay.Enable();
    }
    #endregion
}
