using NUnit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(DealDamageSystem))]
[BurstCompile]
public partial struct DamageSystem : ISystem
{
    ComponentLookup<DamageSys_OwnerComponent> m_damageCompsTCL;
    ComponentLookup<DamageSys_HealthComponent> m_healthCompsTCL;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DamageSystemEnable>();
        state.RequireForUpdate<DamageSys_OwnerComponent>();
        state.RequireForUpdate<DamageSys_HealthComponent>();

        m_damageCompsTCL = state.GetComponentLookup<DamageSys_OwnerComponent>(true);
        m_healthCompsTCL = state.GetComponentLookup<DamageSys_HealthComponent>(true);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        m_damageCompsTCL.Update(ref state);
        m_healthCompsTCL.Update(ref state);

        state.Dependency = new SetCollisionDamageJob
        {
            ecb = ecb,
            damageCompsTCL = m_damageCompsTCL,
            healthCompsTCL = m_healthCompsTCL,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

    }
}

[BurstCompile]
public partial struct SetCollisionDamageJob : ITriggerEventsJob
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer ecb;
    [ReadOnly]
    public ComponentLookup<DamageSys_OwnerComponent> damageCompsTCL;
    [ReadOnly]
    public ComponentLookup<DamageSys_HealthComponent> healthCompsTCL;
    public void Execute(TriggerEvent triggerEvent)
    {
        Entity entA = triggerEvent.EntityA;
        Entity entB = triggerEvent.EntityB;

        bool isDamagerA = damageCompsTCL.HasComponent(entA);
        bool isDamagerB = damageCompsTCL.HasComponent(entB);

        bool isHealthA = healthCompsTCL.HasComponent(entA);
        bool isHealthB = healthCompsTCL.HasComponent(entB);


        if (isDamagerA && isHealthB)
        {


            DamageSys_OwnerComponent damageComp;
            damageCompsTCL.TryGetComponent(entA, out damageComp);
            DamageSys_HealthComponent healthComp;
            healthCompsTCL.TryGetComponent(entB, out healthComp);





            if (1 == 0)
            {
            }
            else
            {
                ecb.AddComponent<DealDamageSys_OwnerComponent>(entB, new DealDamageSys_OwnerComponent
                { Value = damageComp.damage, OriginCharacter = Entity.Null, effectFrequenc = damageComp.effectFrequenc, effectCount = damageComp.effectFrequenc });
            }
            return;
        }

        if (isDamagerB && isHealthA)
        {

            DamageSys_OwnerComponent damageComp;
            damageCompsTCL.TryGetComponent(entB, out damageComp);
            DamageSys_HealthComponent healthComp;
            healthCompsTCL.TryGetComponent(entA, out healthComp);



            if (1 == 0)
            {
            }
            else
            {
                ecb.AddComponent<DealDamageSys_OwnerComponent>(entA, new DealDamageSys_OwnerComponent
                { Value = damageComp.damage, OriginCharacter = Entity.Null, effectFrequenc = damageComp.effectFrequenc, effectCount = damageComp.effectFrequenc });
            }

        }
        if (!isDamagerA && !isHealthB)
        {
            Debug.Log("DamageError");
        }
    }
}