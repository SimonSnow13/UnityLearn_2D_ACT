using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    //角色坐标数据类型，使用字典方式（字符串对应坐标）
    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();

    public Dictionary<string, float> floatSavedData = new Dictionary<string, float>();

    public string sceneToSave;

    public void SaveGameScene(GameSceneSO savedScene)
    {
        //将场景SO序列化成Json文件
        sceneToSave = JsonUtility.ToJson(savedScene);
    }
    public GameSceneSO GetSavedScene()
    {
        //将Json文件反序列化成场景SO
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);
        return newScene;
    }
    //将Vector3序列化
    public class SerializeVector3
    {
        public float x, y, z;

        //创建构造函数，在序列化Vector3时会被调用
        public SerializeVector3(Vector3 pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }
        //构造函数，反向序列化成Vector3时调用
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
