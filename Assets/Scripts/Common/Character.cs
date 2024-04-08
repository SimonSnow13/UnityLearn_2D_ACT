using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveable
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxStamina;
    [HideInInspector]public float currentStamina;
    public float staminaRecoverSpeed;

    [Header("受击无敌")]
    public bool invulnerable;
    public float invulnerableDuration;
    [HideInInspector] public float invulnerableCounter;

    [Header("事件")]
    public UnityEvent<Transform> onHurt;
    public UnityEvent onDeath;
    public UnityEvent<Character> onHealthChange;

    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        //将character脚本注册进了ISaveale中
        ISaveable saveable = this;
        saveable.RegistSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegistSaveData();
    }



    private void Start()
    {
        //为了让敌人在游戏开始时获得血量
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        //无敌时间倒计时
        invulnerableCounter -= Time.deltaTime;
        if (invulnerableCounter <= 0)
        {
            invulnerable = false;
        }
        //体力值自动恢复
        if (currentStamina < maxStamina)
        {
            currentStamina += Time.deltaTime * staminaRecoverSpeed;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                onHealthChange?.Invoke(this);
                onDeath?.Invoke();
            }
        }
    }

    //受伤的计算方法
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
        {
            return;
        }
        if (currentHealth - attacker.damage > 0) 
        {
            currentHealth -= attacker.damage;
            onHurt?.Invoke(attacker.transform);
            TriggerInvulnerable();
        }
        else
        {
            Debug.Log("执行受伤事件");
            currentHealth = 0;
            onDeath?.Invoke();
        }

        onHealthChange?.Invoke(this);
    }

    //触发无敌时间
    public void TriggerInvulnerable() 
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    private void NewGame()
    {
        currentHealth = maxHealth;
        onHealthChange?.Invoke(this);
    }

    public void OnSlide(int cost)
    {
        currentStamina -= cost;
        onHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();//通过拖拽提前挂接
    }

    public void GetSaveData(Data data)
    {
        //更改ID对应的数值，如：切换场景再切换回来，原场景中野猪的坐标应该记录为最新的
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new Data.SerializeVector3(transform.position);
            data.floatSavedData[GetDataID().ID + "Health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "Stamina"] = this.currentStamina;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, new Data.SerializeVector3(transform.position));
            data.floatSavedData.Add(GetDataID().ID + "Health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "Stamina", this.currentStamina);
        }
    }

    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            this.currentHealth = data.floatSavedData[GetDataID().ID + "Health"];
            this.currentStamina = data.floatSavedData[GetDataID().ID + "Stamina"];

            //向UI Manager传递character
            onHealthChange?.Invoke(this);
        }
    }
}
