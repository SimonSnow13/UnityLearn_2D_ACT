using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("组件")]
    public PlayerStateBar playerStateBar;
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    public GameObject mobileTouch;
    public GameObject pausePanel;
    public Button settingsBtn;
    public Slider VolumeSlider;

    [Header("事件广播")]
    public VoidEventSO pauseEvent; 

    [Header("事件监听")]
    public SceneLoadEventSO unloadSceneEvent;
    public CharacterEventSO healthEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    private void Awake()
    {
#if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif
        settingsBtn.onClick.AddListener(TogglePausePanel);
    }

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadSceneEvent.LoadRequestEvent += OnUnloadSceneEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }
    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadSceneEvent.LoadRequestEvent -= OnUnloadSceneEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void TogglePausePanel()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0.0f;
            pauseEvent.RaiseEvent();
        }
    }

    void OnHealthEvent(Character character)
    {
        var percent = character.currentHealth / character.maxHealth;
        playerStateBar.OnHealthChange(percent);
        playerStateBar.OnStaminaChange(character);
    }

    private void OnUnloadSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        if (sceneToLoad.sceneType == SceneType.Menu)
        {
            playerStateBar.gameObject.SetActive(false);
        }
        else
        {
            playerStateBar.gameObject.SetActive(true);
        }
    }

    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        //面板中默认初始选中重新开始
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);
    }
    private void OnSyncVolumeEvent(float amount)
    {
        VolumeSlider.value = (amount + 80) / 100;
    }
}
