using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics.Stateful;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
//[UpdateInGroup(typeof(PhysicsSystemGroup))]
//[UpdateAfter(typeof(StatefulTriggerEventSystem))]
[UpdateBefore(typeof(DeadDestroySystem))]
public partial struct Q_MundoSystem : ISystem
{
    private EntityQuery Q_MundoSkillEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<StatefulTriggerEvent>();
        state.RequireForUpdate<Q_MundoSystemEnable>();
        state.RequireForUpdate<Q_MundoComponent>();
        //   Q_MundoSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<Q_MundoComponent>());
        Q_MundoSkillEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
       .WithAll<Q_MundoComponent>()
       .WithNone<DeadCleanTag>()
       );
        //
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
             .CreateCommandBuffer(state.WorldUnmanaged);

        var nonTriggerQuery = SystemAPI.QueryBuilder().WithNone<StatefulTriggerEvent>().Build();
        Assert.IsFalse(nonTriggerQuery.HasFilter(),
            "The use of EntityQueryMask in this system will not respect the query's active filter settings.");
        var nonTriggerMask = nonTriggerQuery.GetEntityQueryMask();


        foreach (var (triggerEventBuffer, Q_MunComp, localTrans, entity) in
                   SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<Q_MundoComponent>, LocalTransform>()
                       .WithEntityAccess())
        {
            if (triggerEventBuffer.Length > 0)
            {
                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    var triggerEvent = triggerEventBuffer[i];
                    var otherEntity = triggerEvent.GetOtherEntity(entity);

                    // exclude other triggers and processed events
                    if (triggerEvent.State == StatefulEventState.Stay ||
                        !nonTriggerMask.MatchesIgnoreFilter(otherEntity))
                    {
                        continue;
                    }

                    if (triggerEvent.State == StatefulEventState.Enter)
                    {
                        ecb.AppendToBuffer<StatValueModify>(otherEntity, new StatValueModify
                        {
                            value = -Q_MunComp.ValueRO.DamageBasic,
                            statType = StatType.Health

                        }
                        );

                        ecb.AddComponent<DeadCleanTag>(entity);
                        ecb.AddComponent<DeadDestroyTag>(entity, new DeadDestroyTag { DeadAfter = -1 });
                    }
                    else //Exit
                    {

                    }
                }

            }
        }



        //Skill Hoat dong
        state.Dependency = new Q_MundoFly
        {
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()

        }.ScheduleParallel(Q_MundoSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }
}



[BurstCompile]
public partial struct Q_MundoFly : IJobEntity
{
    [ReadOnly]
    public float deltaTime;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent, ref LocalTransform Q_mundoTrans,
                        in Q_MundoComponent q_Mundo
                       )
    {
        //MoveToTarget
        var newLtrans = new LocalTransform
        {
            Position = Q_mundoTrans.Position,
            Rotation = Q_mundoTrans.Rotation,
            Scale = Q_mundoTrans.Scale
        };
        newLtrans.Position += deltaTime * Q_mundoTrans.Up() * q_Mundo.flySpeed;
        ecbp.SetComponent<LocalTransform>(ciqi, ent, newLtrans);

    }
}
