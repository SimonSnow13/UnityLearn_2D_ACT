using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    //����һ���¼�������ΪOnEventRaised���⼴������¼�������ʱִ����Щ����������¼���Ҫ����Character�ű���Ϊ����
    public UnityAction<Character> OnEventRaised;

    //˭����OnEventRaised�¼���˭�ͽ�����ҽӵ�Character�ű�����
    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
