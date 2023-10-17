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
        state.RequireForUpdate<EnemyAISys_TargetComponent>();
        state.RequireForUpdate<EnemyAI_OwnerComponent>();

        m_playersEQG = state.GetEntityQuery(ComponentType.ReadOnly<EnemyAISys_TargetComponent>());
        m_UFOsEQG = state.GetEntityQuery(ComponentType.ReadOnly<EnemyAI_OwnerComponent>());
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        int i = 0;
        NativeArray<float3> playerPosArr = new NativeArray<float3>(m_playersEQG.CalculateEntityCount(), Allocator.TempJob);
        foreach (var lToW in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyAISys_TargetComponent>())
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
    private void Execute([ChunkIndexInQuery] int ciqi, in LocalTransform localTr, in EnemyAI_OwnerComponent enemyAI,
        in Entity ufoEnt, in DynamicBuffer<AnimationParent_ElementComponent> animator, ref EnemyStateComponent stateComponent, ref AutoHitSys_StatusComponent stateHit)
    {

        float sq_MinLengPath = float.MaxValue;
        float3 target_Pos = localTr.Position;
        bool playersExist = false;
        // for now we don't care about any further knowledge than just, closest distance one or another.
        foreach (float3 playerPos in playerPosArr)
        {
            playersExist = true;
            float sq_leng = math.distancesq(playerPos, localTr.Position);
            if (sq_MinLengPath > sq_leng)
            {
                sq_MinLengPath = sq_leng;
                target_Pos = playerPos;
            }

            if (sq_leng > enemyAI.minChaseDist)
            {
                // alsocheck through walls
                float sqDistPortal = float.MaxValue;
                float3 dirToPlayer = math.normalize(playerPos - localTr.Position);
                RaycastInput raycastInput = new RaycastInput()
                {
                    Start = localTr.Position,
                    End = localTr.Position - dirToPlayer * 10,//gan dung
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
        if (playersExist && totalDist >= enemyAI.minChaseDist)
        {
            var newLtrans = new LocalTransform
            {
                Position = localTr.Position,
                Rotation = localTr.Rotation,
                Scale = localTr.Scale
            };
            if (stateComponent.state == EnemyStateComponent.State.Running && !stateHit.Hitting)
            {
                if ((target_Pos - localTr.Position).x != 0)
                {
                    newLtrans.Position += deltaTime * enemyAI.moveSpeed * math.normalize(target_Pos - localTr.Position);
                }
            }
            else if (stateComponent.state == EnemyStateComponent.State.ReceiveDamage)
            {
                if ((target_Pos - localTr.Position).x != 0)
                {
                    newLtrans.Position += deltaTime * enemyAI.moveSpeed * 0.5f * (-math.normalize(target_Pos - localTr.Position));
                }
            }
            ecbp.SetComponent<LocalTransform>(ciqi, ufoEnt, newLtrans);
            stateHit.CanHit = false;
        }
        else
        {
            //SpawnHit
            stateHit.CanHit = true;

        }
        bool isRight = target_Pos.x - localTr.Position.x > 0 ? true : false;
        ecbp.SetComponent<IsFlipTag>(ciqi, animator[0].animParent, new IsFlipTag { isRight = isRight });

        if (stateComponent.state == EnemyStateComponent.State.ReceiveDamage)
        {
            if (stateComponent.statCountDown >= 0)
                stateComponent.statCountDown -= deltaTime;
            else
            {
                stateComponent.state = EnemyStateComponent.State.Running;
                //default Anim
                ecbp.SetComponent<SpriteSheetAnimation>(ciqi, animator[0].animParent, new SpriteSheetAnimation
                {
                    indexAnim = 2,
                    maxSprite = 4,
                    _frameCountdown = 0.25f,
                    nextframe = 0.25f,
                    repeatition = SpriteSheetAnimation.RepeatitionType.LOOP,
                    animationFrameIndex = 0
                });
            }
        }
    }
}
