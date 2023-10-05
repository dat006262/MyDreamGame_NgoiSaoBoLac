using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

#region Update InputSystemByFarme
public partial class HackInputSystem : SystemBase
{

    protected override void OnCreate()
    {
        RequireForUpdate<HackInputSystemEnable>();
        RequireForUpdate<HackInputComponent>();
    }
    protected override void OnDestroy()
    {
    }
    protected override void OnUpdate()//de cai dat cach nhan: VD di chuyen nhan giu dc con ban thì phai nhan
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
        Entities.ForEach((in PlayerMove_OwnerSystem plComp, in Entity ent) =>
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
                UseSkill = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.UseSkill.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.UseSkill.keyCode)
                },
                ChosseItem = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.ChosseItem.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.ChosseItem.keyCode)
                },
                DealDamage = new HackInputComponent.InputPair
                {
                    keyCode = hackComp.DealDamage.keyCode,
                    keyVal = Input.GetKeyDown(hackComp.DealDamage.keyCode)
                },
            });
        }).Run();
    }
}
#endregion
[BurstCompile]
public partial struct HackSystem : ISystem
{
    Entity Enemy;
    Entity Skill;
    Entity chosseItem;
    int testDamage;
    // private BufferLookup<StatModify> statModify;
    private EntityQuery m_ItemEQG;
    private EntityQuery m_playersEQG;
    private EntityQuery m_SkillEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //testDamage = 0;
        //// at least one player in the scene
        //state.RequireForUpdate<PlayerMove_OwnerSystem>();
        //state.RequireForUpdate<HackInputComponent>();
        //state.RequireForUpdate<CheckNeedCalculate>();
        //state.RequireForUpdate<HackSystemEnable>();

        //m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<PlayerMove_OwnerSystem>());
        //m_SkillEQG = state.GetEntityQuery(ComponentType.ReadOnly<SkillComponent>());
        //m_ItemEQG = state.GetEntityQuery(ComponentType.ReadOnly<ItemComponent>());
        ////  statModify = state.GetBufferLookup<StatModify>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //    Skill = Entity.Null;
        //    int numbercheck = 1001;
        //    var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        //    var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        //    //GetAllDataIn Item
        //    NativeArray<int> itemArray = new NativeArray<int>(m_ItemEQG.CalculateEntityCount(), Allocator.TempJob);
        //    int i = 0;
        //    foreach (var (ID, ent) in SystemAPI.Query<RefRO<ItemComponent>>().WithEntityAccess())
        //    {

        //        itemArray[i] = ID.ValueRO.ID;
        //        i++;
        //        if (ID.ValueRO.ID == numbercheck)
        //        {
        //            chosseItem = ent;
        //            //  int newID= chosseItem.Index;
        //        }
        //    }

        //    //GetItem   

        //    // item = SystemAPI.GetSingletonEntity<ItemComponent>();
        //    DynamicBuffer<StatModify> itemMod = state.EntityManager.GetBuffer<StatModify>(chosseItem, true);
        //    var x = itemMod.AsNativeArray();
        //    NativeArray<StatModify> copyBuffer = new NativeArray<StatModify>(x.Length, Allocator.TempJob);
        //    x.CopyTo(copyBuffer);
        //    EquipByPlayerComponent EquipByPlayerComponent = state.EntityManager.GetComponentData<EquipByPlayerComponent>(chosseItem);


        //    #region GetSkill
        //    NativeArray<float> skillArray = new NativeArray<float>(m_SkillEQG.CalculateEntityCount(), Allocator.TempJob);

        //    foreach (var (ID, ent) in SystemAPI.Query<RefRO<SkillCoolDownSys_OwnerComponent>>().WithEntityAccess())
        //    {
        //        //tam thoi

        //        Skill = ent;
        //    }
        //    SkillCoolDownSys_OwnerComponent skillCoolDownComponent = state.EntityManager.GetComponentData<SkillCoolDownSys_OwnerComponent>(Skill);

        //    #endregion
        //    #region GetEnemy
        //    NativeArray<float> enemyArray = new NativeArray<float>(m_SkillEQG.CalculateEntityCount(), Allocator.TempJob);

        //    foreach (var (ID, ent) in SystemAPI.Query<RefRO<EnemyAI_OwnerComponent>>().WithEntityAccess())
        //    {
        //        //tam thoi

        //        Enemy = ent;
        //    }

        //    #endregion

        //    state.Dependency = new HackJob
        //    {
        //        TestDamage = testDamage,
        //        numbercheck = numbercheck,
        //        itemArray = itemArray,
        //        Enemy = Enemy,
        //        Item = chosseItem,
        //        Skill = Skill,
        //        skillCoolDownComponent = skillCoolDownComponent,
        //        EquipByPlayerComponent = EquipByPlayerComponent,
        //        itemMods = copyBuffer,
        //        deltaTime = Time.deltaTime,
        //        ecbp = ecb.AsParallelWriter()

        //    }.ScheduleParallel(m_playersEQG, state.Dependency);
        //    state.Dependency.Complete();

    }
}


//[BurstCompile]
//public partial struct HackJob : IJobEntity
//{
//    public int TestDamage;
//    public Entity Enemy;
//    public int numbercheck;
//    public NativeArray<int> itemArray;

