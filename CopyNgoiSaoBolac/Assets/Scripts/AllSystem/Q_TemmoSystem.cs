using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct Q_TemmoSystem : ISystem
{

    private EntityQuery Q_TemmoSkillEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<Q_TemmoSystemEnable>();
        state.RequireForUpdate<Q_TemmoComponent>();
        Q_TemmoSkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<Q_TemmoComponent>());
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
        //Update TargetPos
        foreach (var Q_temmoTarget in SystemAPI.Query<RefRW<Q_TemmoTargetComponent>>())
        {
            float3 targetPos;
            if (Q_temmoTarget.ValueRO.TargetTo != Entity.Null)
            {
                targetPos = state.EntityManager.GetComponentData<LocalTransform>(Q_temmoTarget.ValueRO.TargetTo).Position;
                Q_temmoTarget.ValueRW.TargetPos = targetPos;
            }

        }
        //Tim kiem nhung nguoi dang trong tam hoat dong Skill

        //Skill Hoat dong
        state.Dependency = new Q_TemmoWork
        {
            //  x = SystemAPI.Query<DynamicBuffer<DealDamageSys2_OwnerComponent>>().WithAll<E_MorganaEffectTag>().WithEntityAccess(),
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter()

        }.ScheduleParallel(Q_TemmoSkillEQG, state.Dependency);
        state.Dependency.Complete();
    }
}



[BurstCompile]
public partial struct Q_TemmoWork : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in Entity ent, ref LocalTransform Q_temmoTrans,
                        ref Q_TemmoComponent q_Temmo, ref Q_TemmoTargetComponent q_TemmoTarget
                       )
    {
        if (q_TemmoTarget.TargetTo == Entity.Null)
        {
            Debug.Log("Q_temmo dont have target");


        }
        else
        {
            //MoveToTarget
            var newLtrans = new LocalTransform
            {
                Position = Q_temmoTrans.Position,
                Rotation = Q_temmoTrans.Rotation,
                Scale = Q_temmoTrans.Scale
            };
            if ((q_TemmoTarget.TargetPos - Q_temmoTrans.Position).x != 0 || (q_TemmoTarget.TargetPos - Q_temmoTrans.Position).y != 0)
            {
                newLtrans.Position += deltaTime * q_Temmo.flySpeed * math.normalize(q_TemmoTarget.TargetPos - Q_temmoTrans.Position);
                ecbp.SetComponent<LocalTransform>(ciqi, ent, newLtrans);
                if (math.distancesq(q_TemmoTarget.TargetPos, Q_temmoTrans.Position) <= q_Temmo.distancesDealDamage)
                {
                    ecbp.AppendToBuffer<DealDamageSys2_OwnerComponent>(ciqi, q_TemmoTarget.TargetTo, new DealDamageSys2_OwnerComponent
                    { effectCount = 0.1f, effectFrequenc = 1, isLoop = true, loopCount = 1, Value = q_Temmo.DamageBasic, OriginCharacter = Entity.Null, type = SkillType.E_Morgana });
                    ecbp.AddComponent<DeadDestroyTag>(ciqi, ent);
                }
            }
        }


    }
}