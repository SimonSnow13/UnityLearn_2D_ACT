using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;
    [Header("ÊÂ¼þ¼àÌý")]
    public FadeEventSO fadeEvent;

    private void OnEnable()
    {
        fadeEvent.OnEventRaised += OnFadeEvent;
    }
    private void OnDisable()
    {
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }

    void OnFadeEvent(Color target, float duration, bool fadeStart)
    {
        fadeImage.DOBlendableColor(target,duration);
    }
}
