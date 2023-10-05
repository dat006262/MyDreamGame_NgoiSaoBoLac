using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial class Status_UpdateSystem : SystemBase
{
    public Action<float, float3> OnUpdateHealth;

    protected override void OnCreate()
    {
        RequireForUpdate<StatValueModify>();
        RequireForUpdate<CharacterStatValue>();

    }
    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()
    {
        foreach (var (statValue, statValueModifyBuffer, transform, entity) in
            SystemAPI.Query<RefRW<CharacterStatValue>, DynamicBuffer<StatValueModify>,
                LocalTransform>().WithEntityAccess())
        {
            for (var i = statValueModifyBuffer.Length - 1; i >= 0; i--)
            {
                if (statValueModifyBuffer.ElementAt(i).statType == StatType.Health)
                {
                    statValue.ValueRW.PlusHealth(statValueModifyBuffer.ElementAt(i).value);
                    OnUpdateHealth?.Invoke(statValueModifyBuffer.ElementAt(i).value, transform.Position);
                    statValueModifyBuffer.RemoveAt(i);
                }

            }
        }


    }
}

