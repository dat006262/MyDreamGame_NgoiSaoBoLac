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

public partial struct W_CamileSystem : ISystem
{
    private EntityQuery W_CamileSkillEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StatefulTriggerEvent>();
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<W_CamileSystemEnable>();
        state.RequireForUpdate<W_CamileComponent>();
        W_CamileSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<W_CamileComponent>());
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
        foreach (var (triggerEventBuffer, W_Camile, entity) in
                  SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<W_CamileComponent>>()
                      .WithEntityAccess())
        {
            for (int i = 0; i < triggerEventBuffer.Length; i++)
            {
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(entity);
                if (W_Camile.ValueRO.damageAfter <= 0 && W_Camile.ValueRO.active)
                {

                    ecb.AppendToBuffer<StatValueModify>(otherEntity, new StatValueModify
                    {
                        value = -W_Camile.ValueRO.DamageBasic,
                        statType = StatType.Health
                    }
                   );
                }

            }
            if (W_Camile.ValueRO.damageAfter <= 0 && W_Camile.ValueRO.active)
            {
                W_Camile.ValueRW.active = false;
                ecb.AddComponent<DeadDestroyTag>(entity, new DeadDestroyTag { DeadAfter = -1 });

            }

        }

        state.Dependency = new W_CamileCountDown
        {


            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(W_CamileSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct W_CamileCountDown : IJobEntity
{

    [ReadOnly]
    public float deltaTime;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref W_CamileComponent w_Camile, in DynamicBuffer<StatefulTriggerEvent> buffer
                       )
    {

        if (w_Camile.damageAfter >= 0)
            w_Camile.damageAfter -= deltaTime;




    }
}
