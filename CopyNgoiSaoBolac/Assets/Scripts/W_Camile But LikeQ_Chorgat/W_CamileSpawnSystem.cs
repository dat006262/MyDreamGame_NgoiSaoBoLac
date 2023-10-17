
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[UpdateAfter(typeof(PlayerInputSystem))]
[BurstCompile]
public partial struct W_CamileSpawnSystem : ISystem
{
    private EntityQuery m_OwnerEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AutoHitSystemEnable>();
        m_OwnerEQG = state.GetEntityQuery(ComponentType.ReadOnly<AutoHitSys_OwnerComponent>());

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

        state.Dependency = new AutoHit_SpawnJob
        {
            //configEntity = SystemAPI.GetSingletonEntity<ConfigComponent>(),
            //Config = config,
            currentTime = Time.timeAsDouble,
            deltaTime = Time.deltaTime,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_OwnerEQG, state.Dependency);
        state.Dependency.Complete();
        state.Dependency = new WaitToHitJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(m_OwnerEQG, state.Dependency);
    }
}

[BurstCompile]
public partial struct AutoHit_SpawnJob : IJobEntity
{
    //public Entity configEntity;
    //public ConfigComponent Config;
    [ReadOnly]
    public double currentTime;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, in AutoHitSys_OwnerComponent plComp,
                        in Entity ent, ref AutoHitSys_StatusComponent statusHit,
                        in LocalTransform ltrans, in WorldTransform wtrans)
    {

        AutoHitSys_OwnerComponent autoHitSys_Owner = plComp;
        // check if projectile equipment slot is expired
        if (autoHitSys_Owner.prefab == Entity.Null)
        {

        }
        else if (/*Config.isAutoHitClick && */statusHit.CanHit && !statusHit.Hitting && autoHitSys_Owner.active && autoHitSys_Owner.prefab != Entity.Null)
        {
            statusHit.Hitting = true;
            statusHit.WaitAnim = 1.5f;

        }
        else if (statusHit.Hitting && autoHitSys_Owner.active && autoHitSys_Owner.prefab != Entity.Null)
        {
            if (statusHit.WaitAnim < 0)
            {
                //ecbp.AddComponent<ConfigComponent>(ciqi, configEntity, new ConfigComponent { isAutoHitClick = false });
                Entity spawnedProj = ecbp.Instantiate(ciqi, autoHitSys_Owner.prefab);
                //  ecbp.AddComponent<DeadDestroyTag>(ciqi, spawnedProj, new DeadDestroyTag { DeadAfter = 3 });
                float3 spawnPos = ltrans.Position/* + ltrans.Up() * 0.5f * ltrans.Scale*/;
                float spawnScale = ltrans.Scale;


                ecbp.SetComponent<LocalTransform>(ciqi, spawnedProj, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = ltrans.Rotation,
                    Scale = spawnScale
                });

                statusHit.Hitting = false;
            }
        }
    }

}
[BurstCompile]
public partial struct WaitToHitJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, ref AutoHitSys_StatusComponent autoHitSys_Status)
    {


        //  if (dedtag.DeadAfter <= 0) { ecbp.DestroyEntity(ciqi, ent); }
        if (autoHitSys_Status.WaitAnim >= 0) { autoHitSys_Status.WaitAnim -= deltaTime; }

    }
}
