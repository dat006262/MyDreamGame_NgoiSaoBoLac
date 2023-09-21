using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial struct W_CamileSystem : ISystem
{
    private EntityQuery W_CamileSkillEQG;
    private EntityQuery W_CamileTagNEG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<W_CamileSystemEnable>();
        state.RequireForUpdate<W_CamileComponent>();
        W_CamileSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<W_CamileComponent>());
        W_CamileTagNEG = state.GetEntityQuery(ComponentType.ReadOnly<W_CamileEffectTag>());
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
        //Tim kiem nhung nguoi dang trong tam hoat dong Skill
        NativeArray<Entity> W_CamileTagEnt = new NativeArray<Entity>(W_CamileTagNEG.CalculateEntityCount(), Allocator.TempJob);
        int i = 0;
        foreach (var (buffer, ent) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<W_CamileEffectTag>().WithEntityAccess())
        {
            W_CamileTagEnt[i] = ent;
            i++;
        }
        //Skill Hoat dong
        state.Dependency = new W_CamileWork
        {
            Entities = W_CamileTagEnt,
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(W_CamileSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct W_CamileWork : IJobEntity
{
    public NativeArray<Entity> Entities;
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent,
                        ref W_CamileComponent w_Camile
                       )
    {
        if (w_Camile.damageAfter > 0 && w_Camile.damageAfter < 10)
            w_Camile.damageAfter -= deltaTime;
        if (w_Camile.damageAfter <= 0)
        {
            //DealDamageHere
            foreach (Entity E_MorTagEnt in Entities)
            {
                ecbp.AppendToBuffer<DealDamageSys2_OwnerComponent>(ciqi, E_MorTagEnt, new DealDamageSys2_OwnerComponent
                { effectCount = 0.1f, effectFrequenc = 1, isLoop = true, loopCount = 1, Value = w_Camile.DamageBasic, OriginCharacter = Entity.Null, type = SkillType.E_Morgana });
            }
            w_Camile.damageAfter = 15;//ko dem nguoc nx , cho den khi destroy
            ecbp.AddComponent<DeadDestroyTag>(ciqi, ent);

        }



    }
}
