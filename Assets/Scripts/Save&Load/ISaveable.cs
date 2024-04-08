using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    DataDefination GetDataID();

    //ע����Ҫ�洢�����Ϣ������
    void RegistSaveData() 
    {
        //�ĸ��ű��ҽ�������ӿڣ��ͻᱻע���DataManager��ʵ����
        DataManager.instance.RegisterSaveData(this);
    }
    
    //ע����Ҫ�洢�����Ϣ������(���ɫ��������)
    void UnRegistSaveData() => DataManager.instance.UnRegisterSaveData(this);//����һ��ע�᷽�����﷨�ǣ���д��
    
    //��ȡ����Ҫ�������Ϣ
    void GetSaveData(Data data);
    
    //֪ͨ���ýӿڵĶ����ȡ����
    void LoadData(Data data);
}
