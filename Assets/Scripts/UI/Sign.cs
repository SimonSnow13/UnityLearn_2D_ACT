using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class Sign : MonoBehaviour
{
    public GameObject signSprite;
    private bool canPress;
    private Animator animator;
    public Transform playerTrans;
    private PlayerInputControl playerInputControl;
    private IInteractable targetItem;

    private void Awake()
    {
        animator = signSprite.GetComponent<Animator>();
        //获取PlayerInputControl组件并启用
        playerInputControl = new PlayerInputControl();
        playerInputControl.Enable();
    }
    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInputControl.Gameplay.Confirm.started += OnConfirm;
    }


    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        canPress = false;
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            targetItem = other.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        //输入设备更改后按下的瞬间进行判定
        if (actionChange == InputActionChange.ActionStarted)
        {
            var device = ((InputAction)obj).activeControl.device;

            switch (device.device)
            {
                case Keyboard:
                    animator.Play("SignKeyboard");
                    break;
                case XInputController:
                    animator.Play("SignXbox");
                    break;
            }
        }
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
        }
    }
}
