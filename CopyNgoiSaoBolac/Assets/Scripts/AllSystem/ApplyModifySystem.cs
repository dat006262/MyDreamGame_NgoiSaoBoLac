using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;


[BurstCompile]
[UpdateAfter(typeof(HackJob))]
public partial struct ApplyModifySystem : ISystem
{
    private EntityQuery m_applyStatus;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<StatModify>();
        state.RequireForUpdate<CharacterStat>();
        state.RequireForUpdate<ApplyModifySystemEnable>();

        m_applyStatus = state.GetEntityQuery(ComponentType.ReadOnly<CharacterStat>());

    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);



        state.Dependency = new ApplyModifyJob
        {
            ecbp = ecb.AsParallelWriter()
        }.ScheduleParallel(m_applyStatus, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct ApplyModifyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        in HackInputComponent input, in CharacterStat stat, in DynamicBuffer<StatModify> dynamicBuffer
                        )
    {


        for (int i = 0; i < dynamicBuffer.Length; i++)
        {

            Debug.Log(i);
        }


    }
}

