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

public partial struct E_MorganaSystem : ISystem
{
    private EntityQuery E_MorSkillEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StatefulTriggerEvent>();
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<E_MorganaSystemEnable>();
        state.RequireForUpdate<E_MorganaComponent>();
        E_MorSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<E_MorganaComponent>());
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
        foreach (var (triggerEventBuffer, e_Mor, entity) in
                  SystemAPI.Query<DynamicBuffer<StatefulTriggerEvent>, RefRW<E_MorganaComponent>>()
                      .WithEntityAccess())
        {
            if (e_Mor.ValueRO.countdown <= 0 && e_Mor.ValueRO.active)
            {
                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    var triggerEvent = triggerEventBuffer[i];
                    var otherEntity = triggerEvent.GetOtherEntity(entity);


                    ecb.AppendToBuffer<StatValueModify>(otherEntity, new StatValueModify
                    {
                        value = -e_Mor.ValueRO.BasicDamePerTurn,
                        statType = StatType.Health
                    }

                   );
                }
                e_Mor.ValueRW.countdown = e_Mor.ValueRO.effectFrequenc;
                e_Mor.ValueRW.turnCount--;

            }
            if (e_Mor.ValueRO.turnCount <= 0 && e_Mor.ValueRO.active)
            {
                e_Mor.ValueRW.active = false;
                ecb.AddComponent<DeadDestroyTag>(entity, new DeadDestroyTag { DeadAfter = -1 });

            }

        }

        state.Dependency = new E_MorCountDown
        {

            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(E_MorSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }

}
[BurstCompile]
public partial struct E_MorCountDown : IJobEntity
{

    public float deltaTime;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref E_MorganaComponent e_Morgana
                       )
    {
        if (e_Morgana.countdown >= 0)
            e_Morgana.countdown -= deltaTime;



    }
}
