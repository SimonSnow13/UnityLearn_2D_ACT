using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    //��ɫ�����������ͣ�ʹ���ֵ䷽ʽ���ַ�����Ӧ���꣩
    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();

    public Dictionary<string, float> floatSavedData = new Dictionary<string, float>();

    public string sceneToSave;

    public void SaveGameScene(GameSceneSO savedScene)
    {
        //������SO���л���Json�ļ�
        sceneToSave = JsonUtility.ToJson(savedScene);
    }
    public GameSceneSO GetSavedScene()
    {
        //��Json�ļ������л��ɳ���SO
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);
        return newScene;
    }
    //��Vector3���л�
    public class SerializeVector3
    {
        public float x, y, z;

        //�������캯���������л�Vector3ʱ�ᱻ����
        public SerializeVector3(Vector3 pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }
        //���캯�����������л���Vector3ʱ����
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
