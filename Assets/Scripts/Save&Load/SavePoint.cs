using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour,IInteractable
{
    public SpriteRenderer spriteRenderer;
    public Sprite signLightSprite;
    public Sprite signDarkSprite;
    private bool isDone;
    public GameObject lightObj;

    [Header("ÊÂ¼þ¹ã²¥")]
    public VoidEventSO saveGameDataEvent;

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? signLightSprite : signDarkSprite;
        lightObj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = signLightSprite;
            lightObj.SetActive(true);

            saveGameDataEvent.RaiseEvent();

            this.gameObject.tag = "Untagged";
        }
    }
}
