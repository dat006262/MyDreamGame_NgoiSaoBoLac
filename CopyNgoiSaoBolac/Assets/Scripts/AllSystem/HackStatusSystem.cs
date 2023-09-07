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
                },
                Calculate = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.Calculate.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.Calculate.keyCode)
                },
                EquipItem = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.EquipItem.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.EquipItem.keyCode)
                },
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
        state.RequireForUpdate<CheckNeedCalculate>();
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

        //GetItem   
        Entity item;
        // EntityQuery x = state.GetEntityQuery(ComponentType.ReadOnly<ItemComponent>());

        item = SystemAPI.GetSingletonEntity<ItemComponent>();// 11
        DynamicBuffer<StatModify> itemMod = state.EntityManager.GetBuffer<StatModify>(item, true);
        var x = itemMod.AsNativeArray();
        NativeArray<StatModify> copyBuffer = new NativeArray<StatModify>(x.Length, Allocator.TempJob);
        x.CopyTo(copyBuffer);
        //tam thoi
        EquipByPlayerComponent EquipByPlayerComponent = state.EntityManager.GetComponentData<EquipByPlayerComponent>(item);
        state.Dependency = new HackJob
        {
            Item = item,
            EquipByPlayerComponent = EquipByPlayerComponent,
            itemMods = copyBuffer,
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
    public Entity Item;
    public NativeArray<StatModify> itemMods;
    public EquipByPlayerComponent EquipByPlayerComponent;
    [ReadOnly]
    public float deltaTime;
    //[ReadOnly] public BufferLookup<StatModify> statModify;

    public EntityCommandBuffer.ParallelWriter ecbp;
    public void Execute([ChunkIndexInQuery] int ciqi, in PlayerComponent plComp, in Entity ent,
                        in HackInputComponent input, in CheckNeedCalculate check, ref DynamicBuffer<StatModify> dynamicBuffer,
                        in LocalTransform ltrans, in WorldTransform wtrans)
    {


        // DynamicBuffer<StatModify> getBuffer = statModify[ent];
        // rotate
        if (input.PlusHealth.keyVal)
        {
            StatModify newModifi = new StatModify
            {
                Value = 100,
                statModType = StatModType.Flat,
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
                Value = 0.1f,
                statModType = StatModType.PercentAdd,
                statType = StatType.Health,
                order = 300,
                Source = Entity.Null
            };
            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);
        }

        if (input.MultiHealth.keyVal)

        {
            StatModify newModifi = new StatModify
            {
                Value = 0.1f,
                statModType = StatModType.PercentMulti,
                statType = StatType.Health,
                order = 300,
                Source = Entity.Null
            };
            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);

        }
        if (input.Calculate.keyVal)
        {
            ecbp.SetComponent<CheckNeedCalculate>(ciqi, ent, new CheckNeedCalculate { dirty = true });
        }
        if (input.EquipItem.keyVal)
        {



            if (!EquipByPlayerComponent.equip)//false
            {
                foreach (StatModify statModify in itemMods)
                {
                    ecbp.AppendToBuffer<StatModify>(ciqi, ent, statModify);
                }


                ecbp.SetComponent<EquipByPlayerComponent>(ciqi, Item, new EquipByPlayerComponent { equip = true });
            }
            else
            {
                #region Cach1 : dung in DynamicBuffer<StatModify> dynamicBuffer(chi doc)
                //var x = dynamicBuffer.AsNativeArray();
                //NativeArray<StatModify> copyBuffer = new NativeArray<StatModify>(x.Length, Allocator.Temp);
                //x.CopyTo(copyBuffer);
                //ecbp.SetBuffer<StatModify>(ciqi, ent);
                //for (int i = copyBuffer.Length - 1; i >= 0; i--)
                //{
                //    if (copyBuffer[i].Source != Item)
                //    {
                //        ecbp.AppendToBuffer<StatModify>(ciqi, ent, copyBuffer[i]);
                //    }
                //}
                //ecbp.SetComponent<EquipByPlayerComponent>(ciqi, Item, new EquipByPlayerComponent { equip = false });

                #endregion
                //-------------------Test 2
                #region Cach2 : dung ref DynamicBuffer<StatModify> dynamicBuffer(doc va ghi)
                for (int i = dynamicBuffer.Length - 1; i >= 0; i--)
                {
                    if (dynamicBuffer[i].Source == Item)
                    {
                        dynamicBuffer.RemoveAt(i);
                    }
                }
                ecbp.SetComponent<EquipByPlayerComponent>(ciqi, Item, new EquipByPlayerComponent { equip = false });
                #endregion
            }
        }
    }
}

