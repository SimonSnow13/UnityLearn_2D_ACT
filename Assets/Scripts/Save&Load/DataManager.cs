using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    //使用列表来保存注册进来的信息
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;
    //存储数据的路径
    private string jsonFolder;

    [Header("事件监听")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private void Awake()
    {
        //确保只有一个实例，多余的就销毁
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        saveData = new Data();

        jsonFolder = Application.persistentDataPath + "/SAVE DATA/";

        ReadSavedData();
    }
    void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }

    void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }
    public void Save()
    {
        foreach (var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
        var resultPath = jsonFolder + "data.sav";
        //将Data文件序列化成Json数据
        var jsonData = JsonConvert.SerializeObject(saveData);
        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        //将保存内容写成文本
        File.WriteAllText(resultPath, jsonData);
    }
    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }
    //读取已保存文件
    private void ReadSavedData()
    {
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);
            //将json数据反序列化成Data文件
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);

            saveData = jsonData;
        }
    }
    //观察者模式（广播模式），通过一个Manager去控制列表中注册的内容
    public void RegisterSaveData(ISaveable saveable)
    {
        //如果不在列表中，才会被注册添加进来
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }
    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }
}
