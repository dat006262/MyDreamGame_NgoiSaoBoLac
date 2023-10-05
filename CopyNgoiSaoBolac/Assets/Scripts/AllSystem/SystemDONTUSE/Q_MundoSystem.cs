using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct Q_MundoSystem : ISystem
{
    private EntityQuery Q_MundoSkillEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Q_MundoSystemEnable>();
        state.RequireForUpdate<Q_MundoComponent>();
        Q_MundoSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<Q_MundoComponent>());
        //
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


        //Skill Hoat dong
        state.Dependency = new Q_MundoWork
        {
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(Q_MundoSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }
}



[BurstCompile]
public partial struct Q_MundoWork : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent, ref LocalTransform Q_mundoTrans,
                        in Q_MundoComponent q_Mundo
                       )
    {


        //MoveToTarget
        var newLtrans = new LocalTransform
        {
            Position = Q_mundoTrans.Position,
            Rotation = Q_mundoTrans.Rotation,
            Scale = Q_mundoTrans.Scale
        };

        newLtrans.Position += deltaTime * Q_mundoTrans.Up() * q_Mundo.flySpeed;
        ecbp.SetComponent<LocalTransform>(ciqi, ent, newLtrans);







    }
}
