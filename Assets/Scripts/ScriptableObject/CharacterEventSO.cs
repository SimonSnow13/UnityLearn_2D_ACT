using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    //声明一个事件，起名为OnEventRaised，意即当这个事件被调用时执行哪些操作，这个事件需要传入Character脚本作为参数
    public UnityAction<Character> OnEventRaised;

    //谁启用OnEventRaised事件，谁就将自身挂接的Character脚本传入
    public void RaiseEvent(Character character)
    {
        OnEventRaised?.Invoke(character);
    }
}
