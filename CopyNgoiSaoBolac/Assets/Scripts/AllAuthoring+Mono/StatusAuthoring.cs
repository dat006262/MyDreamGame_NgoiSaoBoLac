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
                BaseValueStrength = 100,
                BaseValueMana = 100,
                _value = 0
            });
            AddBuffer<StatModify>();

        }
    }
}