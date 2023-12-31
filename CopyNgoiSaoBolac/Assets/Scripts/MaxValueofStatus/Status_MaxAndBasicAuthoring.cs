using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Status_MaxAndBasicAuthoring : MonoBehaviour
{
    public float BaseValueHealth = 100;
    public float BaseValueStrength = 100;
    public float BaseValueMana = 100;
    public float BaseValueSpeed = 100;
    // dieu kien de add vao??? tuong tu Cofig
    public class StatusBaker : Baker<Status_MaxAndBasicAuthoring>
    {

        public override void Bake(Status_MaxAndBasicAuthoring authoring)
        {
            AddComponent<CharacterStat>(new CharacterStat
            {
                BaseValueHealth = authoring.BaseValueHealth,
                _healthMaxValue = authoring.BaseValueHealth,
                BaseValueStrength = authoring.BaseValueStrength,
                _strengMaxValue = authoring.BaseValueStrength,
                BaseValueMana = authoring.BaseValueMana,
                _manaMaxValue = authoring.BaseValueMana,
                BaseValueSpeed = authoring.BaseValueSpeed,
                _speedValue = authoring.BaseValueSpeed,
            });
            AddComponent<CheckNeedCalculate>(new CheckNeedCalculate { dirty = true });
            AddBuffer<StatModify>();
            AddComponent<CharacterStatValue>(new CharacterStatValue
            {
                health = authoring.BaseValueHealth,
                mana = authoring.BaseValueMana,

            });
            AddBuffer<StatValueModify>();
        }
    }
}
public struct CharacterStat : IComponentData
{
    public float BaseValueStrength;
    public float _strengMaxValue;
    public float BaseValueHealth;
    public float _healthMaxValue;
    public float BaseValueMana;
    public float _manaMaxValue;
    public float BaseValueSpeed;
    public float _speedValue;
}
public enum StatType
{
    Strength = 1,
    Health = 2,
    Mana = 3,
    Speed = 4
}
public struct CharacterStatValue : IComponentData
{
    public bool isdirty;
    public float maxHealth;
    public float health;
    public float maxMana;
    public float mana;

    //Tired?
    void ResetHealth()
    {
        health = maxHealth;
    }
    void ResetMana()
    {
        mana = maxMana;
    }
    public void HealWhenMaxHealthChange(float old, float newvalue)
    {
        float x = newvalue / old;
        health *= x;
        maxHealth = newvalue;
    }
    public void ManaWhenMaxManaChange(float old, float newvalue)
    {
        float x = newvalue / old;
        mana *= x;
        maxMana = newvalue;
    }
    public void PlusHealth(float value)
    {
        health += value;
    }
    public void PlusMana(float value)
    {
        mana += value;
    }
}
public struct StatValueModify : IBufferElementData//DamageFrom?
{
    public float value;
    public StatType statType;

}