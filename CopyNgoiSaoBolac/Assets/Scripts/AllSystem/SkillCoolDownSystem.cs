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
    private EntityQuery DealDamage_EffectEQG;
    private EntityQuery E_MorganaEffectEQG;
    private EntityQuery W_CamileEffectEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<SkillCoolDownSystemEnable>();
        state.RequireForUpdate<SkillCoolDownSys_OwnerComponent>();
        SkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<SkillComponent>());

        SkillEffectEQG = state.GetEntityQuery(ComponentType.ReadOnly<DealDamageSys_EffectCountDownComponent>());
        DealDamage_EffectEQG = state.GetEntityQuery(ComponentType.ReadOnly<DealDamageSys2_OwnerComponent>());
        E_MorganaEffectEQG = state.GetEntityQuery(ComponentType.ReadOnly<E_MorganaEffectTag>());
        W_CamileEffectEQG = state.GetEntityQuery(ComponentType.ReadOnly<W_CamileEffectTag>());
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

        state.Dependency = new DealDame_EfectCountDown
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(DealDamage_EffectEQG, state.Dependency);
        state.Dependency.Complete();

        state.Dependency = new RemoveE_MorganaJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(E_MorganaEffectEQG, state.Dependency);
        state.Dependency.Complete();

        state.Dependency = new RemoveW_CamileJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(W_CamileEffectEQG, state.Dependency);
        state.Dependency.Complete();
    }

}

//---------------------------------------------------
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
//------------------------------------------------

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
[BurstCompile]
//-----------------------------------------
public partial struct DealDame_EfectCountDown : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref DynamicBuffer<DealDamageSys2_OwnerComponent> dealDamageEffect
                       )
    {
        for (int i = 0; i < dealDamageEffect.Length; i++)
        {
            if (dealDamageEffect.ElementAt(i).effectCount > 0)
            {
                dealDamageEffect.ElementAt(i).effectCount -= deltaTime;
            }




        }

    }
}
public partial struct RemoveE_MorganaJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent
                       )
    {
        ecbp.RemoveComponent<E_MorganaEffectTag>(ciqi, ent);

    }
}
public partial struct RemoveW_CamileJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent
                       )
    {
        ecbp.RemoveComponent<W_CamileEffectTag>(ciqi, ent);

    }
}