using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DeadDestroySystem))]
public partial struct Q_TemmoSystem : ISystem
{

    private EntityQuery Q_TemmoSkillEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Q_TemmoSystemEnable>();
        state.RequireForUpdate<Q_TemmoComponent>();
        state.RequireForUpdate<Q_TemmoSaveTargetComponent>();
        // Q_TemmoSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<Q_TemmoComponent>());
        Q_TemmoSkillEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
         .WithAll<Q_TemmoComponent>().WithNone<DeadDestroyTag>());
        //
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
        //Update TargetPos
        foreach (var Q_temmoTarget in SystemAPI.Query<RefRW<Q_TemmoSaveTargetComponent>>())
        {
            float3 targetPos;
            if (Q_temmoTarget.ValueRO.TargetTo != Entity.Null)
            {
                targetPos = state.EntityManager.GetComponentData<LocalTransform>(Q_temmoTarget.ValueRO.TargetTo).Position;
                Q_temmoTarget.ValueRW.TargetPos = targetPos;
            }

        }
        //Tim kiem nhung nguoi dang trong tam hoat dong Skill

        //Skill Hoat dong
        state.Dependency = new Q_TemmoWork
        {
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(Q_TemmoSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }
}



[BurstCompile]
public partial struct Q_TemmoWork : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent, ref LocalTransform Q_temmoTrans,
                        ref Q_TemmoComponent q_Temmo, ref Q_TemmoSaveTargetComponent q_TemmoTarget,
                        in DynamicBuffer<DealHardControl_Component> HardCCs, in DynamicBuffer<Debuff> Debuffs
                       )
    {
        if (q_TemmoTarget.TargetTo == Entity.Null)
        {
            Debug.Log("Q_temmo dont have target");


        }
        else
        {
            //MoveToTarget
            var newLtrans = new LocalTransform
            {
                Position = Q_temmoTrans.Position,
                Rotation = Q_temmoTrans.Rotation,
                Scale = Q_temmoTrans.Scale
            };
            if ((q_TemmoTarget.TargetPos - Q_temmoTrans.Position).x != 0 || (q_TemmoTarget.TargetPos - Q_temmoTrans.Position).y != 0)
            {
                newLtrans.Position += deltaTime * q_Temmo.flySpeed * math.normalize(q_TemmoTarget.TargetPos - Q_temmoTrans.Position);
                ecbp.SetComponent<LocalTransform>(ciqi, ent, newLtrans);
                if (math.distancesq(q_TemmoTarget.TargetPos, Q_temmoTrans.Position) <= q_Temmo.distancesDealDamage)
                {
                    if (q_Temmo.active)
                    {
                        //if have CrowdControl
                        foreach (var cc in HardCCs)
                        {
                            ecbp.AppendToBuffer<HardControl_Component>(ciqi, q_TemmoTarget.TargetTo, new HardControl_Component
                            {
                                type = cc.type,

                                time = cc.time,
                            });

                        }


                        //
                        ecbp.AppendToBuffer<StatValueModify>(ciqi, q_TemmoTarget.TargetTo, new StatValueModify
                        {
                            value = -q_Temmo.DamageBasic,
                            statType = StatType.Health
                        }
                        );

                        //Test SlowEffect
                        foreach (var cc in Debuffs)
                        {
                            ecbp.AppendToBuffer<StatModify>(ciqi, q_TemmoTarget.TargetTo, new StatModify
                            {
                                timeEffect = cc.timeEffect,
                                statType = cc.statType,
                                statModType = cc.statModType,
                                Value = cc.Value,
                                order = cc.order,
                                Source = Entity.Null
                            });
                            ecbp.SetComponent<CheckNeedCalculate>(ciqi, q_TemmoTarget.TargetTo, new CheckNeedCalculate { dirty = true });

                        }

                        //
                        ecbp.AddComponent<DeadDestroyTag>(ciqi, ent, new DeadDestroyTag { DeadAfter = -1 });
                        q_Temmo.active = false;
                    }


                }
            }
        }


    }
}