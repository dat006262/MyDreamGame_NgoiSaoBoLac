using DAT;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public partial struct RandomSpawnEnemy_WorkSystem : ISystem
{
    private EntityQuery spawnerEQG;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ConfigComponent>();
        state.RequireForUpdate<RandomSpawnSystemEnable>();
        state.RequireForUpdate<RandomSpawnEnemyTag>();
        spawnerEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp)
           .WithAll<RandomSpawnEnemyTag>()
           );
    }
}
