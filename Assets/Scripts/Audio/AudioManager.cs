using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("组件")]
    public AudioSource BGMSource;
    public AudioSource SFXSource;
    public AudioMixer audioMixer;

    [Header("事件广播")]
    public FloatEventSO syncVolumeEvent;

    [Header("事件监听")]
    public AudioClipEventSO BGMEventSO;
    public AudioClipEventSO SFXEventSO;
    public FloatEventSO volumeChangeEventSO;
    public VoidEventSO pauseEvent;

    private void OnEnable()
    {
        SFXEventSO.OnEventRaised += OnSFXEvent;
        BGMEventSO.OnEventRaised += OnBGMEvent;
        volumeChangeEventSO.OnEventRaised += OnVolumeChangeEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;
    }
    private void OnDisable()
    {
        SFXEventSO.OnEventRaised -= OnSFXEvent;
        BGMEventSO.OnEventRaised -= OnBGMEvent;
        volumeChangeEventSO.OnEventRaised -= OnVolumeChangeEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;
    }


    private void OnVolumeChangeEvent(float amount)
    {
        audioMixer.SetFloat("MasterVolume", amount * 100 - 80);
    }

    void OnSFXEvent(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.Play();
    }
    void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }
    private void OnPauseEvent()
    {
        float amount;
        audioMixer.GetFloat("MasterVolume", out amount);
        syncVolumeEvent.RaiseEvent(amount);
    }
}
