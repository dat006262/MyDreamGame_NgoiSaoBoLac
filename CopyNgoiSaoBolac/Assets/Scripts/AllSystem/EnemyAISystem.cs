using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
public partial struct EnemyAISystem : ISystem
{
    private EntityQuery m_playersEQG;
    private EntityQuery m_UFOsEQG;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyAISystemEnable>();
        //state.RequireForUpdate<PlayerComponent>();
        state.RequireForUpdate<EnemyComponent>();

        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        m_UFOsEQG = state.GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
        //m_boundsGroup = state.GetEntityQuery(ComponentType.ReadOnly<BoundsTagComponent>());
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        int i = 0;
        NativeArray<float3> playerPosArr = new NativeArray<float3>(m_playersEQG.CalculateEntityCount(), Allocator.TempJob);
        foreach (var lToW in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerComponent>())
        {
            playerPosArr[i] = lToW.ValueRO.Position;
            i++;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        PhysicsWorld physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        state.Dependency = new EnemyChaseJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter(),
            playerPosArr = playerPosArr,
            physWorld = physWorld
        }.ScheduleParallel(m_UFOsEQG, state.Dependency);
        state.Dependency.Complete();
    }
}

[BurstCompile]
public partial struct EnemyChaseJob : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    [ReadOnly]
    public NativeArray<float3> playerPosArr;
    [ReadOnly]
    public PhysicsWorld physWorld;

    enum layer
    {
        // TODO: but really would be nice to actually access the Layers defined in settings.
        WorldBounds = (0 << 2),
        UFOs = (2 << 4)
    };

    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int ciqi, in LocalTransform localTrEnemy, in EnemyComponent ufoC, in Entity ufoEnt)
    {
        float sq_MinLengPath = float.MaxValue;
        float3 target_Pos = localTrEnemy.Position;
        bool playersExist = false;
        // for now we don't care about any further knowledge than just, closest distance one or another.
        foreach (float3 playerPos in playerPosArr)
        {
            playersExist = true;
            float sq_leng = math.distancesq(playerPos, localTrEnemy.Position);
            if (sq_MinLengPath > sq_leng)
            {
                sq_MinLengPath = sq_leng;
                target_Pos = playerPos;
            }

            if (sq_leng > ufoC.minChaseDist)
            {
                // alsocheck through walls
                float sqDistPortal = float.MaxValue;
                float3 dirToPlayer = math.normalize(playerPos - localTrEnemy.Position);
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = localTrEnemy.Position,
                    End = localTrEnemy.Position - dirToPlayer * 10,//gan dung
                    Filter = new CollisionFilter
                    {
                        BelongsTo = (uint)layer.UFOs,
                        CollidesWith = (uint)layer.WorldBounds,
                        GroupIndex = 0
                    }
                };

#if UNITY_EDITOR
                Debug.DrawLine(raycastInput.Start, raycastInput.End, Color.red, 0);
                Debug.DrawLine(raycastInput.Start, raycastInput.Start + dirToPlayer * 10, Color.green, 0);
#endif


            }
        }

        // GetMinComplete

        float totalDist = math.sqrt(sq_MinLengPath);
        if (playersExist && totalDist <= ufoC.maxChaseDist && totalDist >= ufoC.minChaseDist)
        {
            var newLtrans = new LocalTransform
            {
                Position = localTrEnemy.Position,
                Rotation = localTrEnemy.Rotation,
                Scale = localTrEnemy.Scale
            };
            if ((target_Pos - localTrEnemy.Position).x != 0)
                newLtrans.Position += deltaTime * ufoC.moveSpeed * math.normalize(target_Pos - localTrEnemy.Position);
            ecbp.SetComponent<LocalTransform>(ciqi, ufoEnt, newLtrans);
        }
        else
        {// "patrol state"
            var newLtrans = new LocalTransform
            {
                Position = localTrEnemy.Position,
                Rotation = localTrEnemy.Rotation,
                Scale = localTrEnemy.Scale
            };
            newLtrans.Position += deltaTime * ufoC.moveSpeed * newLtrans.Right();
            ecbp.SetComponent<LocalTransform>(ciqi, ufoEnt, newLtrans);
        }

    }
}
