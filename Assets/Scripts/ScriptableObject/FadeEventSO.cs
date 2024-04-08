using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float, bool> OnEventRaised;

    public void RaiseEvent(Color target, float duration, bool fadeStart)
    {
        OnEventRaised?.Invoke(target, duration, fadeStart);
    }

    public void FadeStart(float duration)
    {
        RaiseEvent(Color.black, duration, true);
    }

    public void FadeEnd(float duration)
    {
        RaiseEvent(Color.clear, duration, false);
    }
}
