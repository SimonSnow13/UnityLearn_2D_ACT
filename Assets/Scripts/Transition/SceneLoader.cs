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

    [Header("�¼��㲥")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public VoidEventSO newGameEvent;
    public SceneLoadEventSO sceneUnloadEvent;

    [Header("�¼�����")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuLoadScene;
    private GameSceneSO sceneToLoad;
    private GameSceneSO currentScene;
    public VoidEventSO backToMenuEvent;

    public SceneLoadEventSO loadEventSO;

    [Header("���뵭��Ч��")]
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
    //ж�ؾɳ���
    private IEnumerator UnloadPreviousScene()
    {
        //������Ļʵ�ֽ�������Ч��
        if (fadeScreen)
        {
            fadeEvent.FadeStart(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);

        //�ر�����
        playerTrans.gameObject.SetActive(false);

        yield return currentScene.sceneReference.UnLoadScene();

        sceneUnloadEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);

        LoadNewScene();

    }
    //�����³���
    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        loadingOption.Completed += OnLoadCompleted;
    }
    //�����������
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentScene = sceneToLoad;
        //�ƶ���ҽ�ɫ���³���
        playerTrans.position = positionToGo;
        //��������
        playerTrans.gameObject.SetActive(true);

        if (fadeScreen)
        {
            fadeEvent.FadeEnd(fadeDuration);
        }
        
        //����������ɺ��¼�
        if (currentScene.sceneType == SceneType.Locations) 
        {
            afterSceneLoadedEvent.RaiseEvent();
        }

        isLoading = false;
    }
    //��ʼ����Ϸʱ���س���
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
        //ͨ��player�����жϳ����Ƿ�Ϊ��
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