//    public Entity Item;
//    public NativeArray<StatModify> itemMods;
//    public EquipByPlayerComponent EquipByPlayerComponent;

//    public Entity Skill;
//    public SkillCoolDownSys_OwnerComponent skillCoolDownComponent;

//    [ReadOnly]
//    public float deltaTime;
//    //[ReadOnly] public BufferLookup<StatModify> statModify;

//    public EntityCommandBuffer.ParallelWriter ecbp;
//    public void Execute([ChunkIndexInQuery] int ciqi, in PlayerMove_OwnerSystem plComp, in Entity ent, /*ref DynamicBuffer<MiniItemComponent> miniItem,*/
//                        in HackInputComponent input, in CheckNeedCalculate check, ref DynamicBuffer<StatModify> dynamicBuffer,
//                        ref ChosseItemComponent chosse,
//                        in LocalTransform ltrans, in WorldTransform wtrans, ref CharacterAttackStrength damage)
//    {


//        // DynamicBuffer<StatModify> getBuffer = statModify[ent];
//        // rotate
//        #region Test CalculateStatModify
//        if (input.PlusHealth.keyVal)
//        {
//            StatModify newModifi = new StatModify
//            {
//                Value = 100,
//                statModType = StatModType.Flat,
//                statType = StatType.Health,
//                order = 300,
//                Source = Entity.Null
//            };
//            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);


//        }
//        if (input.PlusMana.keyVal)
//        {
//            StatModify newModifi = new StatModify
//            {
//                Value = 0.1f,
//                statModType = StatModType.PercentAdd,
//                statType = StatType.Health,
//                order = 300,
//                Source = Entity.Null
//            };
//            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);
//        }

//        if (input.MultiHealth.keyVal)

//        {
//            StatModify newModifi = new StatModify
//            {
//                Value = 0.1f,
//                statModType = StatModType.PercentMulti,
//                statType = StatType.Health,
//                order = 300,
//                Source = Entity.Null
//            };
//            ecbp.AppendToBuffer<StatModify>(ciqi, ent, newModifi);

//        }
//        if (input.Calculate.keyVal)
//        {
//            ecbp.SetComponent<CheckNeedCalculate>(ciqi, ent, new CheckNeedCalculate { dirty = true });
//        }
//        #endregion
//        #region TestEquip/UnEquip Item
//        if (input.EquipItem.keyVal)
//        {



//            if (!EquipByPlayerComponent.equip)//false
//            {
//                foreach (StatModify statModify in itemMods)
//                {
//                    ecbp.AppendToBuffer<StatModify>(ciqi, ent, statModify);
//                }


//                ecbp.SetComponent<EquipByPlayerComponent>(ciqi, Item, new EquipByPlayerComponent { equip = true });
//            }
//            else
//            {
//                #region Cach1 : dung in DynamicBuffer<StatModify> dynamicBuffer(chi doc)
//                //var x = dynamicBuffer.AsNativeArray();
//                //NativeArray<StatModify> copyBuffer = new NativeArray<StatModify>(x.Length, Allocator.Temp);
//                //x.CopyTo(copyBuffer);
//                //ecbp.SetBuffer<StatModify>(ciqi, ent);
//                //for (int i = copyBuffer.Length - 1; i >= 0; i--)
//                //{
//                //    if (copyBuffer[i].Source != Item)
//                //    {
//                //        ecbp.AppendToBuffer<StatModify>(ciqi, ent, copyBuffer[i]);
//                //    }
//                //}
//                //ecbp.SetComponent<EquipByPlayerComponent>(ciqi, Item, new EquipByPlayerComponent { equip = false });

//                #endregion
//                //-------------------Test 2
//                #region Cach2 : dung ref DynamicBuffer<StatModify> dynamicBuffer(doc va ghi)
//                for (int i = dynamicBuffer.Length - 1; i >= 0; i--)
//                {
//                    if (dynamicBuffer[i].Source == Item)
//                    {
//                        dynamicBuffer.RemoveAt(i);
//                    }
//                }
//                ecbp.SetComponent<EquipByPlayerComponent>(ciqi, Item, new EquipByPlayerComponent { equip = false });
//                #endregion
//            }
//        }
//        #endregion
//        #region Test SkillCooldown
//        if (input.UseSkill.keyVal)
//        {
//            if (skillCoolDownComponent.canUse)
//                ecbp.SetComponent<SkillCoolDownSys_OwnerComponent>(ciqi, Skill, new SkillCoolDownSys_OwnerComponent { coolDown = skillCoolDownComponent.coolDown, remain = skillCoolDownComponent.coolDown });
//        }
//        #endregion
//        #region TestFound Item ByID
//        if (input.ChosseItem.keyVal)
//        {
//            int j = 0;
//            foreach (int id in itemArray)
//            {
//                if (id == numbercheck)
//                {
//                    Debug.Log("Founded!!!!!!!!!!");
//                    chosse.ID = id;


//                    break;
//                }
//                j++;
//            }
//        }
//        #endregion
//        #region TestDealDamage
//        if (input.DealDamage.keyVal)
//        {
//            damage.Value++;
//            ecbp.AddComponent<DealDamageSys_OwnerComponent>(ciqi, Enemy, new DealDamageSys_OwnerComponent { Value = damage.Value, OriginCharacter = ent });
//        }
//        #endregion
//    }
//}




