using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;


[BurstCompile]
[UpdateAfter(typeof(HackStatusSystem))]
public partial struct ApplyModifySystem : ISystem
{
    private EntityQuery m_applyStatus;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<StatModify>();
        state.RequireForUpdate<CharacterStat>();
        state.RequireForUpdate<CheckNeedCalculate>();
        state.RequireForUpdate<ApplyModifySystemEnable>();

        m_applyStatus = state.GetEntityQuery(ComponentType.ReadOnly<CharacterStat>());

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



        state.Dependency = new ApplyModifyJob
        {
            ecbp = ecb.AsParallelWriter()
        }.ScheduleParallel(m_applyStatus, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct ApplyModifyJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        in HackInputComponent input, in CharacterStat stat, in DynamicBuffer<StatModify> dynamicBuffer,
                        in CheckNeedCalculate check
                        )
    {

        if (check.dirty)
        {
            var finalValue = stat.BaseValueHealth;
            //StartCalcucalte
            for (int i = 0; i < dynamicBuffer.Length; i++)
            {
                var modify = dynamicBuffer[i];

                if (modify.statType == StatType.Health)
                {

                    float sumPercentAdd = 0;
                    if (modify.statModType == StatModType.Flat)
                    {
                        finalValue += modify.Value;
                    }

                    else if (modify.statModType == StatModType.PercentAdd)
                    {
                        sumPercentAdd += modify.Value;
                        if (i + 1 >= dynamicBuffer.Length /*|| dynamicBuffer.ElementAt(i+1).statModType != StatModType.PercentAdd*/)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                    }

                    else if (modify.statModType == StatModType.PercentMulti)
                    {
                        finalValue *= 1 + modify.Value;
                    }


                }

                //End

            }

            Debug.Log("Applycomplete");
            ecbp.SetComponent<CheckNeedCalculate>(ciqi, ent, new CheckNeedCalculate { dirty = false });
            ecbp.SetComponent<CharacterStat>(ciqi, ent, new CharacterStat
            {
                BaseValueHealth = stat.BaseValueHealth,
                BaseValueMana = stat.BaseValueMana,
                BaseValueStrength = stat.BaseValueStrength,
                _healthValue = finalValue,
                _manaValue = stat._manaValue,
                _strengValue = stat._strengValue

            });
        }
    }
}

