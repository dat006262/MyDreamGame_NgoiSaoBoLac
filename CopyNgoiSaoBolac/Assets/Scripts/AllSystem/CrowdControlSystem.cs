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

//[BurstCompile]
//public partial struct CownControlJob : IJobEntity
//{

//    public EntityCommandBuffer.ParallelWriter ecbp;
//    [BurstCompile]
//    private void Execute([ChunkIndexInQuery] int ciqi, ref DynamicBuffer<CowdControl_Component> cownControls,
//        in CowdControlTag spawnerComp, ref PlayerMove_OwnerComponent playerMove_Owner)
//    {
//        for (int i = cownControls.Length - 1; i >= 0; i--)
//        {
//            if (cownControls[i].type == CowdControlType.Slow)
//            {
//                if (cownControls[i].time >= 0)
//                {
//                    // cownControls[i].time -= Time.deltaTime;

//                }
//                // playerMove_Owner.moveSpeed = playerMove_Owner.moveSpeed / 2;
//            }
//            if (cownControls[i].type == CowdControlType.Stun)
//            {
//                if (cownControls[i].time <= 0)
//                {
//                    Debug.Log("Stop Stun");
//                    cownControls.RemoveAt(i);

//                }
//            }
//        }
//    }
//}

