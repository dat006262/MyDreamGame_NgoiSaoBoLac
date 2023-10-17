using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class Status_UpdateSystem : SystemBase
{
    //public Action<float, float3> OnUpdateHealth;
    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<StatValueModify>();
        RequireForUpdate<CharacterStatValue>();

    }
    [BurstCompile]
    protected override void OnDestroy()
    {
    }
    [BurstCompile]
    protected override void OnUpdate()
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        foreach (var (statValue, statValueModifyBuffer, animator, transform, type, entity) in
            SystemAPI.Query<RefRW<CharacterStatValue>, DynamicBuffer<StatValueModify>, DynamicBuffer<AnimationParent_ElementComponent>,
                LocalTransform, TypeCharacter>().WithEntityAccess().WithNone<DeadCleanTag>())
        {
            for (var i = statValueModifyBuffer.Length - 1; i >= 0; i--)
            {
                if (statValueModifyBuffer.ElementAt(i).statType == StatType.Health)
                {
                    statValue.ValueRW.PlusHealth(statValueModifyBuffer.ElementAt(i).value);


                    GlobalAction.OnUpdateHealth.Invoke(statValueModifyBuffer.ElementAt(i).value, transform.Position);
                    statValueModifyBuffer.RemoveAt(i);
                    if (type.type == TypeCharac.Enemy)
                        GlobalAction.OnEnemyReceiveHit.Invoke(ecb, entity, animator[0].animParent);



                }

            }

            if (statValue.ValueRO.health <= 0)
            {
                GlobalAction.OnGrantEXP.Invoke(1);
                ecb.AddComponent<DeadCleanTag>(entity);
                ecb.AddComponent<DeadDestroyTag>(entity, new DeadDestroyTag { DeadAfter = -1 });
            }
        }


    }
}

