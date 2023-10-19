using DAT;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
[BurstCompile]
public partial struct MoveByInput_WorkSystem : ISystem
{
    private EntityQuery m_playersEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MoveByInputTag>();
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<PlayerMovementSystemEnable>();

        m_playersEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
        .WithAll<MoveByInputTag>()
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
        state.Dependency = new MoveByInputJob
        {
            deltaTime = delta,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(m_playersEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct MoveByInputJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, MoveByInput_InputAspect moveByInput_InputAspect,
                        ref PhysicsVelocity velocity, in Entity ent, in PhysicsMass mass, ref MoveByInput_CheckIsMove isMove, ref MoveByInput_CheckIsRight isRight,
                        ref LocalTransform ltrans, in WorldTransform wtrans)
    {

        float moveSpeed = moveByInput_InputAspect._moveSpeed.ValueRO.speed;


        if (moveByInput_InputAspect.input.ValueRO.Left)
        {
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, -new float3(1, 0, 0) * moveSpeed * deltaTime, wtrans.Position);
            isMove.ismove = true;
            isRight.isRight = false;
        }

        else if (moveByInput_InputAspect.input.ValueRO.Right)
        {
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, new float3(1, 0, 0) * moveSpeed * deltaTime, wtrans.Position);
            isMove.ismove = true;
            isRight.isRight = true;
        }


        if (moveByInput_InputAspect.input.ValueRO.Up)
        {
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, new float3(0, 1, 0) * moveSpeed * deltaTime, wtrans.Position);
            isMove.ismove = true;
        }


        else if (moveByInput_InputAspect.input.ValueRO.Down)
        {
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, -new float3(0, 1, 0) * moveSpeed * deltaTime, wtrans.Position);
            isMove.ismove = true;
        }



    }

}