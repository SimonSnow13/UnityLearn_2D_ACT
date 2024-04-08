using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/AudioClipEventSO")]
public class AudioClipEventSO : ScriptableObject
{
    public UnityAction<AudioClip> OnEventRaised;

    public void RaiseEvent(AudioClip audioClip)
    {
        OnEventRaised?.Invoke(audioClip);
    }
}
