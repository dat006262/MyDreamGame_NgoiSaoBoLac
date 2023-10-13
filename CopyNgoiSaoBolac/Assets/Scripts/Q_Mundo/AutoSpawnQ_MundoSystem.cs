
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public partial struct AutoSpawnQ_Mundo : ISystem
{
    private EntityQuery m_OwnerEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnLineSkillSystemEnble>();
        m_OwnerEQG = state.GetEntityQuery(ComponentType.ReadOnly<Q_MundoAutoSpawnComponent>());

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        state.Dependency = new LineSkill_AutoSpawnJob
        {

            currentTime = Time.timeAsDouble,
            deltaTime = Time.deltaTime,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_OwnerEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct LineSkill_AutoSpawnJob : IJobEntity
{

    [ReadOnly]
    public double currentTime;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, ref Q_MundoAutoSpawnComponent plComp,
                        in Entity ent,
                        in LocalTransform ltrans, in WorldTransform wtrans)
    {

        Q_MundoAutoSpawnComponent q_MundoAutoSpawnComponent = plComp;
        // check if projectile equipment slot is expired
        if (q_MundoAutoSpawnComponent.prefab == Entity.Null)
        {

        }
        else if (q_MundoAutoSpawnComponent.active && q_MundoAutoSpawnComponent.prefab != Entity.Null && q_MundoAutoSpawnComponent.cooldowmRemain <= 0)
        {

            Entity spawnedProj = ecbp.Instantiate(ciqi, q_MundoAutoSpawnComponent.prefab);
            //  ecbp.AddComponent<DeadDestroyTag>(ciqi, spawnedProj, new DeadDestroyTag { DeadAfter = 5f });
            float3 spawnPos = ltrans.Position/* + ltrans.Up() * 0.5f * ltrans.Scale*/;
            float spawnScale = ltrans.Scale;

            ecbp.SetComponent<LocalTransform>(ciqi, spawnedProj, new LocalTransform
            {
                Position = spawnPos,
                Rotation = ltrans.Rotation /** Quaternion.Euler(0, 0, 90)*/,
                Scale = spawnScale
            });
            plComp.cooldowmRemain = q_MundoAutoSpawnComponent.cooldownTime;


        }
        else
        {
            plComp.cooldowmRemain -= deltaTime;
        }
    }

}