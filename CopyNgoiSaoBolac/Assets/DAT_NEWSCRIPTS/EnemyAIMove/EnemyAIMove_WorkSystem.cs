using DAT;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;



[BurstCompile]
public partial struct EnemyAI_WorkSystem : ISystem
{
    private EntityQuery m_ownerEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyAIMoveTag>();
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<EnemyAISystemEnable>();

        m_ownerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
        .WithAll<EnemyAIMoveTag>()
        );
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

        float delta = SystemAPI.Time.DeltaTime;
        state.Dependency = new EnemyAIMoveJob
        {
            deltaTime = delta,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_ownerEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct EnemyAIMoveJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, EnemyAIMove_InputAspect InputAspect,
                        ref PhysicsVelocity velocity, in Entity ent, in PhysicsMass mass, ref EnemyAIMove_CheckIsRight isRight,
                        ref LocalTransform ltrans, in WorldTransform wtrans)
    {
        float3 line = InputAspect.playerEnt.ValueRO.PlayerPos - ltrans.Position;
        float distances = float.MaxValue;
        float speed = InputAspect._moveSpeed.ValueRO.speed;
        float3 direction;
        if (line.x == 0 && line.y == 0)
        {
            distances = 0.5f;
            direction = new float3(0, 0, 0);
        }
        else { distances = math.length(line); direction = math.normalize(line); }

        InputAspect.distances.ValueRW.distances = distances;
        velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, direction * speed * deltaTime, wtrans.Position);
        RaycastInput raycastInput = new RaycastInput()
        {
            Start = ltrans.Position,
            End = ltrans.Position - direction * 10,//gan dung
            Filter = new CollisionFilter
            {
                //BelongsTo = (uint)layer.UFOs,
                //  CollidesWith = (uint)layer.WorldBounds,
                GroupIndex = 0
            }
        };
#if UNITY_EDITOR
        Debug.DrawLine(raycastInput.Start, raycastInput.End, Color.red, 0);
        Debug.DrawLine(raycastInput.Start, raycastInput.Start + direction * 10, Color.green, 0);
#endif
    }

}