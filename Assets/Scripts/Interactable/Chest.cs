using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public Sprite openSprite;
    public Sprite closeSprite;
    private bool isDone;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }

    public void TriggerAction()
    {
        if (!isDone )
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        GetComponent<AudioDefination>().PlayAduioClip();
        this.gameObject.tag = "Untagged";
        isDone = true;
    }
}
