
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


[BurstCompile]
public partial struct Q_TemmoSpawnSystem : ISystem
{
    private EntityQuery m_OwnerEQG;
    private EntityQuery EntityCanBeTargetEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SkillAutoTargetSystemEnable>();
        state.RequireForUpdate<CharacterStatValue>();
        m_OwnerEQG = state.GetEntityQuery(ComponentType.ReadOnly<SkillAutoTargetSys_OwnerComponent>());
        // EntityCanBeTargetEQG = state.GetEntityQuery(ComponentType.ReadOnly</*EnemyAI_OwnerComponent*/CharacterStatValue>());
        EntityCanBeTargetEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
        .WithAll<CharacterStatValue>()
        .WithAll<HardControl_Component>()
        );

    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ConfigComponent>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);


        NativeArray<Entity> EntityCanbeTargeted = new NativeArray<Entity>(EntityCanBeTargetEQG.CalculateEntityCount(), Allocator.TempJob);
        NativeArray<float3> EntityCanbeTargetedPos = new NativeArray<float3>(EntityCanBeTargetEQG.CalculateEntityCount(), Allocator.TempJob);
        int i = 0;
        foreach (var (trans, ent) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll</*EnemyAI_OwnerComponent*/CharacterStatValue>().WithAll<HardControl_Component>().WithEntityAccess())
        {
            EntityCanbeTargeted[i] = ent;
            EntityCanbeTargetedPos[i] = trans.ValueRO.Position;
            i++;
        }

        state.Dependency = new SkillAutoTarget_SpawnJob
        {
            EntityCanbeTarget = EntityCanbeTargeted,
            EntityCanbeTargetPos = EntityCanbeTargetedPos,
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
public partial struct SkillAutoTarget_SpawnJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<Entity> EntityCanbeTarget;
    [ReadOnly]
    public NativeArray<float3> EntityCanbeTargetPos;
    public float Range;
    [ReadOnly]
    public Entity configEntity;
    public ConfigComponent Config;
    [ReadOnly]
    public double currentTime;
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, in SkillAutoTargetSys_OwnerComponent plComp,
                        in Entity ent, ref PlayerInput_OwnerComponentOld input,
                        ref LocalTransform ltrans, in WorldTransform wtrans)
    {

        SkillAutoTargetSys_OwnerComponent skillAutoTarget_Owner = plComp;
        // check if projectile equipment slot is expired
        if (skillAutoTarget_Owner.prefab == Entity.Null)
        {

        }
        else
        // shoot
        if (Config.isAutoTargetSkillClick && skillAutoTarget_Owner.active && skillAutoTarget_Owner.prefab != Entity.Null)
        {
            float nearestDis = float.MaxValue;
            float3 targetPos_forLeastDist;
            bool target = false;
            Entity entityNearest = Entity.Null;

            for (int i = 0; i < EntityCanbeTargetPos.Length; i++)
            {
                if (EntityCanbeTarget[i].Index == ent.Index)
                {

                    continue;
                }
                if (EntityCanbeTargetPos[i].x - ltrans.Position.x == 0 && EntityCanbeTargetPos[i].y - ltrans.Position.y == 0 || EntityCanbeTarget[i].Index == ent.Index)
                {
                    entityNearest = EntityCanbeTarget[i];
                    break;
                }


                float sqDistEuclid = math.distancesq(EntityCanbeTargetPos[i], ltrans.Position);
                if (nearestDis > sqDistEuclid && sqDistEuclid <= plComp.RangeSkill * plComp.RangeSkill)
                {
                    target = true;
                    nearestDis = sqDistEuclid;
                    targetPos_forLeastDist = EntityCanbeTargetPos[i];
                    entityNearest = EntityCanbeTarget[i];
                }

            }

            ecbp.AddComponent<ConfigComponent>(ciqi, configEntity, new ConfigComponent { isAutoTargetSkillClick = false });
            if (target)
            {
                Entity spawnedProj = ecbp.Instantiate(ciqi, skillAutoTarget_Owner.prefab);
                Debug.Log("SpawnQ_Temm");
                float3 spawnPos = ltrans.Position + ltrans.Up() * 0.5f * ltrans.Scale;
                float spawnScale = ltrans.Scale;



                ecbp.AddComponent<Q_TemmoSaveTargetComponent>(ciqi, spawnedProj, new Q_TemmoSaveTargetComponent { TargetTo = entityNearest });


                ecbp.SetComponent<LocalTransform>(ciqi, spawnedProj, new LocalTransform
                {
                    Position = spawnPos,
                    Rotation = ltrans.Rotation,
                    Scale = spawnScale
                });
            }


        }
    }

}
