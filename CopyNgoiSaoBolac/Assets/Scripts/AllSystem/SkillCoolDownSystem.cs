using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SkillCoolDownSystem : ISystem
{
    private EntityQuery SkillEQG;
    private EntityQuery SkillEffectEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<SkillCoolDownSystemEnable>();
        state.RequireForUpdate<SkillCoolDownSys_OwnerComponent>();
        SkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<SkillComponent>());

        SkillEffectEQG = state.GetEntityQuery(ComponentType.ReadOnly<DealDamageSys_EffectCountDownComponent>());
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


        state.Dependency = new SkillCoolDownJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(SkillEQG, state.Dependency);
        state.Dependency.Complete();


        //state.Dependency = new SkillEffectDownJob
        //{
        //    deltaTime = SystemAPI.Time.DeltaTime,
        //    ecbp = ecb.AsParallelWriter()

        //}.ScheduleParallel(SkillEffectEQG, state.Dependency);
        //state.Dependency.Complete();

    }

}


[BurstCompile]
public partial struct SkillCoolDownJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref SkillCoolDownSys_OwnerComponent coldow
                       )
    {
        if (coldow.canUse) return;
        coldow.remain -= deltaTime;
        if (!coldow.canUse) return;



    }
}

[BurstCompile]
public partial struct SkillEffectDownJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref DealDamageSys_EffectCountDownComponent skillEffect
                       )
    {
        if (skillEffect.canDamage) return;
        skillEffect.effectCountDown -= deltaTime;
        if (!skillEffect.canDamage) return;



    }
}
