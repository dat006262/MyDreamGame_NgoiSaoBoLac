using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SkillCoolDownSystem : ISystem
{
    private EntityQuery SkillEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<SkillCoolDownSystemEnable>();
        state.RequireForUpdate<SkillCoolDownComponent>();
        SkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<SkillComponent>());


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
                        ref SkillCoolDownComponent coldow
                       )
    {
        if (coldow.canUse) return;
        coldow.remain -= deltaTime;
        if (!coldow.canUse) return;



    }
}

