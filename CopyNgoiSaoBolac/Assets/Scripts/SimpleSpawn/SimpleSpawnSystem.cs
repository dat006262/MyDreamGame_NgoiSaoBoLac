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

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<SpawnSimpleSystemEnable>();
        state.RequireForUpdate<SimpleSpawner_OwnerComponent>();
        state.RequireForUpdate<SimpleSpawner_PrefabAndParentBufferComponent>();
        state.RequireForUpdate<SimpleSpawner_PrefabAndParentBufferComponent>();

        spawnerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
           .WithAll<SimpleSpawner_OwnerComponent>()
           );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);//Tao ECB

        state.Dependency = new SpawnerJob
        {
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(spawnerEQG, state.Dependency);

        state.Dependency.Complete();
        state.Enabled = false;
    }
}
[BurstCompile]
public partial struct SpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecbp;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int ciqi, in DynamicBuffer<SimpleSpawner_PrefabAndParentBufferComponent> prefabsAndParents,
        in SimpleSpawner_OwnerComponent spawnerComp)
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
    }
}

