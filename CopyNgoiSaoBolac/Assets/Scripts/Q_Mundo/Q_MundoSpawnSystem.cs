
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

[UpdateAfter(typeof(PlayerInputSystem))]
[BurstCompile]
public partial struct Q_MundoSpawnSystem : ISystem
{
    private EntityQuery m_OwnerEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnLineSkillSystemEnble>();
        m_OwnerEQG = state.GetEntityQuery(ComponentType.ReadOnly<SpawnLineSkillSys_OwnerComponent>());

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

        state.Dependency = new LineSkill_SpawnJob
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
public partial struct LineSkill_SpawnJob : IJobEntity
{
    public Entity configEntity;
    public ConfigComponent Config;
    [ReadOnly]
    public double currentTime;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, in SpawnLineSkillSys_OwnerComponent plComp,
                        in Entity ent, ref PlayerInput_OwnerComponent input,
                        in LocalTransform ltrans, in WorldTransform wtrans)
    {

        SpawnLineSkillSys_OwnerComponent spawnLineSkilSys_Owner = plComp;
        // check if projectile equipment slot is expired
        if (spawnLineSkilSys_Owner.prefab == Entity.Null)
        {

        }
        else
        // shoot
        if (Config.islineSkillClick && spawnLineSkilSys_Owner.active && spawnLineSkilSys_Owner.prefab != Entity.Null)
        {

            ecbp.AddComponent<ConfigComponent>(ciqi, configEntity, new ConfigComponent { islineSkillClick = false });

            Entity spawnedProj = ecbp.Instantiate(ciqi, spawnLineSkilSys_Owner.prefab);
            ecbp.AddComponent<DeadDestroyTag>(ciqi, spawnedProj, new DeadDestroyTag { DeadAfter = 5f });
            float3 spawnPos = ltrans.Position/* + ltrans.Up() * 0.5f * ltrans.Scale*/;
            float spawnScale = ltrans.Scale;

            ecbp.SetComponent<LocalTransform>(ciqi, spawnedProj, new LocalTransform
            {
                Position = spawnPos,
                Rotation = ltrans.Rotation /** Quaternion.Euler(0, 0, 90)*/,
                Scale = spawnScale
            });



        }
    }

}

