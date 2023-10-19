using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial struct E_MorganaSpawnSystem : ISystem
{
    private EntityQuery m_OwnerEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<E_MorganaSpawnSystemEnable>();
        m_OwnerEQG = state.GetEntityQuery(ComponentType.ReadOnly<E_MorganaSpawn_OwnerComponent>());

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        state.Dependency = new E_Morgana_SpawnJob
        {
            configEntity = SystemAPI.GetSingletonEntity<ConfigComponent>(),
            Config = config,
            currentTime = Time.timeAsDouble,
            deltaTime = Time.deltaTime,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_OwnerEQG, state.Dependency);
        state.Dependency.Complete();
    }
}


[BurstCompile]
public partial struct E_Morgana_SpawnJob : IJobEntity
{
    public Entity configEntity;
    public ConfigComponent Config;
    [ReadOnly]
    public double currentTime;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, in E_MorganaSpawn_OwnerComponent plComp,
                        in Entity ent, ref PlayerInput_OwnerComponentOld input,
                        in LocalTransform ltrans, in WorldTransform wtrans)
    {

        E_MorganaSpawn_OwnerComponent E_morSpawn_Owner = plComp;
        if (E_morSpawn_Owner.prefab == Entity.Null)
        {

        }
        else
        // shoot
        if (Config.isAutoHitClick && E_morSpawn_Owner.active && E_morSpawn_Owner.prefab != Entity.Null)
        {

            ecbp.AddComponent<ConfigComponent>(ciqi, configEntity, new ConfigComponent { isAutoHitClick = false });
            Entity spawnedProj = ecbp.Instantiate(ciqi, E_morSpawn_Owner.prefab);
            float3 spawnPos = ltrans.Position/* + ltrans.Up() * 0.5f * ltrans.Scale*/;
            float spawnScale = ltrans.Scale;



            ecbp.SetComponent<LocalTransform>(ciqi, spawnedProj, new LocalTransform
            {
                Position = spawnPos,
                Rotation = ltrans.Rotation * Quaternion.Euler(0, 0, 90),
                Scale = spawnScale
            });
        }
    }

}

