
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


public partial struct TargetToEnemySystem : ISystem
{
    private EntityQuery m_playersEQG;
    private EntityQuery m_UFOsEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TargetToEnemySystemEnable>();
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<PlayerComponent>();
        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
        m_UFOsEQG = state.GetEntityQuery(ComponentType.ReadOnly<EnemyComponent>());
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        int i = 0;
        NativeArray<float3> enemyPosArr = new NativeArray<float3>(m_UFOsEQG.CalculateEntityCount(), Allocator.TempJob);
        foreach (var lToW in SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<PlayerComponent>())
        {
            enemyPosArr[i] = lToW.ValueRO.Position;
            i++;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        PhysicsWorld physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

    }
}

[BurstCompile]
public partial struct TurnHeadToTarget : IJobEntity
{
    [ReadOnly]
    public float deltaTime;
    public EntityCommandBuffer.ParallelWriter ecbp;
    [ReadOnly]
    public NativeArray<float3> EnemyPosArr;
    [ReadOnly]
    public PhysicsWorld physWorld;

    enum layer
    {
        // TODO: but really would be nice to actually access the Layers defined in settings.
        WorldBounds = (1 << 9),
        UFOs = (1 << 15)
    };

    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int ciqi, in LocalToWorld playerLtoW, in LocalTransform playerTras, in PlayerComponent playerC, in Entity playerEnt)
    {
        float nearestDis = float.MaxValue;
        float3 target_forLeastDist = playerTras.Position;
        bool playersExist = false;
        // for now we don't care about any further knowledge than just, closest distance one or another.
        foreach (float3 enemyPos in EnemyPosArr)
        {
            playersExist = true;
            float sqDistEuclid = math.distancesq(enemyPos, playerLtoW.Position);
            if (nearestDis > sqDistEuclid)
            {
                nearestDis = sqDistEuclid;
                target_forLeastDist = enemyPos;
            }
            //update 1 near hon

            if (sqDistEuclid > 1)//
            {
                // alsocheck through walls
                float sqDistPortal = float.MaxValue;
                float3 dirToPlayer = math.normalize(enemyPos - playerLtoW.Position);
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = playerLtoW.Position,
                    End = -dirToPlayer * 10,//lengofRayCast
                    Filter = new CollisionFilter
                    {
                        BelongsTo = (uint)layer.UFOs,
                        CollidesWith = (uint)layer.WorldBounds,
                        GroupIndex = 0
                    }
                };

#if UNITY_EDITOR
                Debug.DrawLine(raycastInput.Start, raycastInput.End, Color.red, 0.25f);
                Debug.DrawLine(raycastInput.Start, dirToPlayer * 100, Color.green, 0.25f);
#endif


            }
        }

        // move towards player or patrol
        {
            float totalDist = math.sqrt(nearestDis);
            if (playersExist && totalDist <= 10 && totalDist >= 1)
            {
                var newLtrans = new LocalTransform
                {
                    Position = playerTras.Position,
                    Rotation = playerTras.Rotation,
                    Scale = playerTras.Scale
                };
                if ((target_forLeastDist - playerLtoW.Position).x != 0)
                    newLtrans.Position += deltaTime * playerC.moveSpeed * math.normalize(target_forLeastDist - playerLtoW.Position);
                ecbp.SetComponent<LocalTransform>(ciqi, playerEnt, newLtrans);
            }
            else
            {// "patrol state"
                var newLtrans = new LocalTransform
                {
                    Position = playerTras.Position,
                    Rotation = playerTras.Rotation,
                    Scale = playerTras.Scale
                };
                newLtrans.Position += deltaTime * playerC.moveSpeed * newLtrans.Right();
                ecbp.SetComponent<LocalTransform>(ciqi, playerEnt, newLtrans);
            }
        }
    }
}
