using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct AnimatorSystem : ISystem
{
    private EntityQuery m_animEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<SpriteSheetAnimation>();
        m_animEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<SpriteSheetAnimation>());
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbEnd = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<ConfigComponent>();
    }
}
[BurstCompile]
public partial struct Animator_Job : IJobEntity
{

    public Entity configEntity;
    public ConfigComponent Config;
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi)
    {

    }
}