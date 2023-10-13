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


//[UpdateAfter(typeof(GameSystem))]
public partial class PlayerInputSystem : SystemBase
{
    private Entity _playerEntity;
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerInputSystemEnable>();
        RequireForUpdate<PlayerInput_OwnerComponent>();
        RequireForUpdate<CharacterStat>();


    }


    protected override void OnDestroy()
    {
    }

    protected override void OnUpdate()//de cai dat cach nhan: VD di chuyen nhan giu dc con ban thì phai nhan
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        Entities.ForEach((in CharacterStat plComp, in Entity ent, in PlayerInput_OwnerComponent input) =>
    {

        var plInpComp = SystemAPI.GetComponent<PlayerInput_OwnerComponent>(ent);
        ecb.SetComponent<PlayerInput_OwnerComponent>(ent, new PlayerInput_OwnerComponent
        {
            Up = new PlayerInput_OwnerComponent.InputPair
            {
                keyCode = plInpComp.Up.keyCode,
                keyVal = Input.GetKey(plInpComp.Up.keyCode)
            },
            Down = new PlayerInput_OwnerComponent.InputPair
            {
                keyCode = plInpComp.Down.keyCode,
                keyVal = Input.GetKey(plInpComp.Down.keyCode)
            },
            Left = new PlayerInput_OwnerComponent.InputPair
            {
                keyCode = plInpComp.Left.keyCode,
                keyVal = Input.GetKey(plInpComp.Left.keyCode)
            },
            Right = new PlayerInput_OwnerComponent.InputPair
            {
                keyCode = plInpComp.Right.keyCode,
                keyVal = Input.GetKey(plInpComp.Right.keyCode)
            },
            Shoot = new PlayerInput_OwnerComponent.InputPair
            {
                keyCode = plInpComp.Shoot.keyCode,
                keyVal = Input.GetKeyDown(plInpComp.Shoot.keyCode)
            },
            Teleport = new PlayerInput_OwnerComponent.InputPair
            {
                keyCode = plInpComp.Teleport.keyCode,
                keyVal = Input.GetKeyUp(plInpComp.Teleport.keyCode)
            },
        });
    }).Run();
    }


}

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateBefore(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    private EntityQuery m_playersEQG;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // at least one player in the scene
        state.RequireForUpdate<CharacterStat>();
        state.RequireForUpdate<PlayerInput_OwnerComponent>();

        m_playersEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
        .WithAll<CharacterStat>()
        .WithAll<PlayerInput_OwnerComponent>()
        .WithAll<HardControlTag>()
        //.WithNone<HardCowdControl_Component>()
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
        state.Dependency = new MovementJob
        {
            deltaTime = delta,
            ecbp = ecb.AsParallelWriter(),
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
    public void Execute([ChunkIndexInQuery] int ciqi, in CharacterStat plComp,
                        ref PhysicsVelocity velocity, in Entity ent,
                        in PlayerInput_OwnerComponent input, in PhysicsMass mass,
                        HardControlTag hardCC,
                        ref LocalTransform ltrans, in WorldTransform wtrans)
    {

        if (hardCC.isCC) return;
        float moveSpeed = plComp._speedValue;

        // rotate
        if (input.Left.keyVal)
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, -new float3(1, 0, 0) * moveSpeed * deltaTime, wtrans.Position);
        //   ltrans.Position += new float3(-1, 0, 0) * moveSpeed * deltaTime;
        if (input.Right.keyVal)
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, new float3(1, 0, 0) * moveSpeed * deltaTime, wtrans.Position);
        //  ltrans.Position += new float3(1, 0, 0) * moveSpeed * deltaTime;
        // move
        if (input.Up.keyVal)
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, new float3(0, 1, 0) * moveSpeed * deltaTime, wtrans.Position);
        //  ltrans.Position += new float3(0, 1, 0) * moveSpeed * deltaTime;

        if (input.Down.keyVal)
            velocity.ApplyImpulse(mass, wtrans.Position, wtrans.Rotation, -new float3(0, 1, 0) * moveSpeed * deltaTime, wtrans.Position);
        //   ltrans.Position += new float3(0, -1, 0) * moveSpeed * deltaTime;

    }

}