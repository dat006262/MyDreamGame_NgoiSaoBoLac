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


public partial class HackStatusSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<HackStatusSystemEnable>();
        RequireForUpdate<HackInputComponent>();
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
            var hackComp = SystemAPI.GetComponent<HackInputComponent>(ent);
            ecb.SetComponent<HackInputComponent>(ent, new HackInputComponent
            {
                PlusHealth = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.PlusHealth.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.PlusHealth.keyCode)
                },
                PlusMana = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.PlusMana.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.PlusMana.keyCode)
                },
                MultiHealth = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.MultiHealth.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.MultiHealth.keyCode)
                }
            });
        }).Run();
    }
}
[BurstCompile]
public partial struct HackSystem : ISystem
{
    // private BufferLookup<StatModify> statModify;
    private EntityQuery m_playersEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // at least one player in the scene
        state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<HackInputComponent>();
        state.RequireForUpdate<HackSystemEnable>();

        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        //  statModify = state.GetBufferLookup<StatModify>(true);
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



        state.Dependency = new HackJob
        {
            deltaTime = Time.deltaTime,
            //  statModify = statModify,
            ecbp = ecb.AsParallelWriter()
        }.ScheduleParallel(m_playersEQG, state.Dependency);
        state.Dependency.Complete();
    }
}


[BurstCompile]
public partial struct HackJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;
    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in PlayerComponent plComp, in Entity ent,
                        in HackInputComponent input,
                        in LocalTransform ltrans, in WorldTransform wtrans)
    {


        // DynamicBuffer<StatModify> getBuffer = statModify[ent];
        // rotate
        if (input.PlusHealth.keyVal)
        {
            StatModify newModifi = new StatModify
            {
                Value = 1,
                statModType = StatModType.PercentAdd,
                statType = StatType.Health,
                order = 300,
                Source = Entity.Null
            };
            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);


        }
        if (input.PlusMana.keyVal)
        {
            StatModify newModifi = new StatModify
            {
                Value = 1,
                statModType = StatModType.PercentAdd,
                statType = StatType.Mana,
                order = 300,
                Source = Entity.Null
            };
            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);
        }

        if (input.MultiHealth.keyVal)

        {
            StatModify newModifi = new StatModify
            {
                Value = 1,
                statModType = StatModType.PercentMulti,
                statType = StatType.Health,
                order = 300,
                Source = Entity.Null
            };
            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);
        }

    }
}

