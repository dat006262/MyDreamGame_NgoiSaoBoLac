using System;
using System.Collections.Generic;
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

public partial struct Status_CalculateMaxValueSystem : ISystem
{
    private EntityQuery m_applyStatus;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CharacterStat>();
        state.RequireForUpdate<CheckNeedCalculate>();
        state.RequireForUpdate<StatModify>();
        state.RequireForUpdate<CharacterStatValue>();
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

        //AutoRemove element of DynamicBuffer<StatModify> here
        foreach (var (Buffer, check, entity) in
          SystemAPI.Query<DynamicBuffer<StatModify>, RefRW<CheckNeedCalculate>>().WithEntityAccess())
        {

            for (var i = Buffer.Length - 1; i >= 0; i--)
            {
                if (Buffer.ElementAt(i).timeEffect <= 80)
                    Buffer.ElementAt(i).timeEffect -= SystemAPI.Time.DeltaTime;

                if (Buffer.ElementAt(i).timeEffect <= 0)
                {
                    Buffer.RemoveAt(i);
                    check.ValueRW.dirty = true;

                }
            }
        }







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
                       /* in HackInputComponent input, */in CharacterStat stat, in DynamicBuffer<StatModify> dynamicBuffer,
                        in CheckNeedCalculate check, ref CharacterStatValue statValue
                        )
    {

        if (check.dirty)
        {
            var x = dynamicBuffer.AsNativeArray();
            NativeArray<StatModify> copyBuffer = new NativeArray<StatModify>(x.Length, Allocator.Temp);
            x.CopyTo(copyBuffer);
            copyBuffer.Sort(new CompareModifierOrder());

            var finalHealthValue = stat.BaseValueHealth;
            var finalStrengthValue = stat.BaseValueHealth;
            var finalManaValue = stat.BaseValueHealth;
            var finalSpeedValue = stat.BaseValueSpeed;

            for (int i = 0; i < copyBuffer.Length; i++)
            {
                var modify = copyBuffer[i];

                if (modify.statType == StatType.Health)
                {

                    float sumPercentAdd = 0;
                    if (modify.statModType == StatModType.Flat)
                    {
                        finalHealthValue += modify.Value;
                    }
                    else if (modify.statModType == StatModType.PercentAdd)
                    {
                        sumPercentAdd += modify.Value;
                        if (i + 1 >= copyBuffer.Length /*|| hits[i].statModType != StatModType.PercentAdd*/)
                        {
                            finalHealthValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                    }
                    else if (modify.statModType == StatModType.PercentMulti)
                    {
                        finalHealthValue *= 1 + modify.Value;
                    }
                }

                if (modify.statType == StatType.Strength)
                {

                    float sumPercentAdd = 0;
                    if (modify.statModType == StatModType.Flat)
                    {
                        finalStrengthValue += modify.Value;
                    }
                    else if (modify.statModType == StatModType.PercentAdd)
                    {
                        sumPercentAdd += modify.Value;
                        if (i + 1 >= copyBuffer.Length /*|| hits[i].statModType != StatModType.PercentAdd*/)
                        {
                            finalStrengthValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                    }
                    else if (modify.statModType == StatModType.PercentMulti)
                    {
                        finalStrengthValue *= 1 + modify.Value;
                    }
                }
                if (modify.statType == StatType.Speed)
                {

                    float sumPercentAdd = 0;
                    if (modify.statModType == StatModType.Flat)
                    {
                        finalSpeedValue += modify.Value;
                    }
                    else if (modify.statModType == StatModType.PercentAdd)
                    {
                        sumPercentAdd += modify.Value;
                        if (i + 1 >= copyBuffer.Length /*|| hits[i].statModType != StatModType.PercentAdd*/)
                        {
                            finalSpeedValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                    }
                    else if (modify.statModType == StatModType.PercentMulti)
                    {
                        finalSpeedValue *= 1 + modify.Value;
                    }
                }
                if (modify.statType == StatType.Mana)
                {

                    float sumPercentAdd = 0;
                    if (modify.statModType == StatModType.Flat)
                    {
                        finalManaValue += modify.Value;
                    }
                    else if (modify.statModType == StatModType.PercentAdd)
                    {
                        sumPercentAdd += modify.Value;
                        if (i + 1 >= copyBuffer.Length /*|| hits[i].statModType != StatModType.PercentAdd*/)
                        {
                            finalManaValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }
                    }
                    else if (modify.statModType == StatModType.PercentMulti)
                    {
                        finalManaValue *= 1 + modify.Value;
                    }
                }
            }

            Debug.Log("Applycomplete");

            statValue.HealWhenMaxHealthChange(stat._healthMaxValue, finalHealthValue);
            statValue.ManaWhenMaxManaChange(stat._manaMaxValue, finalManaValue);

            ecbp.SetComponent<CheckNeedCalculate>(ciqi, ent, new CheckNeedCalculate { dirty = false });
            ecbp.SetComponent<CharacterStat>(ciqi, ent, new CharacterStat
            {
                BaseValueHealth = stat.BaseValueHealth,
                BaseValueMana = stat.BaseValueMana,
                BaseValueStrength = stat.BaseValueStrength,
                _healthMaxValue = finalHealthValue,
                _manaMaxValue = finalStrengthValue,
                _strengMaxValue = finalManaValue,
                BaseValueSpeed = stat.BaseValueSpeed,
                _speedValue = finalSpeedValue


            });

        }
    }
}
public struct CompareModifierOrder : IComparer<StatModify>
{
    public int Compare(StatModify mod1, StatModify mod2)//be=>lon
    {
        if (mod1.order > mod2.order)
            return -1;
        else if (mod1.order < mod2.order)
        {
            return 1;
        }
        return 0;
    }
}
