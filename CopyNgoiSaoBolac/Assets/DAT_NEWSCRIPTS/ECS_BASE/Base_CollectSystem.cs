using DAT;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
public partial struct Base_CollectSystem : ISystem
{
    private EntityQuery m_ownerEQG;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Base_Enable>();

        m_ownerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Base_Tag>()
            );

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
        float delta = SystemAPI.Time.DeltaTime;

        state.Dependency = new Base_CollectJob
        {
            deltaTime = delta,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_ownerEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct Base_CollectJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi)
    {

    }

}