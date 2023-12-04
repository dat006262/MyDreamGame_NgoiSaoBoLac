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
        // m_DeadDestroyEQG = state.GetEntityQuery(ComponentType.ReadOnly<DeadDestroyTag>());
        m_DeadDestroyEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
         .WithAll<DeadDestroyTag>()
         .WithAll<LocalTransform>()
         );
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
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecbEnd.AsParallelWriter(),
        }.ScheduleParallel(m_DeadDestroyEQG, state.Dependency);
        state.Dependency.Complete();
    }

    [BurstCompile]
    public partial struct DeadDestroyJob : IJobEntity
    {
        [ReadOnly]
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbp;

        public void Execute([ChunkIndexInQuery] int ciqi,
                            ref DeadDestroyTag dedtag,
                            in Entity ent,
                            in DynamicBuffer<Child> children)
        {

            dedtag.DeadAfter -= deltaTime;
            //  if (dedtag.DeadAfter <= 0) { ecbp.DestroyEntity(ciqi, ent); }
            if (dedtag.DeadAfter <= 0)
            {
                foreach (var child in children)
                {
                    ecbp.DestroyEntity(ciqi, child.Value);
                }
                ecbp.DestroyEntity(ciqi, ent);
            }
        }
    }
}
