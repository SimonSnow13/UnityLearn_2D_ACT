using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    //ʹ���б�������ע���������Ϣ
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Data saveData;
    //�洢���ݵ�·��
    private string jsonFolder;

    [Header("�¼�����")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private void Awake()
    {
        //ȷ��ֻ��һ��ʵ��������ľ�����
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
        //��Data�ļ����л���Json����
        var jsonData = JsonConvert.SerializeObject(saveData);
        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }
        //����������д���ı�
        File.WriteAllText(resultPath, jsonData);
    }
    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }
    //��ȡ�ѱ����ļ�
    private void ReadSavedData()
    {
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);
            //��json���ݷ����л���Data�ļ�
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);

            saveData = jsonData;
        }
    }
    //�۲���ģʽ���㲥ģʽ����ͨ��һ��Managerȥ�����б���ע�������
    public void RegisterSaveData(ISaveable saveable)
    {
        //��������б��У��Żᱻע����ӽ���
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
