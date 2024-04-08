using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraControl : MonoBehaviour
{
    public CinemachineConfiner2D confiner;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    
    [Header("事件监听")]
    public VoidEventSO afterSceneLoadedEvent;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }

    /// <summary>
    /// 摄像机产生抖动效果
    /// </summary>
    public void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }
    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBound();
    }

    private void Start()
    {
        GetNewCameraBound();
    }

    void GetNewCameraBound()
    {
        //清空上一场景的缓存
        confiner.InvalidateCache();

        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj != null)
        {
            confiner.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        }
    }
}
