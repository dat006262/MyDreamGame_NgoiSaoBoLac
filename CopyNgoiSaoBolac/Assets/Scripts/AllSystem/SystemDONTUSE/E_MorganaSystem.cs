using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial struct E_MorganaSystem : ISystem
{
    private EntityQuery E_MorSkillEQG;
    private EntityQuery E_MorTagNEG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<E_MorganaSystemEnable>();
        state.RequireForUpdate<E_MorganaComponent>();
        E_MorSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<E_MorganaComponent>());
        E_MorTagNEG = state.GetEntityQuery(ComponentType.ReadOnly<E_MorganaEffectTag>());
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
        NativeArray<Entity> E_MorTagEnt = new NativeArray<Entity>(E_MorTagNEG.CalculateEntityCount(), Allocator.TempJob);
        int i = 0;
        foreach (var (buffer, ent) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<E_MorganaEffectTag>().WithEntityAccess())
        {
            E_MorTagEnt[i] = ent;
            i++;
        }
        state.Dependency = new E_MorganaWork
        {
            Entities = E_MorTagEnt,
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(E_MorSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }

}
[BurstCompile]
public partial struct E_MorganaWork : IJobEntity
{
    public NativeArray<Entity> Entities;
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref E_MorganaComponent e_Morgana
                       )
    {
        if (!e_Morgana.active) { return; }
        e_Morgana.countdown -= deltaTime;
        if (e_Morgana.countdown <= 0)
        {
            //DealDamageHere
            foreach (Entity E_MorTagEnt in Entities)
            {
                ecbp.AppendToBuffer<DealDamageSys2_OwnerComponent>(ciqi, E_MorTagEnt, new DealDamageSys2_OwnerComponent
                { effectCount = 0.1f, effectFrequenc = 1, isLoop = true, loopCount = 1, Value = 1, OriginCharacter = Entity.Null, type = SkillType.E_Morgana });
            }

            e_Morgana.countdown = e_Morgana.effectFrequenc;
            e_Morgana.loopCount--;
        }
        if (e_Morgana.loopCount <= 0)
        {
            e_Morgana.active = false;
            ecbp.AddComponent<DeadDestroyTag>(ciqi, ent);
        }


    }
}
