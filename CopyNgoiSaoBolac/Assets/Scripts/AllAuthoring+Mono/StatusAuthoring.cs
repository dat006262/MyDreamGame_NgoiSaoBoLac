using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class StatusAuthoring : MonoBehaviour
{
    // dieu kien de add vao??? tuong tu Cofig
    public class StatusBaker : Baker<StatusAuthoring>
    {
        public override void Bake(StatusAuthoring authoring)
        {
            AddComponent<CharacterStat>(new CharacterStat
            {
                BaseValueHealth = 100,
                _healthValue = 0,
                BaseValueStrength = 100,
                _strengValue = 0,
                BaseValueMana = 100,
                _manaValue = 0
            });
            AddComponent<CheckNeedCalculate>(new CheckNeedCalculate { dirty = true });
            AddBuffer<StatModify>();

        }
    }
}