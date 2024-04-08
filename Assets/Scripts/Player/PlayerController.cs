using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //声明变量，获取用户输入脚本组件
    public PlayerInputControl inputControl;

    [Header("材质")]
    public PhysicsMaterial2D normalMaterial;
    public PhysicsMaterial2D wallMaterial;

    [Header("基础参数")]
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

    //声明二位向量，用以存储输入的方向
    public Vector2 inputDirection;
    private Vector2 originalColliderOffset;
    private Vector2 originalColliderSize;
    
    [Header("状态")]
    public bool isDead;
    public bool isHurt;
    public bool isCrouch;
    public bool isAttack;   
    public bool isOnWallJumping;
    public bool isSliding;

    [Header("事件监听")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    //组件对象
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

        //将组件实例化为对象
        inputControl = new PlayerInputControl();
        inputControl.Gameplay.Jump.started += Jump;
        inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Slide.started += Slide;

        originalColliderOffset = capsuleCollider.offset; 
        originalColliderSize = capsuleCollider.size;

        #region 按住按键走路，松开时奔跑
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
        //调用方法：InputSystem组件对象.ActionMap.Action.值/方法
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();
    }
    
    /// <summary>
    /// 角色移动、翻转、下蹲
    /// </summary>
    void Move() 
    {
        //角色移动
        if (!isCrouch && !isOnWallJumping)
        {
            rigidbody.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rigidbody.velocity.y);
        }

        //角色翻转
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
        //判断下蹲状态
        isCrouch = inputDirection.y < -0.2f && physicsCheck.isGround;
        if (isCrouch )
        {
            //更改碰撞体大小和位置
            capsuleCollider.size = new Vector2(0.7f , 1.7f);
            capsuleCollider.offset = new Vector2(-0.05f , 0.85f);
        }
        else
        {
            //还原碰撞体参数
            capsuleCollider.size = originalColliderSize;
            capsuleCollider.offset = originalColliderOffset;
        }
    }
   
    //角色跳跃
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
            //跳跃中断滑行
            isSliding = false;
            StopAllCoroutines();
        }
    }
    //角色攻击
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        isAttack = true;
        playerAnimation.PlayerAttack();
    }
    //滑行
    private void Slide(InputAction.CallbackContext context)
    {
        if (!isSliding && physicsCheck.isGround && character.currentStamina >= slideStaminaCost)
        {
            isSliding = true;
            character.OnSlide(slideStaminaCost);
        }
        //目标地点
        var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
        //更改玩家角色图层，实现滑铲中无敌
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

    #region 角色状态相关方法
    /// <summary>
    /// 检测玩家角色状态
    /// </summary>
    void CheckState()
    {
        capsuleCollider.sharedMaterial = physicsCheck.isGround ?  normalMaterial : wallMaterial;
        //控制贴墙时下落速度
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
        //避免死亡状态下敌人攻击玩家角色
        if (isDead || isSliding)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");
    }


    //角色受伤后反弹
    public void GetHurt(Transform attackerTransform)
    {
        isHurt = true;
        rigidbody.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attackerTransform.position.x ,0).normalized;
        rigidbody.AddForce(dir * hurtForce , ForceMode2D.Impulse);
    }
    //角色死亡
    public void OnDeath()
    {
        isDead = true;
        inputControl.Gameplay.Disable();
    }
    #endregion

    #region 事件相关方法
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
