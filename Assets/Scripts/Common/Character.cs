using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveable
{
    [Header("��������")]
    public float maxHealth;
    public float currentHealth;
    public float maxStamina;
    [HideInInspector]public float currentStamina;
    public float staminaRecoverSpeed;

    [Header("�ܻ��޵�")]
    public bool invulnerable;
    public float invulnerableDuration;
    [HideInInspector] public float invulnerableCounter;

    [Header("�¼�")]
    public UnityEvent<Transform> onHurt;
    public UnityEvent onDeath;
    public UnityEvent<Character> onHealthChange;

    [Header("�¼�����")]
    public VoidEventSO newGameEvent;

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        //��character�ű�ע�����ISaveale��
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
        //Ϊ���õ�������Ϸ��ʼʱ���Ѫ��
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    private void Update()
    {
        //�޵�ʱ�䵹��ʱ
        invulnerableCounter -= Time.deltaTime;
        if (invulnerableCounter <= 0)
        {
            invulnerable = false;
        }
        //����ֵ�Զ��ָ�
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

    //���˵ļ��㷽��
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
            Debug.Log("ִ�������¼�");
            currentHealth = 0;
            onDeath?.Invoke();
        }

        onHealthChange?.Invoke(this);
    }

    //�����޵�ʱ��
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
        return GetComponent<DataDefination>();//ͨ����ק��ǰ�ҽ�
    }

    public void GetSaveData(Data data)
    {
        //����ID��Ӧ����ֵ���磺�л��������л�������ԭ������Ұ�������Ӧ�ü�¼Ϊ���µ�
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

            //��UI Manager����character
            onHealthChange?.Invoke(this);
        }
    }
}
