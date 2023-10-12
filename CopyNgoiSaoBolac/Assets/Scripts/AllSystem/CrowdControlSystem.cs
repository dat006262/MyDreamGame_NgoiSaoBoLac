using System;
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

//Chu so huy CrowcotrolTag se co kha nang gay hieu ung len muc tieu cua no
public partial struct CrowdControlSystem : ISystem
{
    //private EntityQuery m_SkillEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CowdControlSystemEnable>();
        //   m_SkillEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
        //.WithAll<CowdControlTag>().WithAll<CharacterStat>()
        // );
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);//Tao ECB
        foreach (var (Buffer, transform, hardCCTag, entity) in
           SystemAPI.Query<DynamicBuffer<HardControl_Component>,
               LocalTransform, RefRW<HardControlTag>>().WithEntityAccess())
        {
            if (Buffer.Length == 0) { hardCCTag.ValueRW.isCC = false; }
            else { hardCCTag.ValueRW.isCC = true; }
            for (var i = Buffer.Length - 1; i >= 0; i--)
            {
                Buffer.ElementAt(i).time -= SystemAPI.Time.DeltaTime;
                if (Buffer.ElementAt(i).type == HardCowdControlType.KNOCKUP)
                {
                    if (Buffer.ElementAt(i).time <= 0)
                    {
                        Debug.Log("Stop Slow");
                        Buffer.RemoveAt(i);

                    }
                }
                else if (Buffer.ElementAt(i).type == HardCowdControlType.STUN)
                {
                    if (Buffer.ElementAt(i).time <= 0)
                    {
                        Debug.Log("Stop Stun");
                        Buffer.RemoveAt(i);

                    }
                }
            }
        }

        //state.Dependency = new CownControlJob
        //{
        //    ecbp = ecb.AsParallelWriter()
        //}.ScheduleParallel(m_SkillEQG, state.Dependency);

        //state.Dependency.Complete();
    }
}
