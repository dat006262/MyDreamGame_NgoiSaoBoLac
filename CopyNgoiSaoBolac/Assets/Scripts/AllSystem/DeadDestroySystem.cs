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
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[BurstCompile]
public partial struct DeadDestroySystem : ISystem
{
    private EntityQuery m_DeadDestroyEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<DeadDestroySystemEnable>();
        state.RequireForUpdate<DeadDestroyTag>();
        m_DeadDestroyEQG = state.GetEntityQuery(ComponentType.ReadOnly<DeadDestroyTag>());
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbEnd = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        state.Dependency = new DeadDestroyJob
        {
            ecbp = ecbEnd.AsParallelWriter(),
        }.ScheduleParallel(m_DeadDestroyEQG, state.Dependency);
        state.Dependency.Complete();
    }

    [BurstCompile]
    public partial struct DeadDestroyJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbp;

        public void Execute([ChunkIndexInQuery] int ciqi,
                            in DeadDestroyTag dedtag,
                            in Entity ent,
                            in DynamicBuffer<Child> children)
        {
            foreach (var child in children)
            {
                ecbp.DestroyEntity(ciqi, child.Value);
                Debug.Log("Destroy");
            }
            ecbp.DestroyEntity(ciqi, ent);

        }
    }
}
