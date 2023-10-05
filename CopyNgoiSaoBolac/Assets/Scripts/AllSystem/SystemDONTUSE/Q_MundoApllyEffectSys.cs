using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[UpdateAfter(typeof(ExecuteTriggerSystem))]
public partial struct Q_MundoApllyEffectSys : ISystem
{
    private EntityQuery Q_MundoEffectTag;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Q_MundoSystemEnable>();
        state.RequireForUpdate<Q_MundoEffectTag>();
        Q_MundoEffectTag = state.GetEntityQuery(ComponentType.ReadOnly<Q_MundoEffectTag>());
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
        state.Dependency = new Q_MundoEffectWork
        {
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(Q_MundoEffectTag, state.Dependency);
        state.Dependency.Complete();
    }

}

[BurstCompile]
public partial struct Q_MundoEffectWork : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent, ref LocalTransform Q_mundoTrans,
                        in Q_MundoEffectTag q_Mundo
                       )
    {
        Debug.Log("Q_MundoEffectWork");
        ecbp.RemoveComponent<Q_MundoEffectTag>(ciqi, ent);

        ecbp.AppendToBuffer<DealDamageSys2_OwnerComponent>(ciqi, ent, new DealDamageSys2_OwnerComponent
        { effectCount = 0.1f, effectFrequenc = 1, isLoop = true, loopCount = 1, Value = 97, OriginCharacter = Entity.Null, type = SkillType.E_Morgana });





    }
}