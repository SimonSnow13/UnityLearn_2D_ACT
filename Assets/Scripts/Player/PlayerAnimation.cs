using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private new Rigidbody2D rigidbody2D;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimation();
    }
    //¸ü¸Ä¶¯»­×´Ì¬
    public void SetAnimation()
    {
        animator.SetFloat("velocityX",Mathf.Abs(rigidbody2D.velocity.x));
        animator.SetFloat("velocityY",rigidbody2D.velocity.y);
        animator.SetBool("isGround", physicsCheck.isGround);
        animator.SetBool("isCrouch", playerController.isCrouch);
        animator.SetBool("isDead",playerController.isDead);
        animator.SetBool("isAttack",playerController.isAttack);
        animator.SetBool("isOnWall",physicsCheck.isOnWall);
        animator.SetBool("isSlide", playerController.isSliding);
    }
    public void PlayHurt()
    {
        animator.SetTrigger("hurt");
    }
    public void PlayerAttack()
    {
        animator.SetTrigger("attack");
    }
}
