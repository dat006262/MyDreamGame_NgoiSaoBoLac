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
public partial struct EnemyAiMove_CollectSystem : ISystem
{
    private EntityQuery m_ownerEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<EnemyAISystemEnable>();
        m_ownerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
        .WithAll<EnemyAIMoveTag>()
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


        Entity PlayerEntity = SystemAPI.GetSingletonEntity<InputSystemTag>();
        state.Dependency = new EnemyAiMove_CollectJob
        {
            PlayerEntity = SystemAPI.GetSingletonEntity<InputSystemTag>(),
            PlayerPos = state.EntityManager.GetComponentData<LocalTransform>(PlayerEntity).Position,
            deltaTime = delta,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_ownerEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct EnemyAiMove_CollectJob : IJobEntity
{
    public Entity PlayerEntity;
    public float3 PlayerPos;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, ref EnemyAIMove_TargetTo EnemyAIMove_TargetTo)
    {

        EnemyAIMove_TargetTo.playerEntity = PlayerEntity;
        EnemyAIMove_TargetTo.PlayerPos = PlayerPos;

    }

}