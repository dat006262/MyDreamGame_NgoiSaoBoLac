using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

using UnityEngine;
using static DeadDestroySystem;

public partial struct RandomSpawnSystem : ISystem
{
    private EntityQuery spawnerEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<RandomSpawnSystemEnable>();
        state.RequireForUpdate<RandomSpawnComponent>();
        state.RequireForUpdate<SimpleSpawner_PrefabAndParentBufferComponent>();
        state.RequireForUpdate<RandomValue>();
        spawnerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
           .WithAll<RandomSpawnComponent>()
           .WithAll<RandomValue>()
           );
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);//Tao ECB
        var randomint = UnityEngine.Random.Range(1, 100000);
        state.Dependency = new RandomSpawmJob
        {
            randomint = randomint,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(spawnerEQG, state.Dependency);
        state.Dependency.Complete();
        state.Dependency = new SpawnWaveCoolDownJob
        {

            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter(),
        }.ScheduleParallel(spawnerEQG, state.Dependency);
        state.Dependency.Complete();


    }

}
[BurstCompile]
public partial struct SpawnWaveCoolDownJob : IJobEntity
{

    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;

    public void Execute([ChunkIndexInQuery] int ciqi, ref RandomSpawnComponent randomSpawn)
    {
        if (randomSpawn.waveTimeRemain >= 0)
            randomSpawn.waveTimeRemain -= deltaTime;
    }
}
[BurstCompile]
public partial struct RandomSpawmJob : IJobEntity
{
    public int randomint;
    public EntityCommandBuffer.ParallelWriter ecbp;
    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int ciqi, in DynamicBuffer<SimpleSpawner_PrefabAndParentBufferComponent> prefabsAndParents,
        WorldTransform wordTrans, in Entity entity,
        in DynamicBuffer<RandomSpawnPosBufferElement> spawnerPos, ref RandomSpawnComponent randomSpawnComp, ref RandomValue randomValue)
    {
        if (randomSpawnComp.waveTimeRemain >= 0) return;
        else
            randomSpawnComp.waveTimeRemain = randomSpawnComp.waveTimeColdDown;
        for (uint i = 0; i < randomSpawnComp.spawnPerWave; i++)
        {
            for (int j = 0; j < prefabsAndParents.Length; j++)
            {
                Entity prefabInstance = ecbp.Instantiate(ciqi, prefabsAndParents[j].prefab);


                randomValue.Value = Unity.Mathematics.Random.CreateFromIndex(((uint)randomint + i));
                var newrandomValue = randomValue.Value.NextInt(0, spawnerPos.Length);
                ecbp.AddComponent<LocalTransform>(ciqi, prefabInstance, new LocalTransform
                {
                    Position = wordTrans.Position + spawnerPos[newrandomValue].sumPos,
                    Rotation = wordTrans.Rotation,
                    Scale = wordTrans.Scale,

                    /*+ spawnerPos[newrandomValue].sumPos*/
                });
                ecbp.AddComponent<Unity.Transforms.Parent>(ciqi, prefabInstance, new Unity.Transforms.Parent
                {
                    Value = prefabsAndParents[j].parent//Add tagparent luon
                });


            }
        }
    }
}

