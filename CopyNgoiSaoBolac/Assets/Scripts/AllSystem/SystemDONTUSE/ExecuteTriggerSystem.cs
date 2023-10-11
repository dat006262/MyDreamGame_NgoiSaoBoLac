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

[BurstCompile]
public partial struct ExecuteTriggerSystem : ISystem
{
    ComponentLookup<ExecuteTriggerSys_OwnerComponent> m_damageCompsTCL;
    ComponentLookup<ExecuteTriggerSys_HealthComponent> m_healthCompsTCL;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ExecuteTriggerSystemEnable>();
        state.RequireForUpdate<ExecuteTriggerSys_OwnerComponent>();
        state.RequireForUpdate<ExecuteTriggerSys_HealthComponent>();

        m_damageCompsTCL = state.GetComponentLookup<ExecuteTriggerSys_OwnerComponent>(true);
        m_healthCompsTCL = state.GetComponentLookup<ExecuteTriggerSys_HealthComponent>(true);

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

        state.Dependency = new SetTriggerDamageJob
        {
            ecb = ecb,
            damageCompsTCL = m_damageCompsTCL,
            healthCompsTCL = m_healthCompsTCL,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);

    }
}

[BurstCompile]
public partial struct SetTriggerDamageJob : ITriggerEventsJob
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer ecb;
    [ReadOnly]
    public ComponentLookup<ExecuteTriggerSys_OwnerComponent> damageCompsTCL;
    [ReadOnly]
    public ComponentLookup<ExecuteTriggerSys_HealthComponent> healthCompsTCL;
    public void Execute(TriggerEvent triggerEvent)
    {
        Entity entA = triggerEvent.EntityA;
        Entity entB = triggerEvent.EntityB;

        bool isDamagerA = damageCompsTCL.HasComponent(entA);
        bool isDamagerB = damageCompsTCL.HasComponent(entB);

        bool isHealthA = healthCompsTCL.HasComponent(entA);
        bool isHealthB = healthCompsTCL.HasComponent(entB);


        //if (isDamagerA && isHealthB)
        //{


        //    ExecuteTriggerSys_OwnerComponent damageComp;
        //    damageCompsTCL.TryGetComponent(entA, out damageComp);
        //    ExecuteTriggerSys_HealthComponent healthComp;
        //    healthCompsTCL.TryGetComponent(entB, out healthComp);

        //    if (damageComp.type == SkillType.E_Morgana)
        //    {
        //        ecb.AddComponent<E_MorganaEffectTag>(entB);
        //    }
        //    else if (damageComp.type == SkillType.W_Camile)
        //    {
        //        ecb.AddComponent<W_CamileEffectTag>(entB);
        //    }
        //    else if (damageComp.type == SkillType.Q_Mundo)
        //    {
        //        Debug.Log("DealDamageB");
        //        ecb.AddComponent<Q_MundoEffectTag>(entB);
        //        ecb.AddComponent<DeadDestroyTag>(entA, new DeadDestroyTag { DeadAfter = 0f });
        //    }



        //}

        /* else */

        if (isDamagerB/* trigger */ && isHealthA)
        {

            ExecuteTriggerSys_OwnerComponent damageComp;
            damageCompsTCL.TryGetComponent(entB, out damageComp);
            ExecuteTriggerSys_HealthComponent healthComp;
            healthCompsTCL.TryGetComponent(entA, out healthComp);
            if (damageComp.active)
            {
                if (damageComp.type == SkillType.E_Morgana)
                {
                    ecb.AddComponent<E_MorganaEffectTag>(entA);
                }
                else if (damageComp.type == SkillType.W_Camile)
                {
                    ecb.AddComponent<W_CamileEffectTag>(entA);
                }
                else if (damageComp.type == SkillType.Q_Mundo)
                {

                    Debug.Log("Q_MunTrigger");
                    ecb.AddComponent<Q_MundoEffectTag>(entA);

                    damageComp.active = false;
                    ecb.AddComponent<DeadDestroyTag>(entB, new DeadDestroyTag { DeadAfter = -1f });
                }

            }
        }
    }
}

