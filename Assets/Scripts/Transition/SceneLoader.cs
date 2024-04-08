using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour,ISaveable
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    private Vector3 positionToGo;
    private bool isLoading;

    [Header("事件广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public VoidEventSO newGameEvent;
    public SceneLoadEventSO sceneUnloadEvent;

    [Header("事件监听")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuLoadScene;
    private GameSceneSO sceneToLoad;
    private GameSceneSO currentScene;
    public VoidEventSO backToMenuEvent;

    public SceneLoadEventSO loadEventSO;

    [Header("淡入淡出效果")]
    private bool fadeScreen;
    public float fadeDuration;


    private void Awake()
    {
        //currentScene = firstLoadScene;
        //currentScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        ////Addressables.LoadSceneAsync(firstLoadScene.sceneReference,LoadSceneMode.Additive);
    }

    private void Start()
    {
        loadEventSO.RaiseLoadRequestEvent(menuLoadScene,menuPosition,true);
    }
    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegistSaveData();
    }
    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegistSaveData();
    }

    private void OnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;
        isLoading = true;

        sceneToLoad = sceneToGo;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if (currentScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }
    //卸载旧场景
    private IEnumerator UnloadPreviousScene()
    {
        //先让屏幕实现渐出渐入效果
        if (fadeScreen)
        {
            fadeEvent.FadeStart(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);

        //关闭人物
        playerTrans.gameObject.SetActive(false);

        yield return currentScene.sceneReference.UnLoadScene();

        sceneUnloadEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);

        LoadNewScene();

    }
    //加载新场景
    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        loadingOption.Completed += OnLoadCompleted;
    }
    //场景加载完毕
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentScene = sceneToLoad;
        //移动玩家角色至新场景
        playerTrans.position = positionToGo;
        //启用人物
        playerTrans.gameObject.SetActive(true);

        if (fadeScreen)
        {
            fadeEvent.FadeEnd(fadeDuration);
        }
        
        //场景加载完成后事件
        if (currentScene.sceneType == SceneType.Locations) 
        {
            afterSceneLoadedEvent.RaiseEvent();
        }

        isLoading = false;
    }
    //开始新游戏时加载场景
    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);

        loadEventSO.RaiseLoadRequestEvent(sceneToLoad,firstPosition,true);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentScene);
    }

    public void LoadData(Data data)
    {
        //通过player数据判断场景是否为空
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }
    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }
}
