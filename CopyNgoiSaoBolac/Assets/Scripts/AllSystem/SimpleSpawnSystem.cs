using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public partial struct SimpleSpawnSystem : ISystem
{
    private EntityQuery spawnerEQG;
    private EntityQuery spawnerPlayer;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<SpawnSimpleSystemEnable>();
        state.RequireForUpdate<SimpleSpawnerComponent>();
        state.RequireForUpdate<PrefabAndParentBufferComponent>();

        spawnerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
           .WithAll<SimpleSpawnerComponent>()
           );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();//After cai j thi GetsingleTon caid day
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);//Tao ECB
        Debug.Log("[SimpleSpawner][InitialSpawn] spawning on game start. Bounds, Players, initial shield pickup.. ");

        //Entity stateCompEnt = SystemAPI.GetSingletonEntity<SimpleSpawnerComponent>();
        //var prefabsAndParents = SystemAPI.GetBuffer<PrefabAndParentBufferComponent>(stateCompEnt);
        state.Dependency = new SpawnerJob
        {
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(spawnerEQG, state.Dependency);

        //  ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        //new SpawnerJobPlayer
        //{
        //    ecbp = ecb.AsParallelWriter(),
        //    time = SystemAPI.Time.ElapsedTime
        //}.ScheduleParallel(spawnerPlayerEQG);


        state.Dependency.Complete();
        state.Enabled = false;
    }
}
[BurstCompile]
public partial struct SpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecbp;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int ciqi, in DynamicBuffer<PrefabAndParentBufferComponent> prefabsAndParents, in SimpleSpawnerComponent spawnerComp)
    {
        for (uint i = 0; i < spawnerComp.spawnNumber; i++)
        {
            for (int j = 0; j < prefabsAndParents.Length; j++)
            {
                Entity prefabInstance = ecbp.Instantiate(ciqi, prefabsAndParents[j].prefab);

                ecbp.AddComponent<Unity.Transforms.Parent>(ciqi, prefabInstance, new Unity.Transforms.Parent
                {
                    Value = prefabsAndParents[j].parent//Add tagparent luon
                });

            }
        }
        // ecbp.DestroyEntity(ciqi, prefabsAndParents[0].prefab);     
        //spawnerCompArr.Dispose();
    }
}

