using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    DataDefination GetDataID();

    //注册需要存储相关信息的内容
    void RegistSaveData() 
    {
        //哪个脚本挂接了这个接口，就会被注册进DataManager的实例中
        DataManager.instance.RegisterSaveData(this);
    }
    
    //注销需要存储相关信息的内容(如角色已死亡等)
    void UnRegistSaveData() => DataManager.instance.UnRegisterSaveData(this);//上面一种注册方法的语法糖，简化写法
    
    //获取具体要保存的信息
    void GetSaveData(Data data);
    
    //通知调用接口的对象读取数据
    void LoadData(Data data);
}
