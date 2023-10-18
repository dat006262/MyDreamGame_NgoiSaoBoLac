using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
public partial class AnimationSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<SpriteSheetAnimation>();
        RequireForUpdate<AnimationSystemEnable>();
    }


    protected override void OnDestroy()
    {
    }
    protected override void OnUpdate()//de cai dat cach nhan: VD di chuyen nhan giu dc con ban thì phai nhan
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
        bool isRight = true;

        Entities.WithoutBurst().ForEach((in SpriteRenderer spriteRender, in Entity ent, in SpriteSheetAnimation anim, in IsFlipTag isFlipTag) =>
        {
            if (!spriteRender.enabled) { return; }
            spriteRender.sprite = AnimationManager.intances.animations[anim.indexAnim].sprites[anim.animationFrameIndex];

        }).Run();
    }
}

[BurstCompile]
public partial struct Animation_NextFrameSystem : ISystem
{
    private EntityQuery m_animEQG;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        state.RequireForUpdate<SpriteSheetAnimation>();
        m_animEQG = state.GetEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<SpriteSheetAnimation>());
    }
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbEnd = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        state.Dependency = new Animation_NextFrameJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecbp = ecbEnd.AsParallelWriter(),
        }.ScheduleParallel(m_animEQG, state.Dependency);
        state.Dependency.Complete();
    }
    [BurstCompile]
    public partial struct Animation_NextFrameJob : IJobEntity
    {
        [ReadOnly]
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbp;

        public void Execute([ChunkIndexInQuery] int ciqi,
                            ref SpriteSheetAnimation nextframe)
        {

            nextframe.nextframe -= deltaTime;
            if (nextframe.nextframe <= 0)
            {
                nextframe.nextframe = nextframe._frameCountdown;
                nextframe.animationFrameIndex++;
                if (nextframe.animationFrameIndex >= nextframe.maxSprite)
                {
                    nextframe.animationFrameIndex = 0;
                }
            }
        }
    }
}