
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public partial struct TargetToEnemySystem : ISystem
{
    private EntityQuery m_playersEQG;
    private EntityQuery m_EnemysEQG;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<TargetToEnemySystemEnable>();
        state.RequireForUpdate<TargetToEnemySy_TargetComponent>();
        state.RequireForUpdate<TargetToEnemySy_OwnerComponent>();
        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<TargetToEnemySy_OwnerComponent>());
        m_EnemysEQG = state.GetEntityQuery(ComponentType.ReadOnly<TargetToEnemySy_TargetComponent>());
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        int i = 0;
        NativeArray<float3> enemyPosArr = new NativeArray<float3>(m_EnemysEQG.CalculateEntityCount(), Allocator.TempJob);
        foreach (var lToW in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<TargetToEnemySy_TargetComponent>())
        {
            enemyPosArr[i] = lToW.ValueRO.Position;
            i++;
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        PhysicsWorld physWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        state.Dependency = new TurnHeadToTarget
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecb.AsParallelWriter(),
            EnemyPosArr = enemyPosArr,
            physWorld = physWorld
        }.ScheduleParallel(m_playersEQG, state.Dependency);
        state.Dependency.Complete();
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
        WorldBounds = (0 << 2),
        UFOs = (2 << 4)
    };

    [BurstCompile]
    private void Execute([ChunkIndexInQuery] int ciqi, ref LocalTransform playerTras, in TargetToEnemySy_OwnerComponent playerC, in Entity playerEnt)
    {
        float nearestDis = float.MaxValue;
        float3 targetPos_forLeastDist = playerTras.Position;
        bool playersExist = false;
        // for now we don't care about any further knowledge than just, closest distance one or another.
        foreach (float3 enemyPos in EnemyPosArr)
        {
            playersExist = true;
            float sqDistEuclid = math.distancesq(enemyPos, playerTras.Position);
            if (nearestDis > sqDistEuclid)
            {
                nearestDis = sqDistEuclid;
                targetPos_forLeastDist = enemyPos;
            }
            //update 1 near hon

            if (sqDistEuclid > 1)//
            {
                // alsocheck through walls
                float sqDistPortal = float.MaxValue;
                float3 dirToPlayer = math.normalize(enemyPos - playerTras.Position);
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = playerTras.Position,
                    End = playerTras.Position - dirToPlayer * 10,//lengofRayCast
                    Filter = new CollisionFilter
                    {
                        BelongsTo = (uint)layer.UFOs,
                        CollidesWith = (uint)layer.WorldBounds,
                        GroupIndex = 0
                    }
                };

#if UNITY_EDITOR
                Debug.DrawLine(raycastInput.Start, raycastInput.End, Color.red, 0.25f);
                Debug.DrawLine(raycastInput.Start, raycastInput.Start + dirToPlayer * 10, Color.green, 0.25f);
#endif


            }
        }
        //GetMinComplete

        Quaternion lookRotation = Quaternion.LookRotation(playerTras.Position - targetPos_forLeastDist, new float3(0, 0, 1));

        for (int k = 0; k < playerC.turnSpeed  /*speed*/; k++)
        {
            float3 rotation = Quaternion.Lerp(playerTras.Rotation, lookRotation, 0.01f).eulerAngles;//do chinh xac
            playerTras.Rotation = (Quaternion.Euler(0, 0, rotation.z));
        }

    }
}
public static class MathHelpers
{
    public static float GetHeading(float3 objectPosition, float3 targetPosition)
    {
        var x = objectPosition.x - targetPosition.x;
        var y = objectPosition.z - targetPosition.z;
        return math.atan2(x, y) + math.PI;
    }
}