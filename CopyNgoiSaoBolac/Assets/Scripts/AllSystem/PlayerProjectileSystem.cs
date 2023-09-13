
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
public partial struct PlayerProjectileSystem : ISystem
{

    private EntityQuery m_playersEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerProjectileSystemEnable>();
        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<PlayerMove_OwnerSystem>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        state.Dependency = new FireProjectileJob
        {
            configEntity = SystemAPI.GetSingletonEntity<Config>(),
            Config = config,
            currentTime = Time.timeAsDouble,
            deltaTime = Time.deltaTime,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_playersEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct FireProjectileJob : IJobEntity
{
    public Entity configEntity;
    public Config Config;
    [ReadOnly]
    public double currentTime;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, in PlayerMove_OwnerSystem plComp,
                        in EquippedProjectileDataComponent equippedProj,
                        in Entity ent, ref PlayerInput_OwnerComponent input,
                        in LocalTransform ltrans, in WorldTransform wtrans,
                        in PickupProjectileDataComponent defaultProjectileData)
    {
        EquippedProjectileDataComponent equippedProjectile = equippedProj;
        // check if projectile equipment slot is expired
        if (equippedProjectile.prefab == Entity.Null ||
            equippedProjectile.pickupTimeToLive > -1
        //   &&  currentTime >= equippedProjectile.pickupTime + equippedProjectile.pickupTimeToLive
        )
        {
            EquippedProjectileDataComponent newEquipped = new EquippedProjectileDataComponent
            {
                active = true,
                isCollisionInvulnerable = defaultProjectileData.isCollisionInvulnerable,
                timeToLive = defaultProjectileData.timeToLive,
                owner = ent,
                prefab = defaultProjectileData.prefab,
                activeVisual = defaultProjectileData.activeVisual,
                speed = defaultProjectileData.speed,
                scale = defaultProjectileData.scale,
                pickupTime = currentTime,
                pickupTimeToLive = -1
            };
            ecbp.SetComponent<EquippedProjectileDataComponent>(ciqi, ent, newEquipped);
            if (equippedProjectile.activeVisual != Entity.Null)
            {
                ecbp.AddComponent<DeadDestroyTag>(ciqi, equippedProjectile.activeVisual);
            }
        }
        else
        // shoot
        if (/*input.Shoot.keyVal*/Config.isFireClick && equippedProjectile.active && equippedProjectile.prefab != Entity.Null)
        {

            ecbp.AddComponent<Config>(ciqi, configEntity, new Config { isFireClick = false });
            Entity spawnedProj = ecbp.Instantiate(ciqi, equippedProjectile.prefab);
            float3 spawnPos = ltrans.Position + ltrans.Up() * 0.5f * ltrans.Scale;

            input.Shoot.keyVal = false;

            ecbp.SetComponent<LocalTransform>(ciqi, spawnedProj, new LocalTransform
            {
                Position = spawnPos,
                Rotation = ltrans.Rotation,
                Scale = equippedProjectile.scale
            });


            var mass = PhysicsMass.CreateDynamic(MassProperties.UnitSphere, 1);
            var velocity = new PhysicsVelocity();
            velocity.ApplyImpulse(mass, spawnPos, ltrans.Rotation, ltrans.Up() * equippedProjectile.speed * deltaTime, spawnPos);
            velocity.ApplyAngularImpulse(mass, new float3(0, +equippedProjectile.speed * deltaTime, 0));
            ecbp.AddComponent<PhysicsVelocity>(ciqi, spawnedProj, velocity);
            ecbp.AddComponent<PhysicsMass>(ciqi, spawnedProj,
                mass
            );
        }
    }

}

