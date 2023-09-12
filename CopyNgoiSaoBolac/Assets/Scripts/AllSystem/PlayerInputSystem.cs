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


//[UpdateAfter(typeof(GameSystem))]
public partial class PlayerInputSystem : SystemBase
{
    private Entity _playerEntity;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerInputSystemEnable>();
        RequireForUpdate<PlayerInputComponent>();
    }


    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()//de cai dat cach nhan: VD di chuyen nhan giu dc con ban thì phai nhan
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
        Entities.ForEach((in PlayerComponent plComp, in Entity ent) =>
        {
            var plInpComp = SystemAPI.GetComponent<PlayerInputComponent>(ent);
            ecb.SetComponent<PlayerInputComponent>(ent, new PlayerInputComponent
            {
                Up = new PlayerInputComponent.InputPair
                {
                    keyCode = plInpComp.Up.keyCode,
                    keyVal = Input.GetKey(plInpComp.Up.keyCode)
                },
                Down = new PlayerInputComponent.InputPair
                {
                    keyCode = plInpComp.Down.keyCode,
                    keyVal = Input.GetKey(plInpComp.Down.keyCode)
                },
                Left = new PlayerInputComponent.InputPair
                {
                    keyCode = plInpComp.Left.keyCode,
                    keyVal = Input.GetKey(plInpComp.Left.keyCode)
                },
                Right = new PlayerInputComponent.InputPair
                {
                    keyCode = plInpComp.Right.keyCode,
                    keyVal = Input.GetKey(plInpComp.Right.keyCode)
                },
                Shoot = new PlayerInputComponent.InputPair
                {
                    keyCode = plInpComp.Shoot.keyCode,
                    keyVal = Input.GetKeyDown(plInpComp.Shoot.keyCode)
                },
                Teleport = new PlayerInputComponent.InputPair
                {
                    keyCode = plInpComp.Teleport.keyCode,
                    keyVal = Input.GetKeyUp(plInpComp.Teleport.keyCode)
                },
            });
        }).Run();
    }


}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    private EntityQuery m_playersEQG;
    private EntityQuery m_boundsGroup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // at least one player in the scene
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<PlayerInputComponent>();
        state.RequireForUpdate<PlayerMovementSystemEnable>();

        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
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

        float3 targetAreaBL = float3.zero;
        float3 targetAreaTR = float3.zero;
        float delta = SystemAPI.Time.DeltaTime;
        state.Dependency = new MovementJob
        {
            deltaTime = delta,
            ecbp = ecb.AsParallelWriter(),
            targetAreaBL = targetAreaBL,
            targetAreaTR = targetAreaTR
        }.ScheduleParallel(m_playersEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct MovementJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    [ReadOnly]
    public float3 targetAreaBL;
    [ReadOnly]
    public float3 targetAreaTR;
    public void Execute([ChunkIndexInQuery] int ciqi, in PlayerComponent plComp,
                        ref PhysicsVelocity velocity, in Entity ent,
                        in PlayerInputComponent input, in PhysicsMass mass,
                        ref LocalTransform ltrans, in WorldTransform wtrans)
    {
        float rotateSpeed = plComp.rotateSpeed;
        float moveSpeed = plComp.moveSpeed;
        // rotate
        if (input.Left.keyVal)
            velocity.ApplyImpulse(mass, ltrans.Position, ltrans.Rotation, -ltrans.Right() * moveSpeed * deltaTime, wtrans.Position);
        //   ltrans.Position += new float3(-1, 0, 0) * moveSpeed * deltaTime;

        if (input.Right.keyVal)
            velocity.ApplyImpulse(mass, ltrans.Position, ltrans.Rotation, ltrans.Right() * moveSpeed * deltaTime, wtrans.Position);
        //  ltrans.Position += new float3(1, 0, 0) * moveSpeed * deltaTime;
        // move
        if (input.Up.keyVal)
            velocity.ApplyImpulse(mass, ltrans.Position, ltrans.Rotation, ltrans.Up() * moveSpeed * deltaTime, wtrans.Position);
        //  ltrans.Position += new float3(0, 1, 0) * moveSpeed * deltaTime;

        if (input.Down.keyVal)
            velocity.ApplyImpulse(mass, ltrans.Position, ltrans.Rotation, -ltrans.Up() * moveSpeed * deltaTime, wtrans.Position);
        //   ltrans.Position += new float3(0, -1, 0) * moveSpeed * deltaTime;

        // teleport / hyperspace
        if (input.Shoot.keyVal)
        {

        }
    }

}