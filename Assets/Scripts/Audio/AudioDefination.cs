using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public AudioClipEventSO playAudioEvent;
    public AudioClip audioClip;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayAduioClip();
        }
    }

    public void PlayAduioClip()
    {
        playAudioEvent.RaiseEvent(audioClip);
    }
}
